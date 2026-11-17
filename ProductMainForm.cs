using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TemplateMatchDemo
{
    /// <summary>
    /// 启动主界面：主流视觉软件排版 —— 中央常驻图像区（当前产品 + 红色高亮）、
    /// 底部结果表格（多张产品）、右侧结果面板。模板通过独立窗口按需定义。
    /// </summary>
    public sealed partial class ProductMainForm : Form
    {
        private sealed class ProductItem
        {
            public string Path, Name;
            public MatchResult Result = new MatchResult();
            public List<FeatureLine> Lines = new List<FeatureLine>();
            public bool Matched, Ok;
        }

        private readonly TemplateData Tpl = DemoState.Template;
        private readonly List<ProductItem> _items = new List<ProductItem>();

        private bool _suppressRoi, _roiDragging;
        private Point _dragA, _dragB;
        private int _viewIndex = -1;
        private Bitmap _viewImage;

        public ProductMainForm()
        {
            InitializeComponent();   // 界面布局在 ProductMainForm.Designer.cs，可在设计器里拖拽编辑
            // 设计器打开时不要执行业务逻辑，避免把设计时的控件状态改乱
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            SetTolInputs();
            // 6 个上下限输入框任意一个改动 → 写回模板 + 持久化 + 立即重判（不用重新比对）
            EventHandler h = TolChanged;
            _tolXMin.ValueChanged += h; _tolXMax.ValueChanged += h;
            _tolYMin.ValueChanged += h; _tolYMax.ValueChanged += h;
            _tolAngMin.ValueChanged += h; _tolAngMax.ValueChanged += h;
            RefreshHeader();
            RefreshRoiPanel();
            RefreshAll();
            SetStatus("工作台已就绪。点【添加产品】导入图片；模板点【添加模板】新建，"
                + "识别区域 ROI 在右侧面板设置，也可直接在图像区拖框。");
        }

        // ---------- 工具栏 / 面板事件（供设计器绑定）----------
        private bool _suppressTol;

        /// <summary>把模板里的容差同步到信息栏的 6 个输入框（初始化时调用，避免触发写回）。</summary>
        private void SetTolInputs()
        {
            _suppressTol = true;
            _tolXMin.Value = ClampVal(_tolXMin, (decimal)Tpl.TolXMin);
            _tolXMax.Value = ClampVal(_tolXMax, (decimal)Tpl.TolXMax);
            _tolYMin.Value = ClampVal(_tolYMin, (decimal)Tpl.TolYMin);
            _tolYMax.Value = ClampVal(_tolYMax, (decimal)Tpl.TolYMax);
            _tolAngMin.Value = ClampVal(_tolAngMin, (decimal)Tpl.TolAngMin);
            _tolAngMax.Value = ClampVal(_tolAngMax, (decimal)Tpl.TolAngMax);
            _suppressTol = false;
        }

        private static decimal ClampVal(NumericUpDown n, decimal v)
        {
            if (v < n.Minimum) return n.Minimum;
            if (v > n.Maximum) return n.Maximum;
            return v;
        }

        /// <summary>任一容差输入框改动：写回模板 → 持久化 → 立即重判合格与否。</summary>
        private void TolChanged(object sender, EventArgs e)
        {
            if (_suppressTol) return;
            // 下限必须小于上限，越界时自动把另一端顶开，避免判定条件恒为假
            if (_tolXMin.Value >= _tolXMax.Value) _tolXMax.Value = _tolXMin.Value + 0.01m;
            if (_tolYMin.Value >= _tolYMax.Value) _tolYMax.Value = _tolYMin.Value + 0.01m;
            if (_tolAngMin.Value >= _tolAngMax.Value) _tolAngMax.Value = _tolAngMin.Value + 0.5m;

            _suppressTol = true;
            Tpl.TolXMin = (double)_tolXMin.Value;
            Tpl.TolXMax = (double)_tolXMax.Value;
            Tpl.TolYMin = (double)_tolYMin.Value;
            Tpl.TolYMax = (double)_tolYMax.Value;
            Tpl.TolAngMin = (double)_tolAngMin.Value;
            Tpl.TolAngMax = (double)_tolAngMax.Value;
            _suppressTol = false;

            CalibrationStore.SaveAll(Tpl);   // 容差跟随标定一起持久化
            RecheckVerdicts();
        }

        /// <summary>只按当前容差重判每张产品是否合格（不重新提图/比对）。</summary>
        private void RecheckVerdicts()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                ProductItem it = _items[i];
                if (!it.Matched) continue;
                it.Ok = it.Result.Found
                    && it.Result.DetX >= Tpl.TolXMin && it.Result.DetX <= Tpl.TolXMax
                    && it.Result.DetY >= Tpl.TolYMin && it.Result.DetY <= Tpl.TolYMax
                    && it.Result.Angle >= Tpl.TolAngMin && it.Result.Angle <= Tpl.TolAngMax;
            }
            RefreshGrid();
        }

        private void btnAddTemplate_Click(object sender, EventArgs e) { OpenTemplate(); }
        private void btnEditTemplate_Click(object sender, EventArgs e) { EditTemplate(); }
        private void btnAddProduct_Click(object sender, EventArgs e) { AddProducts(); }
        private void btnRunAll_Click(object sender, EventArgs e) { RunAll(); }
        private void btnExportCsv_Click(object sender, EventArgs e) { ExportCsv(); }
        private void btnCalibrate_Click(object sender, EventArgs e) { OpenCalibration(); }
        private void _roiList_SelectedIndexChanged(object sender, EventArgs e) { SyncRoiInputsFromList(); }
        private void btnApply_Click(object sender, EventArgs e) { ApplyRoiFromInputs(); }
        private void btnAll_Click(object sender, EventArgs e) { FrameWholeImage(); }
        private void btnClr_Click(object sender, EventArgs e) { ClearRois(); }
        private void _grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _items.Count) ShowImage(e.RowIndex);
        }

        // ---------- 刷新 ----------
        private void RefreshAll()
        {
            RefreshHeader();
            RefreshGrid();
            _gridHost.Visible = _items.Count > 0;
            if (_viewIndex >= _items.Count) _viewIndex = -1;
            if (_viewIndex >= 0) ShowImage(_viewIndex);
            else
            {
                _pic.Image = null;
                _pic.Invalidate();
                RefreshResultPanel(null);
            }
        }

        private void RefreshHeader()
        {
            bool hasTpl = Tpl.CountByRole(FeatureRole.Position) > 0;
            if (hasTpl)
            {
                string n = string.IsNullOrEmpty(Tpl.SamplePath)
                    ? "自定义模板" : Path.GetFileName(Tpl.SamplePath);
                _lblTemplate.Text = "模板：已添加  " + n + "   ｜ 线 "
                    + Tpl.Lines.Count + " 条（绿 " + Tpl.CountByRole(FeatureRole.Position)
                    + " / 黄 " + Tpl.CountByRole(FeatureRole.Attached)
                    + " / 紫 " + Tpl.CountByRole(FeatureRole.Ignored) + "）";
                _lblTemplate.ForeColor = Color.DarkGreen;
            }
            else
            {
                _lblTemplate.Text = "模板：未添加   ｜ 随时可点【添加模板】定义样板";
                _lblTemplate.ForeColor = Color.Firebrick;
            }
            int done = 0;
            foreach (ProductItem it in _items) if (it.Matched) done++;
            _lblCount.Text = "产品 " + _items.Count + " 张 ｜ 已比对 " + done;
        }

        private void RefreshResultPanel(ProductItem it)
        {
            string tplInfo = Tpl.CountByRole(FeatureRole.Position) > 0
                ? "模板已就绪：绿 " + Tpl.CountByRole(FeatureRole.Position)
                  + " / 黄 " + Tpl.CountByRole(FeatureRole.Attached) + " 条"
                : "尚未定义模板";
            _resTemplate.Text = tplInfo + "\n标定：1px = "
                + Tpl.ScaleX.ToString("F6", CultureInfo.InvariantCulture) + " / "
                + Tpl.ScaleY.ToString("F6", CultureInfo.InvariantCulture) + " mm";
            if (it == null)
            {
                _curName.Text = "— 未选择产品 —";
                _resFound.Text = "匹配判定：—";
                _resDX.Text = "ΔX：—";
                _resDY.Text = "ΔY：—";
                _resAngle.Text = "角度偏移：—";
                _resScore.Text = "相似度：—";
                _resRequired.Text = "必要特征：—";
                _resVerdict.Text = "判定：—";
                _resVerdict.ForeColor = Color.DarkSlateGray;
                return;
            }
            _curName.Text = it.Name;
            if (!it.Matched)
            {
                _resFound.Text = "匹配判定：待比对";
                _resDX.Text = "ΔX：—";
                _resDY.Text = "ΔY：—";
                _resAngle.Text = "角度偏移：—";
                _resScore.Text = "相似度：—";
                _resRequired.Text = "必要特征：—";
                _resVerdict.Text = "判定：待比对";
                _resVerdict.ForeColor = Color.DarkSlateGray;
                return;
            }
            MatchResult m = it.Result;
            _resFound.Text = "匹配判定：" + (m.Found ? "✓ 找到" : "✗ 未找到");
            _resFound.ForeColor = m.Found ? Color.Green : Color.Red;
            _resDX.Text = "ΔX：" + m.DetX.ToString("F6", CultureInfo.InvariantCulture);
            _resDY.Text = "ΔY：" + m.DetY.ToString("F6", CultureInfo.InvariantCulture);
            _resAngle.Text = "角度偏移：" + (m.Angle >= 0 ? "+" : "")
                + m.Angle.ToString("F2", CultureInfo.InvariantCulture) + "°";
            _resScore.Text = "相似度：" + m.Score.ToString("F2", CultureInfo.InvariantCulture) + " 分";
            _resRequired.Text = Tpl.CountByRole(FeatureRole.Attached) == 0
                ? "必要特征：未设置"
                : "必要特征：" + (m.AttachedFound ? "存在" : "缺失");
            _resVerdict.Text = "判定：" + (it.Ok ? "合格" : "不合格");
            _resVerdict.ForeColor = it.Ok ? Color.Green : Color.Red;
        }

        private void RefreshGrid()
        {
            _grid.Rows.Clear();
            int ok = 0;
            for (int i = 0; i < _items.Count; i++)
            {
                ProductItem it = _items[i];
                MatchResult m = it.Result;
                string dx = it.Matched ? m.DetX.ToString("F6", CultureInfo.InvariantCulture) : "—";
                string dy = it.Matched ? m.DetY.ToString("F6", CultureInfo.InvariantCulture) : "—";
                string ang = it.Matched ? m.Angle.ToString("F2", CultureInfo.InvariantCulture) : "—";
                string score = it.Matched ? m.Score.ToString("F2", CultureInfo.InvariantCulture) : "—";
                string req = !it.Matched ? "—"
                    : (Tpl.CountByRole(FeatureRole.Attached) == 0 ? "未设置"
                        : (m.AttachedFound ? "存在" : "缺失"));
                string verdict = !it.Matched ? "待比对" : (it.Ok ? "合格" : "不合格");
                int row = _grid.Rows.Add(i + 1, it.Name, dx, dy, ang, score, req, verdict);
                DataGridViewCell c = _grid.Rows[row].Cells["Verdict"];
                if (!it.Matched)
                {
                    c.Style.BackColor = Color.FromArgb(238, 242, 248);
                    c.Style.ForeColor = Color.DarkSlateGray;
                }
                else
                {
                    if (it.Ok) ok++;
                    c.Style.BackColor = it.Ok
                        ? Color.FromArgb(215, 245, 215) : Color.FromArgb(255, 215, 215);
                    c.Style.ForeColor = it.Ok ? Color.DarkGreen : Color.DarkRed;
                }
                c.Style.SelectionBackColor = c.Style.BackColor;
                c.Style.SelectionForeColor = c.Style.ForeColor;
            }
            if (_summary != null)
                _summary.Text = "共 " + _items.Count + " 张 ｜ 合格 " + ok
                    + " ｜ 不合格 " + (_items.Count - ok);
        }

        // ---------- 模板（工作副本：取消时原模板完全不变）----------
        private static TemplateData CloneTemplate(TemplateData src)
        {
            TemplateData d = new TemplateData();
            d.Rect = src.Rect;
            d.Sensitivity = src.Sensitivity;
            d.MinArea = src.MinArea;
            d.SmoothLevel = src.SmoothLevel;
            d.ScaleX = src.ScaleX; d.ScaleY = src.ScaleY;
            d.SamplePath = src.SamplePath;
            d.ProductPath = src.ProductPath;
            d.LastThreshold = src.LastThreshold;
            foreach (Roi r in src.Rois)
                d.Rois.Add(new Roi { IsCircle = r.IsCircle, Bounds = r.Bounds });
            foreach (FeatureLine l in src.Lines) d.Lines.Add(CloneLine(l));
            return d;
        }

        private static FeatureLine CloneLine(FeatureLine s)
        {
            FeatureLine d = new FeatureLine();
            d.Points = new List<PointF>(s.Points);
            d.RawPoints = s.RawPoints == null ? null : new List<PointF>(s.RawPoints);
            d.Role = s.Role;
            d.Area = s.Area;
            d.Bounds = s.Bounds;
            d.IsRoundCandidate = s.IsRoundCandidate;
            d.Roundness = s.Roundness;
            d.IsFittedCircle = s.IsFittedCircle;
            d.CircleCenter = s.CircleCenter;
            d.CircleRadius = s.CircleRadius;
            return d;
        }

        private static void CommitTemplate(TemplateData dst, TemplateData src)
        {
            dst.Rect = src.Rect;
            dst.Sensitivity = src.Sensitivity;
            dst.MinArea = src.MinArea;
            dst.SmoothLevel = src.SmoothLevel;
            dst.ScaleX = src.ScaleX; dst.ScaleY = src.ScaleY;
            dst.SamplePath = src.SamplePath;
            dst.ProductPath = src.ProductPath;
            dst.LastThreshold = src.LastThreshold;
            dst.Rois.Clear();
            foreach (Roi r in src.Rois)
                dst.Rois.Add(new Roi { IsCircle = r.IsCircle, Bounds = r.Bounds });
            dst.Lines.Clear();
            foreach (FeatureLine l in src.Lines) dst.Lines.Add(CloneLine(l));
        }

        private void InvalidateResults()
        {
            foreach (ProductItem it in _items)
            {
                it.Matched = false;
                it.Ok = false;
                it.Result = new MatchResult();
                it.Lines.Clear();
            }
            RefreshHeader();
        }

        private void AfterTemplateSaved()
        {
            InvalidateResults();
            RefreshRoiPanel();
            if (_items.Count > 0) RunAll();   // 模板好了，已有产品自动重新检测
            else
            {
                RefreshAll();
                SetStatus("模板已保存：共 " + Tpl.Lines.Count + " 条线、"
                    + Tpl.Rois.Count + " 个 ROI。现在点【添加产品】导入图片。");
            }
        }

        private void OpenTemplate()
        {
            TemplateData work = CloneTemplate(Tpl);
            using (TemplateEditorForm f = new TemplateEditorForm(work, true))
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    CommitTemplate(Tpl, work);
                    AfterTemplateSaved();
                }
                else SetStatus("已取消，原模板保持不变。");
            }
        }

        private void EditTemplate()
        {
            if (Tpl.CountByRole(FeatureRole.Position) == 0)
            {
                SetStatus("还没有可编辑的模板，已打开空白模板窗口。");
                OpenTemplate();
                return;
            }
            TemplateData work = CloneTemplate(Tpl);
            using (TemplateEditorForm f = new TemplateEditorForm(work, false))
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    CommitTemplate(Tpl, work);
                    AfterTemplateSaved();
                }
                else SetStatus("已取消，原模板保持不变。");
            }
        }

        // ---------- 标定 ----------
        private void OpenCalibration()
        {
            using (CalibrationForm f = new CalibrationForm(Tpl.ScaleX, Tpl.ScaleY))
            {
                if (f.ShowDialog(this) != DialogResult.OK)
                {
                    SetStatus("已取消标定，原系数保持不变。");
                    return;
                }
                Tpl.ScaleX = f.ResultScaleX;
                Tpl.ScaleY = f.ResultScaleY;
                CalibrationStore.Save(Tpl.ScaleX, Tpl.ScaleY);
                // DetX / DetY 由 ScaleX / ScaleY 换算，改完必须重算所有结果
                if (_items.Count > 0 && Tpl.CountByRole(FeatureRole.Position) > 0)
                    RunAll();
                else RefreshAll();
                SetStatus("标定已应用：1 px = "
                    + Tpl.ScaleX.ToString("F6", CultureInfo.InvariantCulture) + " / "
                    + Tpl.ScaleY.ToString("F6", CultureInfo.InvariantCulture)
                    + " mm。所有 ΔX / ΔY 已按新系数重算，并保存到 calibration.txt。");
            }
        }

        // ---------- 识别区域 ROI ----------
        private void RefreshRoiPanel()
        {
            _suppressRoi = true;
            int sel = _roiList.SelectedIndex;
            _roiList.Items.Clear();
            for (int i = 0; i < Tpl.Rois.Count; i++)
            {
                Roi r = Tpl.Rois[i];
                Rectangle b = r.Bounds;
                _roiList.Items.Add("#" + (i + 1) + " " + (r.IsCircle ? "圆形" : "矩形")
                    + " (" + b.X + "," + b.Y + "," + b.Width + "×" + b.Height + ")");
            }
            if (Tpl.Rois.Count == 0)
            {
                _roiX.Value = 0; _roiY.Value = 0;
                _roiW.Value = 0; _roiH.Value = 0;
            }
            else
            {
                if (sel < 0 || sel >= Tpl.Rois.Count) sel = 0;
                _roiList.SelectedIndex = sel;
                SyncRoiInputsFromList();
            }
            _suppressRoi = false;
        }

        private void SyncRoiInputsFromList()
        {
            int i = _roiList.SelectedIndex;
            if (i < 0 || i >= Tpl.Rois.Count) return;
            _suppressRoi = true;
            Rectangle b = Tpl.Rois[i].Bounds;
            _roiX.Value = Math.Max(0, Math.Min(4000, b.X));
            _roiY.Value = Math.Max(0, Math.Min(4000, b.Y));
            _roiW.Value = Math.Max(0, Math.Min(4000, b.Width));
            _roiH.Value = Math.Max(0, Math.Min(4000, b.Height));
            _roiKind.SelectedIndex = Tpl.Rois[i].IsCircle ? 1 : 0;
            _suppressRoi = false;
        }

        private void ApplyRoiFromInputs()
        {
            if (_suppressRoi) return;
            int w = (int)_roiW.Value, h = (int)_roiH.Value;
            if (w < 10 || h < 10)
            { SetStatus("ROI 宽高至少要 10 像素。"); return; }
            Roi roi = new Roi();
            roi.IsCircle = _roiKind.SelectedIndex == 1;
            roi.Bounds = new Rectangle((int)_roiX.Value, (int)_roiY.Value, w, h);
            int sel = _roiList.SelectedIndex;
            if (sel >= 0 && sel < Tpl.Rois.Count) Tpl.Rois[sel] = roi;
            else Tpl.Rois.Add(roi);
            RefreshRoiPanel();
            RerunAfterRoi();
        }

        private void FrameWholeImage()
        {
            int w = 800, h = 580;
            if (_viewImage != null) { w = _viewImage.Width; h = _viewImage.Height; }
            Tpl.Rois.Clear();
            Tpl.Rois.Add(new Roi
            {
                IsCircle = false,
                Bounds = new Rectangle(0, 0, w, h)
            });
            RefreshRoiPanel();
            RerunAfterRoi();
        }

        private void ClearRois()
        {
            Tpl.Rois.Clear();
            RefreshRoiPanel();
            RerunAfterRoi();
        }

        private void RerunAfterRoi()
        {
            _pic.Invalidate();
            if (_items.Count > 0 && Tpl.CountByRole(FeatureRole.Position) > 0)
                RunAll();
            else
            {
                RefreshAll();
                SetStatus("ROI 已更新（共 " + Tpl.Rois.Count
                    + " 个）。添加模板后会自动按新区域重新检测。");
            }
        }

        private bool ClientToImage(Point client, out Point img)
        {
            img = new Point();
            if (_viewImage == null) return false;
            float scale = Math.Min(
                (float)_pic.ClientSize.Width / _viewImage.Width,
                (float)_pic.ClientSize.Height / _viewImage.Height);
            if (scale <= 0) return false;
            float ox = (_pic.ClientSize.Width - _viewImage.Width * scale) / 2f;
            float oy = (_pic.ClientSize.Height - _viewImage.Height * scale) / 2f;
            int x = (int)Math.Round((client.X - ox) / scale);
            int y = (int)Math.Round((client.Y - oy) / scale);
            img = new Point(Math.Max(0, Math.Min(_viewImage.Width - 1, x)),
                            Math.Max(0, Math.Min(_viewImage.Height - 1, y)));
            return true;
        }

        private void OnPicMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _viewImage == null) return;
            _dragA = _dragB = e.Location;
            _roiDragging = true;
        }

        private void OnPicMouseMove(object sender, MouseEventArgs e)
        {
            if (!_roiDragging) return;
            _dragB = e.Location;
            _pic.Invalidate();
        }

        private void OnPicMouseUp(object sender, MouseEventArgs e)
        {
            if (!_roiDragging) return;
            _roiDragging = false;
            _dragB = e.Location;
            Point a, b;
            if (!ClientToImage(_dragA, out a) || !ClientToImage(_dragB, out b))
            { _pic.Invalidate(); return; }
            int x0 = Math.Min(a.X, b.X), y0 = Math.Min(a.Y, b.Y);
            int w = Math.Abs(a.X - b.X), h = Math.Abs(a.Y - b.Y);
            if (w < 10 || h < 10) { _pic.Invalidate(); return; }
            bool circle = _roiKind.SelectedIndex == 1;
            if (circle) { w = h = Math.Min(w, h); }
            Roi roi = new Roi { IsCircle = circle, Bounds = new Rectangle(x0, y0, w, h) };
            int sel = _roiList.SelectedIndex;
            if (sel >= 0 && sel < Tpl.Rois.Count) Tpl.Rois[sel] = roi;
            else Tpl.Rois.Add(roi);
            RefreshRoiPanel();
            RerunAfterRoi();
        }

        // ---------- 产品 ----------
        private void AddProducts()
        {
            string[] files;
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Title = "一次选择多张产品图片";
                d.Multiselect = true;
                d.Filter = "图片文件|*.bmp;*.png;*.jpg;*.jpeg|全部文件|*.*";
                if (d.ShowDialog(this) != DialogResult.OK) return;
                files = d.FileNames;
            }
            int firstNew = _items.Count;
            foreach (string f in files)
            {
                ProductItem it = new ProductItem();
                it.Path = f; it.Name = Path.GetFileName(f);
                _items.Add(it);
            }
            _viewIndex = firstNew;   // 立刻显示新加的第一张
            RefreshAll();

            if (Tpl.CountByRole(FeatureRole.Position) > 0)
                RunAll();            // 有模板 → 添加即自动比对出高亮
            else
            {
                ShowImage(firstNew);
                SetStatus("已添加 " + files.Length + " 张产品图。尚未定义模板，"
                    + "点【添加模板】完成后会自动比对并显示高亮特征。");
            }
        }

        private void RunAll()
        {
            if (_items.Count == 0)
            {
                MessageBox.Show(this, "请先点【添加产品】导入产品图片。",
                    "还没有产品", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Tpl.CountByRole(FeatureRole.Position) == 0)
            {
                MessageBox.Show(this, "还没有可用模板。请先点【添加模板】，把位置边界设为【绿色·判断位置】。",
                    "缺少模板", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OpenTemplate();
                return;
            }
            double thr = (double)_scoreMin.Value;
            Cursor old = Cursor; Cursor = Cursors.WaitCursor;
            try
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    ProductItem it = _items[i];
                    SetStatus("正在比对 " + (i + 1) + " / " + _items.Count + "：" + it.Name);
                    Application.DoEvents();
                    try
                    {
                        using (Bitmap b = new Bitmap(it.Path))
                        {
                            int th, rej;
                            List<int> flt;
                            it.Lines = FeatureExtractor.ExtractAll(b, Tpl.Rois,
                                Tpl.Sensitivity, Tpl.MinArea, Tpl.SmoothLevel,
                                out th, out rej, out flt);
                            it.Result = _matchMode.SelectedIndex == 1
                                ? GeometryMatcher.Compare(Tpl, it.Lines, thr)
                                : Matcher.Match(b, Tpl, thr);
                            // 两种比对模式统一用「形状一对一」判定必要特征，避免切模式后结论不一致
                            GeometryMatcher.ComputeAttached(Tpl, it.Lines, it.Result);
                        }
                        MarkPositions(it);
                        it.Matched = true;
                        // 判定容差：X/Y 各自上下限 + 角度上下限（可在工具条「容差…」里设）
                        it.Ok = it.Result.Found
                            && it.Result.DetX >= Tpl.TolXMin && it.Result.DetX <= Tpl.TolXMax
                            && it.Result.DetY >= Tpl.TolYMin && it.Result.DetY <= Tpl.TolYMax
                            && it.Result.Angle >= Tpl.TolAngMin && it.Result.Angle <= Tpl.TolAngMax;
                    }
                    catch
                    {
                        it.Matched = true;
                        it.Ok = false;
                        it.Result = new MatchResult();
                        it.Lines = new List<FeatureLine>();
                    }
                }
            }
            finally { Cursor = old; }
            if (_viewIndex < 0 || _viewIndex >= _items.Count) _viewIndex = 0;
            RefreshAll();
            SetStatus("比对完成：共 " + _items.Count
                + " 张。上方显示当前产品的高亮特征，单击下方表格任一行可切换。");
        }

        /// <summary>把产品图上形状最像样板绿线的几条标成位置特征（红色高亮）。</summary>
        private void MarkPositions(ProductItem it)
        {
            it.Result.PositionIndices.Clear();
            if (!it.Result.Found || it.Lines.Count == 0) return;
            foreach (FeatureLine g in Tpl.Lines)
            {
                if (g.Role != FeatureRole.Position) continue;
                List<KeyValuePair<int, double>> cand = new List<KeyValuePair<int, double>>();
                for (int i = 0; i < it.Lines.Count; i++)
                {
                    double s = GeometryMatcher.ShapeSimilarity(g, it.Lines[i]);
                    if (s >= 0.70) cand.Add(new KeyValuePair<int, double>(i, s));
                }
                cand.Sort(delegate(KeyValuePair<int, double> a, KeyValuePair<int, double> b)
                { return b.Value.CompareTo(a.Value); });
                foreach (KeyValuePair<int, double> kv in cand)
                {
                    if (it.Result.PositionIndices.Contains(kv.Key)) continue;
                    it.Result.PositionIndices.Add(kv.Key);
                    break;
                }
            }
        }

        // ---------- 当前产品显示 ----------
        private void ShowImage(int idx)
        {
            if (idx < 0 || idx >= _items.Count) return;
            ProductItem it = _items[idx];
            try
            {
                if (_viewImage != null) { _viewImage.Dispose(); _viewImage = null; }
                _viewImage = new Bitmap(it.Path);
            }
            catch { SetStatus("这张图打不开：" + it.Name); return; }
            _viewIndex = idx;
            _pic.Image = _viewImage;
            _pic.Invalidate();
            RefreshResultPanel(it);
            if (_grid.Rows.Count > idx)
            {
                _grid.ClearSelection();
                _grid.CurrentCell = _grid.Rows[idx].Cells[0];
                _grid.Rows[idx].Selected = true;
                if (idx > 8)
                    _grid.FirstDisplayedScrollingRowIndex = Math.Max(0, idx - 4);
            }
            SetStatus("当前显示：" + it.Name + "（" + (it.Matched
                ? (it.Ok ? "合格" : "不合格") : "待比对") + "）。单击下方表格可切换其它产品。");
        }

        private void OnPaintImage(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 空状态：中央引导框，图像区始终常驻
            if (_viewImage == null || _viewIndex < 0 || _viewIndex >= _items.Count)
            {
                int w = _pic.ClientSize.Width, h = _pic.ClientSize.Height;
                int bw = Math.Min(560, w - 80), bh = 210;
                Rectangle rc = new Rectangle((w - bw) / 2, (h - bh) / 2, bw, bh);
                using (Pen dash = new Pen(Color.FromArgb(90, 90, 140, 175), 2f))
                {
                    dash.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(dash, rc);
                }
                string t = "添加产品图，开始检测";
                string s1 = "点顶部【添加产品】选择一张或多张产品图片";
                string s2 = "需要样板时，点【添加模板】在弹出窗口中定义";
                using (Font f1 = new Font("Microsoft YaHei UI", 22f, FontStyle.Bold))
                using (Font f2 = new Font("Microsoft YaHei UI", 11f))
                {
                    SizeF a = g.MeasureString(t, f1);
                    g.DrawString(t, f1, Brushes.White, (w - a.Width) / 2, rc.Y + 36);
                    SizeF b = g.MeasureString(s1, f2);
                    g.DrawString(s1, f2, Brushes.LightSteelBlue, (w - b.Width) / 2, rc.Y + 96);
                    SizeF c = g.MeasureString(s2, f2);
                    g.DrawString(s2, f2, Brushes.LightSteelBlue, (w - c.Width) / 2, rc.Y + 126);
                }
                return;
            }

            float scale = Math.Min(
                (float)_pic.ClientSize.Width / _viewImage.Width,
                (float)_pic.ClientSize.Height / _viewImage.Height);
            float ox = (_pic.ClientSize.Width - _viewImage.Width * scale) / 2f;
            float oy = (_pic.ClientSize.Height - _viewImage.Height * scale) / 2f;

            // 识别区域 + 四角 HUD 标记（主流视觉软件观感）
            foreach (Roi roi in Tpl.Rois)
            {
                float bx = ox + roi.Bounds.X * scale, by = oy + roi.Bounds.Y * scale;
                float bw = roi.Bounds.Width * scale, bh = roi.Bounds.Height * scale;
                using (Pen pen = new Pen(roi.IsCircle ? Color.Orange : Color.Lime, 2f))
                {
                    if (roi.IsCircle) pen.DashStyle = DashStyle.Dash;
                    if (roi.IsCircle) g.DrawEllipse(pen, bx, by, bw, bh);
                    else g.DrawRectangle(pen, bx, by, bw, bh);
                }
                if (!roi.IsCircle)
                {
                    using (Pen corner = new Pen(Color.Lime, 4f))
                    {
                        g.DrawLine(corner, bx, by, bx + 24, by);
                        g.DrawLine(corner, bx, by, bx, by + 24);
                        g.DrawLine(corner, bx + bw, by, bx + bw - 24, by);
                        g.DrawLine(corner, bx + bw, by, bx + bw, by + 24);
                        g.DrawLine(corner, bx, by + bh, bx + 24, by + bh);
                        g.DrawLine(corner, bx, by + bh, bx, by + bh - 24);
                        g.DrawLine(corner, bx + bw, by + bh, bx + bw - 24, by + bh);
                        g.DrawLine(corner, bx + bw, by + bh, bx + bw, by + bh - 24);
                    }
                }
            }

            ProductItem it = _items[_viewIndex];
            if (it.Matched)
            {
                HashSet<int> pos = new HashSet<int>(it.Result.PositionIndices);
                using (Pen glow = new Pen(Color.FromArgb(95, 255, 40, 40), 8f))
                    for (int i = 0; i < it.Lines.Count; i++)
                        if (pos.Contains(i)) DrawLine(g, it.Lines[i], glow, ox, oy, scale);
                for (int i = 0; i < it.Lines.Count; i++)
                {
                    bool hit = pos.Contains(i);
                    using (Pen pen = new Pen(hit ? Color.Red : Color.Cyan, hit ? 2.6f : 1.5f))
                        DrawLine(g, it.Lines[i], pen, ox, oy, scale);
                }
                if (!it.Result.Found)
                {
                    string msg = "未找到匹配  ｜  相似度 "
                        + it.Result.Score.ToString("F1", CultureInfo.InvariantCulture) + " 分";
                    using (Font f = new Font("Microsoft YaHei UI", 17f, FontStyle.Bold))
                    using (Brush bg = new SolidBrush(Color.FromArgb(205, 28, 28, 32)))
                    {
                        SizeF sz = g.MeasureString(msg, f);
                        float bw = sz.Width + 40, bh = sz.Height + 24;
                        float bx = (_pic.ClientSize.Width - bw) / 2f;
                        g.FillRectangle(bg, bx, 18, bw, bh);
                        g.DrawString(msg, f, Brushes.OrangeRed, bx + 20, 30);
                    }
                }
            }
            else
            {
                string msg = "尚未比对：点【添加模板】定义样板后会自动检测";
                using (Font f = new Font("Microsoft YaHei UI", 13f, FontStyle.Bold))
                using (Brush bg = new SolidBrush(Color.FromArgb(190, 28, 28, 32)))
                {
                    SizeF sz = g.MeasureString(msg, f);
                    float bw = sz.Width + 36, bh = sz.Height + 20;
                    float bx = (_pic.ClientSize.Width - bw) / 2f;
                    g.FillRectangle(bg, bx, 18, bw, bh);
                    g.DrawString(msg, f, Brushes.Khaki, bx + 18, 28);
                }
            }

            // 正在图像区拖拽的新 ROI（橙色虚线）
            if (_roiDragging)
            {
                Rectangle r = Rectangle.FromLTRB(
                    Math.Min(_dragA.X, _dragB.X), Math.Min(_dragA.Y, _dragB.Y),
                    Math.Max(_dragA.X, _dragB.X), Math.Max(_dragA.Y, _dragB.Y));
                using (Pen pen = new Pen(Color.Orange, 2f))
                {
                    pen.DashStyle = DashStyle.Dash;
                    if (_roiKind.SelectedIndex == 1)
                    {
                        int d = Math.Min(r.Width, r.Height);
                        g.DrawEllipse(pen, r.X, r.Y, d, d);
                    }
                    else g.DrawRectangle(pen, r);
                }
            }
        }

        private static void DrawLine(Graphics g, FeatureLine l, Pen pen,
            float ox, float oy, float scale)
        {
            if (l.Points == null || l.Points.Count < 2) return;
            PointF[] pts = new PointF[l.Points.Count];
            for (int i = 0; i < l.Points.Count; i++)
                pts[i] = new PointF(ox + l.Points[i].X * scale,
                                    oy + l.Points[i].Y * scale);
            g.DrawLines(pen, pts);
        }

        // ---------- 导出 ----------
        private void ExportCsv()
        {
            if (_items.Count == 0)
            { SetStatus("还没有产品结果可导出。"); return; }
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Title = "导出检测结果";
                d.Filter = "CSV 文件|*.csv";
                d.FileName = "产品检测结果.csv";
                if (d.ShowDialog(this) != DialogResult.OK) return;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("序号,文件名,ΔX(mm),ΔY(mm),角度(°),相似度,必要特征,判定,完整路径");
                for (int i = 0; i < _items.Count; i++)
                {
                    ProductItem it = _items[i];
                    MatchResult m = it.Result;
                    sb.Append(i + 1).Append(',').Append(Csv(it.Name)).Append(',')
                      .Append(it.Matched ? m.DetX.ToString("F6", CultureInfo.InvariantCulture) : "").Append(',')
                      .Append(it.Matched ? m.DetY.ToString("F6", CultureInfo.InvariantCulture) : "").Append(',')
                      .Append(it.Matched ? m.Angle.ToString("F2", CultureInfo.InvariantCulture) : "").Append(',')
                      .Append(it.Matched ? m.Score.ToString("F2", CultureInfo.InvariantCulture) : "").Append(',')
                      .Append(!it.Matched ? "待比对"
                          : (Tpl.CountByRole(FeatureRole.Attached) == 0
                              ? "未设置" : (m.AttachedFound ? "存在" : "缺失"))).Append(',')
                      .Append(!it.Matched ? "待比对" : (it.Ok ? "合格" : "不合格")).Append(',')
                      .Append(Csv(it.Path)).AppendLine();
                }
                File.WriteAllText(d.FileName, sb.ToString(), new UTF8Encoding(true));
                SetStatus("已导出 " + _items.Count + " 条结果到 " + d.FileName);
            }
        }

        private static string Csv(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            if (s.IndexOf(',') >= 0 || s.IndexOf('"') >= 0 || s.IndexOf('\n') >= 0)
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private void SetStatus(string s) { if (_status != null) _status.Text = s; }

    }
}

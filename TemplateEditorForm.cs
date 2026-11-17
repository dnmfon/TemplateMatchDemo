using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TemplateMatchDemo
{
    /// <summary>单一中文窗口：导入样板 → 拖框抓边界 → 定义绿/紫/黄 → 载入产品图比对偏移。</summary>
    public sealed partial class TemplateEditorForm : Form
    {
        private PictureBox _pic;
        private Bitmap _sample, _product;
        private Label _status;
        private Label _lblX, _lblY, _lblScore, _lblRequired, _lblAngle, _lblFound, _lblMode;
        private ComboBox _lineBox, _roiShape, _matchMode;
        private NumericUpDown _sens, _minArea, _rx, _ry, _rw, _rh, _smooth;
        private Button _btnFit;
        private readonly TemplateData Tpl;
        private readonly List<string> Images = DemoState.Images;
        private readonly bool _startBlank;
        private int _selected = -1;
        private bool _showProduct;
        private MatchResult _last = new MatchResult();
        private List<FeatureLine> _productLines = new List<FeatureLine>();

        // ---- 批量识别 ----
        private Panel _batchPanel;
        private DataGridView _grid;
        private Label _batchSummary;
        private NumericUpDown _batchScore, _batchTolXY, _batchTolAng;
        private readonly List<BatchItem> _batch = new List<BatchItem>();

        private sealed class BatchItem
        {
            public string Path, Name;
            public MatchResult Result;
            public bool Ok, HasRequired;
        }

        /// <summary>
        /// 无参构造函数：仅供 VS 窗体设计器实例化并显示控件。
        /// 业务逻辑（模板数据、图片加载）不在此处执行。
        /// </summary>
        public TemplateEditorForm()
        {
            InitializeComponent();
        }

        /// <param name="data">模板数据（主界面传入工作副本，取消时原模板不受影响）。</param>
        /// <param name="startBlank">true = 新建空白模板（不预置图片与框）；false = 编辑现有模板。</param>
        public TemplateEditorForm(TemplateData data, bool startBlank)
        {
            InitializeComponent();
            Tpl = data ?? DemoState.Template;
            _startBlank = startBlank;
            Text = startBlank ? "添加模板 — 模板特征比对" : "编辑模板 — 模板特征比对";
            HideResultGroup();   // 比对结果属于主界面，模板窗口不显示

            // 不预置任何默认框 / 默认样板图，全部由用户自己导入和拖框
            if (startBlank) StartBlank();
            else if (Tpl.Lines.Count > 0 && !string.IsNullOrEmpty(Tpl.SamplePath)
                && File.Exists(Tpl.SamplePath)) LoadExistingForEdit();
        }

        /// <summary>新建模板的空白初始态：无图、无线、无 ROI。</summary>
        private void StartBlank()
        {
            Tpl.Lines.Clear();
            Tpl.Rois.Clear();
            Tpl.SamplePath = "";
            _showProduct = false;
            if (_sample != null) { _sample.Dispose(); _sample = null; }
            _pic.Image = null;
            _suppressParam = true;
            _rx.Value = 0; _ry.Value = 0;
            _rw.Value = _rw.Minimum; _rh.Value = _rh.Minimum;
            _suppressParam = false;
            RefreshLines();
            _pic.Invalidate();
            SetStatus("空白模板：点【导入样板图】选一张样板图，再在图上按住鼠标拖框抓边界；"
                + "也可点【框住全图】一次性覆盖整图。");
        }

        private void HideResultGroup()
        {
            foreach (Control c in Controls)
            {
                Panel p = c as Panel;
                if (p == null) continue;
                foreach (Control g in p.Controls)
                    if (g is GroupBox && g.Text == "比对结果") g.Visible = false;
            }
        }

        /// <summary>编辑已有模板：只把样板图显示出来，保留全部线条定义。</summary>
        private void LoadExistingForEdit()
        {
            _showProduct = false;
            if (_sample != null) { _sample.Dispose(); _sample = null; }
            _sample = new Bitmap(Tpl.SamplePath);
            _pic.Image = _sample;
            RefreshLines();
            _pic.Invalidate();
            SetStatus("正在编辑已有模板。改完点【完成 · 返回主界面】；点【导入样板图】可换成新图重新定义。");
        }

        /// <summary>完成模板定义：至少要有一条绿色位置特征。</summary>
        private void FinishTemplate()
        {
            if (Tpl.CountByRole(FeatureRole.Position) == 0)
            {
                MessageBox.Show(this,
                    "请先拖框抓到边界，并把用于判断位置的线设为【绿色·判断位置】。",
                    "模板未完成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ParamChanged(object sender, EventArgs e)
        {
            if (_suppressParam) return;
            Tpl.Sensitivity = (int)_sens.Value;
            Tpl.MinArea = (int)_minArea.Value;
            Tpl.SmoothLevel = (int)_smooth.Value;
            Tpl.Rect = new Rectangle((int)_rx.Value, (int)_ry.Value, (int)_rw.Value, (int)_rh.Value);
            // 只有用户真的改了框的 X/Y/宽/高 才同步最后一个 ROI；
            // 调灵敏度/圆滑/滤噪时不能动 ROI，否则多框会被悄悄改掉。
            if ((sender == _rx || sender == _ry || sender == _rw || sender == _rh)
                && Tpl.Rois.Count > 0)
            {
                Roi last = Tpl.Rois[Tpl.Rois.Count - 1];
                if (last.IsCircle)
                {
                    int d = Math.Min(Tpl.Rect.Width, Tpl.Rect.Height);
                    last.Bounds = new Rectangle(Tpl.Rect.X + (Tpl.Rect.Width - d) / 2,
                        Tpl.Rect.Y + (Tpl.Rect.Height - d) / 2, d, d);
                }
                else last.Bounds = Tpl.Rect;
            }
            if (_sample != null && !_showProduct) ReExtract();
        }

        private void ImportSample()
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Title = "选择样板图片";
                d.Filter = "图片文件|*.bmp;*.png;*.jpg;*.jpeg|全部文件|*.*";
                if (d.ShowDialog(this) != DialogResult.OK) return;
                Tpl.SamplePath = d.FileName;
                LoadSample(-1, d.FileName);
            }
        }

        private void LoadSample(int index, string path = null)
        {
            _showProduct = false;
            Tpl.Lines.Clear();   // 换了样板图，旧的颜色定义作废
            if (_pic.Image != null) _pic.Image = null;
            if (_sample != null) { _sample.Dispose(); _sample = null; }
            if (path != null) _sample = new Bitmap(path);
            else
            {
                if (Images.Count == 0) return;
                if (index < 0) index = 0;
                if (index >= Images.Count) index = Images.Count - 1;
                DemoState.CurrentIndex = index;
                Tpl.SamplePath = Images[index];
                _sample = new Bitmap(Images[index]);
            }
            _pic.Image = _sample;
            if (_sample != null)
            {
                if (Tpl.Rect.Right > _sample.Width) Tpl.Rect = new Rectangle(Tpl.Rect.X, Tpl.Rect.Y,
                    Math.Max(20, _sample.Width - Tpl.Rect.X), Tpl.Rect.Height);
                ReExtract();
                SetStatus("样板 " + Path.GetFileName(Tpl.SamplePath) + "；" + _status.Text);
            }
        }

        private void ReExtract()
        {
            if (_sample == null) return;

            // 调参数会重新抓线：先记住已经指定过的颜色，按位置就近还原，不让用户白选一次
            List<PointF> oldCenters = new List<PointF>();
            List<FeatureLine> oldLines = new List<FeatureLine>();
            foreach (FeatureLine old in Tpl.Lines)
            {
                if (old.Role == FeatureRole.Unassigned && !old.IsFittedCircle) continue;
                oldCenters.Add(new PointF(old.Bounds.X + old.Bounds.Width / 2f,
                                          old.Bounds.Y + old.Bounds.Height / 2f));
                oldLines.Add(old);
            }

            int th, rejected;
            List<int> filtered;
            Tpl.Lines = FeatureExtractor.ExtractAll(_sample, Tpl.Rois, Tpl.Sensitivity,
                Tpl.MinArea, Tpl.SmoothLevel, out th, out rejected, out filtered);
            Tpl.LastThreshold = th;

            int restored = 0;
            foreach (FeatureLine now in Tpl.Lines)
            {
                float cx = now.Bounds.X + now.Bounds.Width / 2f;
                float cy = now.Bounds.Y + now.Bounds.Height / 2f;
                float best = 60f; int bi = -1;   // 调圆滑度后轮廓中心会漂移，放宽以免丢颜色
                for (int i = 0; i < oldCenters.Count; i++)
                {
                    float ax = oldCenters[i].X - cx, ay = oldCenters[i].Y - cy;
                    float d = (float)Math.Sqrt(ax * ax + ay * ay);
                    if (d < best) { best = d; bi = i; }
                }
                if (bi >= 0)
                {
                    now.Role = oldLines[bi].Role;
                    if (oldLines[bi].IsFittedCircle) EdgeMap.FitCircle(now);
                    oldCenters.RemoveAt(bi); oldLines.RemoveAt(bi);
                    restored++;
                }
            }

            _selected = Tpl.Lines.Count > 0 ? 0 : -1;
            RefreshLines();
            UpdateFitButton();
            _pic.Invalidate();

            string msg = "共 " + Tpl.Rois.Count + " 个框｜滤噪 " + Tpl.MinArea + "：滤掉 " + rejected
                + " 个小碎块，保留 " + Tpl.Lines.Count + " 条边界";
            filtered.Sort(delegate(int a, int b) { return b.CompareTo(a); });
            if (filtered.Count > 0)
            {
                msg += "；本次删掉 " + filtered.Count + " 条边界（面积 ";
                int show = Math.Min(4, filtered.Count);
                for (int i = 0; i < show; i++)
                    msg += (i > 0 ? "、" : "") + filtered[i];
                if (filtered.Count > show) msg += " …";
                msg += "）";
            }
            else msg += "；没有删掉任何整条边界";
            if (Tpl.Lines.Count > 0)
            {
                int smallest = int.MaxValue;
                foreach (FeatureLine l in Tpl.Lines) if (l.Area < smallest) smallest = l.Area;
                msg += "；最小剩 " + smallest + "，再删调到 " + (smallest + 1);
            }
            else msg += "；一条都不剩，把阈值调小";
            if (restored > 0) msg += "；保留 " + restored + " 条颜色";
            int circleRois = 0;
            FeatureLine cand = null;
            foreach (Roi r in Tpl.Rois) if (r.IsCircle) circleRois++;
            foreach (FeatureLine l in Tpl.Lines)
                if (l.IsRoundCandidate) { cand = l; break; }
            if (circleRois > 0)
            {
                if (cand == null)
                    msg += "｜圆形框内没抓到边界，把框放大或调小滤噪阈值";
                else
                    msg += "｜圆形框按边界判定：" + cand.RoundVerdict + " "
                        + cand.Roundness.ToString("F2", CultureInfo.InvariantCulture)
                        + "（保留真实边界，未画正圆）";
            }
            else msg += "｜改形状用【轮廓圆滑】或【拟合成正圆】，滤噪只删线";
            SetStatus(msg);
        }

        private void RefreshLines()
        {
            _lineBox.Items.Clear();
            for (int i = 0; i < Tpl.Lines.Count; i++)
            {
                FeatureLine l = Tpl.Lines[i];
                string rn = l.Role == FeatureRole.Position ? "绿色" :
                            l.Role == FeatureRole.Attached ? "黄色" :
                            l.Role == FeatureRole.Ignored ? "紫色" : "未定义";
                string tag = l.IsFittedCircle ? " ○正圆"
                    : l.IsRoundCandidate ? " " + l.RoundVerdict + "·"
                        + l.Roundness.ToString("F2", CultureInfo.InvariantCulture)
                    : "";
                _lineBox.Items.Add("#" + (i + 1) + " " + rn + tag + "  面积 " + l.Area);
            }
            if (_selected >= 0 && _selected < _lineBox.Items.Count) _lineBox.SelectedIndex = _selected;
        }

        private void ToggleFitCircle()
        {
            if (_selected < 0 || _selected >= Tpl.Lines.Count)
            { SetStatus("请先在左图上点选一条边界线，再点【拟合成正圆】。"); return; }
            FeatureLine l = Tpl.Lines[_selected];
            if (l.IsFittedCircle)
            {
                EdgeMap.RestoreCircle(l);
                SetStatus("#" + (_selected + 1) + " 已还原成真实轮廓。");
            }
            else if (EdgeMap.FitCircle(l))
            {
                SetStatus("#" + (_selected + 1) + " 已拟合成正圆，半径 "
                    + l.CircleRadius.ToString("F1", CultureInfo.InvariantCulture)
                    + " 像素；再点一次按钮可还原。");
            }
            else
            {
                SetStatus("#" + (_selected + 1) + " 点太少或形状太散，不适合拟合成圆。");
                return;
            }
            RefreshLines();
            UpdateFitButton();
            _pic.Invalidate();
        }

        private void UpdateFitButton()
        {
            if (_btnFit == null) return;
            bool fitted = _selected >= 0 && _selected < Tpl.Lines.Count
                && Tpl.Lines[_selected].IsFittedCircle;
            _btnFit.Text = fitted ? "还原成真实轮廓" : "把选中线拟合成正圆";
        }

        /// <summary>选中线时把「该按哪个按钮」直接写在状态栏，避免用户去调滤噪阈值。</summary>
        private void ShowSelectionHint()
        {
            if (_selected < 0 || _selected >= Tpl.Lines.Count) return;
            FeatureLine l = Tpl.Lines[_selected];
            double c = EdgeMap.Circularity(l);
            string s = "已选中 #" + (_selected + 1) + "（" + l.RoleName + "）";
            if (l.IsFittedCircle)
                s += "，已是正圆；点【还原成真实轮廓】可退回。";
            else if (l.IsRoundCandidate)
                s += "，圆形框按真实边界判定为「" + l.RoundVerdict + "」（圆度 "
                    + l.Roundness.ToString("F2", CultureInfo.InvariantCulture)
                    + "）。已保留原始边界；想要数学正圆再点【把选中线拟合成正圆】。";
            else if (c > 0.55)
                s += "，接近圆形（" + c.ToString("F2", CultureInfo.InvariantCulture)
                    + "）：点【把选中线拟合成正圆】变标准圆，调滤噪阈值没用。";
            else
                s += "；想让线更顺调【轮廓圆滑】，点右侧按钮改用途。";
            SetStatus(s);
        }

        private void SetRole(FeatureRole role)
        {
            if (_selected < 0 || _selected >= Tpl.Lines.Count)
            { SetStatus("请先在左图上点选一条边界线。"); return; }
            Tpl.Lines[_selected].Role = role;
            RefreshLines(); _pic.Invalidate();
            SetStatus("#" + (_selected + 1) + " 已设为「" + Tpl.Lines[_selected].RoleName + "」。");
        }

        private string MatchModeName
        {
            get { return _matchMode.SelectedIndex == 1 ? "几何形状比对" : "模板匹配"; }
        }

        /// <summary>按当前比对方式出结果；两种方式都产同一个 MatchResult，界面统一消费。</summary>
        private MatchResult RunMatch(Bitmap product, List<FeatureLine> productLines)
        {
            double thr = (double)_batchScore.Value;
            MatchResult r = _matchMode.SelectedIndex == 1
                ? GeometryMatcher.Compare(Tpl, productLines, thr)
                : Matcher.Match(product, Tpl, thr);
            // 两种模式统一用「形状一对一」判定必要特征
            GeometryMatcher.ComputeAttached(Tpl, productLines, r);
            MarkPositionLines(r, productLines);
            return r;
        }

        /// <summary>
        /// 把产品图上「形状最像样板绿线」的几条标成位置特征，供产品视图红色高亮。
        /// 标的是产品图自己抓到的线，不是叠上去的样板模板；未找到时不高亮。
        /// </summary>
        private void MarkPositionLines(MatchResult r, List<FeatureLine> productLines)
        {
            r.PositionIndices.Clear();
            if (!r.Found || productLines == null || productLines.Count == 0) return;
            foreach (FeatureLine g in Tpl.Lines)
            {
                if (g.Role != FeatureRole.Position) continue;
                List<KeyValuePair<int, double>> cand = new List<KeyValuePair<int, double>>();
                for (int i = 0; i < productLines.Count; i++)
                {
                    double s = GeometryMatcher.ShapeSimilarity(g, productLines[i]);
                    if (s >= 0.70) cand.Add(new KeyValuePair<int, double>(i, s));
                }
                cand.Sort(delegate(KeyValuePair<int, double> a, KeyValuePair<int, double> b)
                { return b.Value.CompareTo(a.Value); });
                // 两条绿斜边形状几乎一样，都指向同一条产品线时，让后一条取次佳候选
                foreach (KeyValuePair<int, double> kv in cand)
                {
                    if (r.PositionIndices.Contains(kv.Key)) continue;
                    r.PositionIndices.Add(kv.Key);
                    break;
                }
            }
        }

        /// <summary>释放产品图前先摘掉 PictureBox 的引用，避免控件拿着已释放的 Bitmap 报错。</summary>
        private void ReleaseProduct()
        {
            Bitmap old = _product;
            _product = null;
            if (_pic.Image == old) _pic.Image = null;
            if (old != null) old.Dispose();
        }

        private void LoadProductAndMatch()
        {
            if (Tpl.Lines.Count == 0) { SetStatus("请先在样板图上拖框抓到边界线。"); return; }
            if (Tpl.CountByRole(FeatureRole.Position) == 0)
            { SetStatus("请先把用于判断位置的边界设为【绿色·判断位置】。"); return; }
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Title = "选择产品照片（与样板比对）";
                d.Filter = "图片文件|*.bmp;*.png;*.jpg;*.jpeg|全部文件|*.*";
                string next = "";
                if (Images.Count > 1)
                    next = Images[(DemoState.CurrentIndex + 1) % Images.Count];
                if (File.Exists(next)) d.FileName = next;
                if (d.ShowDialog(this) != DialogResult.OK) return;
                Tpl.ProductPath = d.FileName;
                ReleaseProduct();
                _product = new Bitmap(d.FileName);
            }
            Cursor old = Cursor; Cursor = Cursors.WaitCursor;
            try
            {
                // 产品图先用同一组参数独立抓一次边界，再按当前方式比对
                int th, rej; List<int> flt;
                _productLines = FeatureExtractor.ExtractAll(_product, Tpl.Rois,
                    Tpl.Sensitivity, Tpl.MinArea, Tpl.SmoothLevel,
                    out th, out rej, out flt);
                _last = RunMatch(_product, _productLines);
            }
            finally { Cursor = old; }
            _showProduct = true;
            ShowImageMode();
            _pic.Image = _product;
            _pic.Invalidate();
            ShowMatchResult(_last);
            if (_last.Found)
                SetStatus("【" + MatchModeName + "】找到匹配：像素偏移 (" + _last.PixelDx + ", " + _last.PixelDy
                    + ")，角度 " + _last.Angle.ToString("F2", CultureInfo.InvariantCulture)
                    + "°。绿框=识别区域，红色高亮=找到的位置特征，青色=本图其他特征；不叠加样板模板。");
            else
                SetStatus("【" + MatchModeName + "】未找到匹配：相似度 " + _last.Score.ToString("F1", CultureInfo.InvariantCulture)
                    + " 分，低于下限 " + _last.Threshold.ToString("F0", CultureInfo.InvariantCulture)
                    + "。绿框=识别区域，青色=本图抓到的特征；未找到位置特征，故无红色高亮。");
        }

        private void BackToSample()
        {
            if (_sample == null) return;
            _showProduct = false;
            _productLines.Clear();
            ShowImageMode();
            _pic.Image = _sample;
            _pic.Invalidate();
            SetStatus("已返回样板图，可继续点线改用途或重新拖框。");
        }

        private void ClearRois()
        {
            Tpl.Rois.Clear();
            Tpl.Lines.Clear();
            _selected = -1;
            RefreshLines();
            UpdateFitButton();
            _pic.Invalidate();
            SetStatus("已清空所有框选。可连续拖多个框累加特征；切到「圆形框选」会优先判圆。");
        }

        /// <summary>一键框住整张图，抓出框外那些特征（上方小圆圈、左右下方形状等）。</summary>
        private void SelectWholeImage()
        {
            if (_sample == null) return;
            Tpl.Rois.Clear();
            Roi r = new Roi();
            r.IsCircle = false;
            r.Bounds = new Rectangle(0, 0, _sample.Width, _sample.Height);
            Tpl.Rois.Add(r);
            _suppressParam = true;
            _rx.Value = 0; _ry.Value = 0;
            _rw.Value = _sample.Width; _rh.Value = _sample.Height;
            _suppressParam = false;
            Tpl.Rect = r.Bounds;
            _showProduct = false;
            ShowImageMode();
            if (_pic.Image != _sample) _pic.Image = _sample;
            ReExtract();
            SetStatus("已框住整张图（" + _sample.Width + "×" + _sample.Height
                + "），原先在框外的特征现在也会抓到；可继续拖小框做多次框选。");
        }

        // ---- 批量识别 ----

        private void BatchImportAndMatch()
        {
            if (Tpl.Lines.Count == 0) { SetStatus("请先在样板图上拖框抓到边界线。"); return; }
            if (Tpl.CountByRole(FeatureRole.Position) == 0)
            { SetStatus("请先把用于判断位置的边界设为【绿色·判断位置】。"); return; }
            string[] files;
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Title = "一次选择多张产品图片";
                d.Multiselect = true;
                d.Filter = "图片文件|*.bmp;*.png;*.jpg;*.jpeg|全部文件|*.*";
                if (d.ShowDialog(this) != DialogResult.OK) return;
                files = d.FileNames;
            }
            RunBatch(files);
        }

        /// <summary>批量识别核心，与文件对话框解耦，便于一次跑多张做验证。</summary>
        internal void RunBatch(string[] files)
        {
            if (files == null || files.Length == 0) return;
            bool hasRequired = Tpl.CountByRole(FeatureRole.Attached) > 0;
            _batch.Clear();
            Cursor old = Cursor; Cursor = Cursors.WaitCursor;
            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    SetStatus("正在识别 " + (i + 1) + " / " + files.Length
                        + "：" + Path.GetFileName(files[i]));
                    Application.DoEvents();
                    BatchItem it = new BatchItem();
                    it.Path = files[i];
                    it.Name = Path.GetFileName(files[i]);
                    it.HasRequired = hasRequired;
                    try
                    {
                        using (Bitmap b = new Bitmap(files[i]))
                        {
                            int th, rej; List<int> flt;
                            List<FeatureLine> pl = FeatureExtractor.ExtractAll(b, Tpl.Rois,
                                Tpl.Sensitivity, Tpl.MinArea, Tpl.SmoothLevel,
                                out th, out rej, out flt);
                            it.Result = RunMatch(b, pl);
                        }
                    }
                    catch { it.Result = new MatchResult(); }   // 坏图不打断整批
                    _batch.Add(it);
                }
            }
            finally { Cursor = old; }
            ShowBatchMode();
            Rejudge();
            SetStatus("【" + MatchModeName + "】批量完成：共 " + _batch.Count
                + " 张。双击某一行可看那张的叠加图，点【导出 CSV】保存结果。");
        }

        private void Rejudge()
        {
            decimal minScore = _batchScore.Value;
            decimal tolXY = _batchTolXY.Value;
            decimal tolAng = _batchTolAng.Value;
            int ok = 0;
            foreach (BatchItem it in _batch)
            {
                MatchResult m = it.Result;
                it.Ok = (decimal)m.Score >= minScore
                    && Math.Abs((decimal)m.DetX) <= tolXY
                    && Math.Abs((decimal)m.DetY) <= tolXY
                    && Math.Abs((decimal)m.Angle) <= tolAng
                    && (!it.HasRequired || m.AttachedFound);
                if (it.Ok) ok++;
            }
            RefreshGrid();
            _batchSummary.Text = "共 " + _batch.Count + " 张 ｜ 合格 " + ok
                + " ｜ 不合格 " + (_batch.Count - ok);
        }

        private void RefreshGrid()
        {
            _grid.Rows.Clear();
            for (int i = 0; i < _batch.Count; i++)
            {
                BatchItem it = _batch[i];
                MatchResult m = it.Result;
                int row = _grid.Rows.Add(
                    i + 1,
                    it.Name,
                    m.DetX.ToString("F6", CultureInfo.InvariantCulture),
                    m.DetY.ToString("F6", CultureInfo.InvariantCulture),
                    m.Angle.ToString("F2", CultureInfo.InvariantCulture),
                    m.Score.ToString("F2", CultureInfo.InvariantCulture),
                    it.HasRequired ? (m.AttachedFound ? "存在" : "缺失") : "未设置",
                    it.Ok ? "合格" : "不合格");
                DataGridViewCell c = _grid.Rows[row].Cells["Verdict"];
                c.Style.BackColor = it.Ok ? Color.FromArgb(215, 245, 215) : Color.FromArgb(255, 215, 215);
                c.Style.ForeColor = it.Ok ? Color.DarkGreen : Color.DarkRed;
                c.Style.SelectionBackColor = c.Style.BackColor;
                c.Style.SelectionForeColor = c.Style.ForeColor;
            }
        }

        internal string BuildCsv()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("序号,文件名,ΔX(mm),ΔY(mm),角度(°),相似度,必要特征,判定,完整路径");
            for (int i = 0; i < _batch.Count; i++)
            {
                BatchItem it = _batch[i];
                MatchResult m = it.Result;
                sb.Append(i + 1).Append(',').Append(Csv(it.Name)).Append(',')
                  .Append(m.DetX.ToString("F6", CultureInfo.InvariantCulture)).Append(',')
                  .Append(m.DetY.ToString("F6", CultureInfo.InvariantCulture)).Append(',')
                  .Append(m.Angle.ToString("F2", CultureInfo.InvariantCulture)).Append(',')
                  .Append(m.Score.ToString("F2", CultureInfo.InvariantCulture)).Append(',')
                  .Append(it.HasRequired ? (m.AttachedFound ? "存在" : "缺失") : "未设置").Append(',')
                  .Append(it.Ok ? "合格" : "不合格").Append(',')
                  .Append(Csv(it.Path)).AppendLine();
            }
            return sb.ToString();
        }

        private void ExportCsv()
        {
            if (_batch.Count == 0) { SetStatus("还没有批量识别结果可导出。"); return; }
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Title = "导出批量识别结果";
                d.Filter = "CSV 文件|*.csv";
                d.FileName = "批量识别结果.csv";
                if (d.ShowDialog(this) != DialogResult.OK) return;
                File.WriteAllText(d.FileName, BuildCsv(), new UTF8Encoding(true));
                SetStatus("已导出 " + _batch.Count + " 条结果到 " + d.FileName);
            }
        }

        private static string Csv(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            if (s.IndexOf(',') >= 0 || s.IndexOf('"') >= 0 || s.IndexOf('\n') >= 0)
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private void ShowBatchItem(int idx)
        {
            BatchItem it = _batch[idx];
            try
            {
                ReleaseProduct();
                _product = new Bitmap(it.Path);
            }
            catch { SetStatus("这张图打不开：" + it.Name); return; }
            _last = it.Result;   // 与批量表格的判定保持一致
            int th, rej; List<int> flt;
            _productLines = FeatureExtractor.ExtractAll(_product, Tpl.Rois,
                Tpl.Sensitivity, Tpl.MinArea, Tpl.SmoothLevel,
                out th, out rej, out flt);
            MarkPositionLines(_last, _productLines);   // 索引按这次实际抓到的线重新对一遍
            _showProduct = true;
            ShowImageMode();
            _pic.Image = _product;
            _pic.Invalidate();
            ShowMatchResult(_last);
            SetStatus("正在查看第 " + (idx + 1) + " 张：" + it.Name
                + "（" + (it.Ok ? "合格" : "不合格") + "）。点【返回样板图】可回样板。");
        }

        private void ShowMatchResult(MatchResult m)
        {
            _lblMode.Text = "比对方式：" + (_matchMode.SelectedIndex == 1
                ? "几何·两边各抓再比形状" : "模板·样板线找命中");
            _lblMode.ForeColor = Color.DarkSlateGray;
            _lblFound.Text = "匹配判定：" + (m.Found ? "✓ 找到" : "✗ 未找到");
            _lblFound.ForeColor = m.Found ? Color.Green : Color.Red;
            _lblX.Text = "ΔX（偏移）：" + m.DetX.ToString("F6", CultureInfo.InvariantCulture);
            _lblY.Text = "ΔY（偏移）：" + m.DetY.ToString("F6", CultureInfo.InvariantCulture);
            _lblAngle.Text = "角度偏移：" + (m.Angle >= 0 ? "+" : "")
                + m.Angle.ToString("F2", CultureInfo.InvariantCulture) + "°";
            _lblScore.Text = "相似度：" + m.Score.ToString("F2", CultureInfo.InvariantCulture) + " 分";
            if (Tpl.CountByRole(FeatureRole.Attached) == 0)
            {
                _lblRequired.Text = "必要特征：未设置";
                _lblRequired.ForeColor = Color.DarkSlateGray;
            }
            else
            {
                _lblRequired.Text = "必要特征：" + (m.AttachedFound ? "存在" : "缺失")
                    + "（匹配 " + m.AttachedScore.ToString("F0", CultureInfo.InvariantCulture) + "%）";
                _lblRequired.ForeColor = m.AttachedFound ? Color.Green : Color.Red;
            }
        }

        private void ShowBatchMode()
        {
            _pic.Visible = false;
            _batchPanel.Visible = true;
            _batchPanel.BringToFront();
        }

        private void ShowImageMode()
        {
            _batchPanel.Visible = false;
            _pic.Visible = true;
            _pic.BringToFront();
        }

        // ---- 绘制 ----
        private void GetZoom(out float scale, out float ox, out float oy)
        {
            Bitmap im = _showProduct ? _product : _sample;
            if (im == null) { scale = 1f; ox = 0f; oy = 0f; return; }
            scale = Math.Min((float)_pic.ClientSize.Width / im.Width,
                             (float)_pic.ClientSize.Height / im.Height);
            ox = (_pic.ClientSize.Width - im.Width * scale) / 2f;
            oy = (_pic.ClientSize.Height - im.Height * scale) / 2f;
        }

        private void OnPaintImage(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float scale, ox, oy;
            GetZoom(out scale, out ox, out oy);

            // 空白模板：画引导框，明确告诉用户先导入样板图，不预置任何图或 ROI
            if (!_showProduct && _sample == null)
            {
                int w = _pic.ClientSize.Width, h = _pic.ClientSize.Height;
                int bw = Math.Min(520, w - 80), bh = 190;
                Rectangle rc = new Rectangle((w - bw) / 2, (h - bh) / 2, bw, bh);
                using (Pen dash = new Pen(Color.FromArgb(90, 90, 140, 175), 2f))
                {
                    dash.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(dash, rc);
                }
                using (Font f1 = new Font("Microsoft YaHei UI", 20f, FontStyle.Bold))
                using (Font f2 = new Font("Microsoft YaHei UI", 11f))
                {
                    string t = "空白模板 · 请先导入样板图";
                    SizeF a = g.MeasureString(t, f1);
                    g.DrawString(t, f1, Brushes.White, (w - a.Width) / 2, rc.Y + 32);
                    string s1 = "点【导入样板图】选择一张样板图";
                    SizeF b = g.MeasureString(s1, f2);
                    g.DrawString(s1, f2, Brushes.LightSteelBlue, (w - b.Width) / 2, rc.Y + 88);
                    string s2 = "然后在图上按住鼠标拖框，程序只认真实边界";
                    SizeF c = g.MeasureString(s2, f2);
                    g.DrawString(s2, f2, Brushes.LightSteelBlue, (w - c.Width) / 2, rc.Y + 116);
                }
                return;
            }

            int dx = _showProduct ? _last.PixelDx : 0;
            int dy = _showProduct ? _last.PixelDy : 0;
            float ang = _showProduct ? (float)_last.Angle : 0f;
            PointF ctr = Tpl.PositionCenter;   // 与 Matcher 的旋转中心保持一致
            bool found = !_showProduct || _last.Found;
            // 识别区域：矩形绿实线、圆形橙虚线。产品视图里这就是「本图的识别区域」。
            foreach (Roi roi in Tpl.Rois)
            {
                Color c = _dragging ? Color.FromArgb(90, 120, 120, 120)
                    : (roi.IsCircle ? Color.Orange : Color.Lime);
                float bx = ox + roi.Bounds.X * scale, by = oy + roi.Bounds.Y * scale;
                float bw = roi.Bounds.Width * scale, bh = roi.Bounds.Height * scale;
                using (Pen pen = new Pen(c, roi.IsCircle ? 1.6f : 2f))
                {
                    if (roi.IsCircle) pen.DashStyle = DashStyle.Dash;
                    if (roi.IsCircle) g.DrawEllipse(pen, bx, by, bw, bh);
                    else g.DrawRectangle(pen, bx, by, bw, bh);
                }
            }

            if (_showProduct)
            {
                // 产品视图只显示「这张图在识别区域内按参数抓到的特征」；
                // 被判定为位置特征的那几条用红色高亮（发光描边 + 粗红线），其余青色。
                HashSet<int> pos = new HashSet<int>(_last.PositionIndices);
                using (Pen glow = new Pen(Color.FromArgb(95, 255, 40, 40), 8f))
                    for (int i = 0; i < _productLines.Count; i++)
                        if (pos.Contains(i))
                            Stroke(g, _productLines[i], glow, ox, oy, scale, 0, 0, 0f, ctr);
                using (Pen shadow = new Pen(Color.FromArgb(150, 0, 30, 60), 3.2f))
                    for (int i = 0; i < _productLines.Count; i++)
                        if (!pos.Contains(i))
                            Stroke(g, _productLines[i], shadow, ox, oy, scale, 0, 0, 0f, ctr);
                using (Pen cyan = new Pen(Color.Cyan, 1.7f))
                    for (int i = 0; i < _productLines.Count; i++)
                        if (!pos.Contains(i))
                            Stroke(g, _productLines[i], cyan, ox, oy, scale, 0, 0, 0f, ctr);
                using (Pen red = new Pen(Color.Red, 2.6f))
                    for (int i = 0; i < _productLines.Count; i++)
                        if (pos.Contains(i))
                            Stroke(g, _productLines[i], red, ox, oy, scale, 0, 0, 0f, ctr);
                if (!found) DrawNoMatch(g);
            }
            else
            {
                // 样板视图：显示定义好颜色（绿/黄/紫/青）的边界
                using (Pen shadow = new Pen(Color.FromArgb(150, 0, 45, 0), 2.4f))
                    foreach (FeatureLine l in Tpl.Lines)
                        Stroke(g, l, shadow, ox, oy, scale, dx, dy, ang, ctr);
                for (int i = 0; i < Tpl.Lines.Count; i++)
                {
                    FeatureLine l = Tpl.Lines[i];
                    Color c;
                    if (l.Role == FeatureRole.Position) c = Color.Lime;
                    else if (l.Role == FeatureRole.Attached) c = Color.Yellow;
                    else if (l.Role == FeatureRole.Ignored) c = Color.FromArgb(205, 90, 245);
                    else c = Color.Cyan;
                    using (Pen pen = new Pen(c, i == _selected ? 2.6f : 1.3f))
                        Stroke(g, l, pen, ox, oy, scale, dx, dy, ang, ctr);
                }
            }

            // 拖框过程中实时画出正在拉的框，松开前就能看到范围
            if (_dragging)
            {
                int x = Math.Min(_a.X, _b.X), y = Math.Min(_a.Y, _b.Y);
                int w = Math.Abs(_a.X - _b.X), h = Math.Abs(_a.Y - _b.Y);
                bool circle = _roiShape.SelectedIndex == 1;
                if (circle)
                {
                    int d = Math.Min(w, h);
                    x += (w - d) / 2; y += (h - d) / 2;
                    w = d; h = d;
                }
                float rx = ox + x * scale, ry = oy + y * scale;
                float rw = w * scale, rh = h * scale;
                using (Brush fill = new SolidBrush(Color.FromArgb(28, 255, 165, 0)))
                {
                    if (circle) g.FillEllipse(fill, rx, ry, rw, rh);
                    else g.FillRectangle(fill, rx, ry, rw, rh);
                }
                using (Pen dash = new Pen(Color.Orange, 2f))
                {
                    dash.DashStyle = DashStyle.Dash;
                    if (circle) g.DrawEllipse(dash, rx, ry, rw, rh);
                    else g.DrawRectangle(dash, rx, ry, rw, rh);
                }
                using (Font ft = new Font("Microsoft YaHei UI", 9f))
                {
                    string tip = circle ? "圆形 Ø" + w : w + " × " + h;
                    SizeF ts = g.MeasureString(tip, ft);
                    float tx = rx + 4, ty = Math.Max(2f, ry - ts.Height - 4);
                    using (Brush bg = new SolidBrush(Color.FromArgb(190, 40, 40, 40)))
                        g.FillRectangle(bg, tx - 2, ty - 1, ts.Width + 4, ts.Height + 2);
                    using (Brush tb = new SolidBrush(Color.Orange))
                        g.DrawString(tip, ft, tb, tx, ty);
                }
            }
        }

        /// <summary>没匹配上时给出醒目的失败提示，并说明各颜色含义，避免误以为抓到了特征。</summary>
        private void DrawNoMatch(Graphics g)
        {
            string title = "未找到匹配";
            string sub = "相似度 " + _last.Score.ToString("F1", CultureInfo.InvariantCulture)
                + " 分  ＜  下限 " + _last.Threshold.ToString("F0", CultureInfo.InvariantCulture) + " 分";
            if (Tpl.CountByRole(FeatureRole.Attached) > 0)
                sub += "，必要特征" + (_last.AttachedFound ? "存在" : "缺失");
            sub += "；绿框 = 识别区域，青色 = 本图特征，无红色位置特征";
            using (Font big = new Font("Microsoft YaHei UI", 20f, FontStyle.Bold))
            using (Font small = new Font("Microsoft YaHei UI", 9.5f))
            using (Brush bg = new SolidBrush(Color.FromArgb(210, 28, 28, 32)))
            {
                SizeF s1 = g.MeasureString(title, big);
                SizeF s2 = g.MeasureString(sub, small);
                float bw = Math.Max(s1.Width, s2.Width) + 36;
                float bh = s1.Height + s2.Height + 26;
                float x = (_pic.ClientSize.Width - bw) / 2f;
                float y = 24f;
                g.FillRectangle(bg, x, y, bw, bh);
                using (Pen edge = new Pen(Color.FromArgb(255, 120, 120), 1.5f))
                    g.DrawRectangle(edge, x, y, bw, bh);
                g.DrawString(title, big, Brushes.OrangeRed, x + (bw - s1.Width) / 2f, y + 8);
                g.DrawString(sub, small, Brushes.White, x + (bw - s2.Width) / 2f, y + 12 + s1.Height);
            }
        }

        private static void Stroke(Graphics g, FeatureLine l, Pen pen, float ox, float oy,
            float scale, int dx, int dy, float angleDeg, PointF ctr)
        {
            // 轮廓跟踪得到的一条连续折线：先绕质心按检测角度旋转，再平移到产品位置
            if (l.Points.Count < 2) return;
            double a = angleDeg * Math.PI / 180.0;
            float cs = (float)Math.Cos(a), sn = (float)Math.Sin(a);
            PointF[] pts = new PointF[l.Points.Count];
            for (int i = 0; i < pts.Length; i++)
            {
                PointF p = l.Points[i];
                float rx = p.X - ctr.X, ry = p.Y - ctr.Y;
                float x = ctr.X + rx * cs - ry * sn + dx;
                float y = ctr.Y + rx * sn + ry * cs + dy;
                pts[i] = new PointF(ox + x * scale, oy + y * scale);
            }
            g.DrawLines(pen, pts);
        }

        // ---- 拖框 ----
        // 按下时先不决定是「点选线」还是「拖框」，移动超过 6px 才进入拖框，
        // 这样即使起点正好落在边界线上，也能正常拉出新框。
        private bool _pending, _dragging, _suppressParam;
        private Point _downScreen, _a, _b;

        private Point ToImage(Point p)
        {
            float scale, ox, oy;
            GetZoom(out scale, out ox, out oy);
            return new Point((int)Math.Round((p.X - ox) / scale),
                             (int)Math.Round((p.Y - oy) / scale));
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_sample == null || _showProduct) return;
            _pending = true;
            _dragging = false;
            _downScreen = e.Location;
            _a = ToImage(e.Location); _b = _a;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_pending && !_dragging)
            {
                int mx = e.X - _downScreen.X, my = e.Y - _downScreen.Y;
                if (mx * mx + my * my < 36) return;
                _dragging = true;
            }
            if (!_dragging) return;
            _b = ToImage(e.Location);
            _pic.Invalidate();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!_pending && !_dragging) return;
            bool wasDragging = _dragging;
            _pending = false; _dragging = false;
            _b = ToImage(e.Location);

            if (wasDragging)
            {
                int x = Math.Min(_a.X, _b.X), y = Math.Min(_a.Y, _b.Y);
                int w = Math.Abs(_a.X - _b.X), h = Math.Abs(_a.Y - _b.Y);
                if (_sample != null)
                {
                    x = Math.Max(0, Math.Min(x, _sample.Width - 1));
                    y = Math.Max(0, Math.Min(y, _sample.Height - 1));
                    w = Math.Min(w, _sample.Width - x);
                    h = Math.Min(h, _sample.Height - y);
                }
                _pic.Invalidate();
                if (w < 20 || h < 20) return;
                bool circle = _roiShape.SelectedIndex == 1;
                Rectangle rb;
                if (circle)
                {
                    int d = Math.Min(w, h);
                    rb = new Rectangle(x + (w - d) / 2, y + (h - d) / 2, d, d);
                }
                else rb = new Rectangle(x, y, w, h);

                Roi roi = new Roi();
                roi.IsCircle = circle;
                roi.Bounds = rb;
                Tpl.Rois.Add(roi);   // 追加：多次框选的特征累加，不覆盖之前的

                _suppressParam = true;   // 四个数值框会各自触发 ParamChanged，先压住
                _rx.Value = rb.X; _ry.Value = rb.Y;
                _rw.Value = rb.Width; _rh.Value = rb.Height;
                _suppressParam = false;
                Tpl.Rect = rb;
                ReExtract();
                return;
            }

            // 没有拖动 → 判定为点选某条边界线
            Point q = _a;
            float scale, ox, oy;
            GetZoom(out scale, out ox, out oy);
            int best = -1;
            double bestD = 12.0 / Math.Max(0.01, scale);
            for (int k = 0; k < Tpl.Lines.Count; k++)
                foreach (PointF p in Tpl.Lines[k].Points)
                {
                    double d = Math.Sqrt((p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y));
                    if (d < bestD) { bestD = d; best = k; }
                }
            if (best >= 0)
            {
                _selected = best;
                if (_lineBox.SelectedIndex != best) _lineBox.SelectedIndex = best;
            }
            else _selected = -1;
            UpdateFitButton();
            ShowSelectionHint();
            _pic.Invalidate();
        }

        private void SetStatus(string s) { if (_status != null) _status.Text = s; }
    }
}

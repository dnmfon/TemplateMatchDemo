using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TemplateMatchDemo
{
    /// <summary>标定结果持久化（exe 目录下 calibration.txt）。</summary>
    public static class CalibrationStore
    {
        private static string PathOf()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "calibration.txt");
        }

        public static void Load(TemplateData tpl)
        {
            try
            {
                string p = PathOf();
                if (!File.Exists(p)) return;
                string[] parts = File.ReadAllText(p).Trim().Split('|');
                if (parts.Length < 2) return;
                double x = double.Parse(parts[0], CultureInfo.InvariantCulture);
                double y = double.Parse(parts[1], CultureInfo.InvariantCulture);
                if (x > 0 && y > 0) { tpl.ScaleX = x; tpl.ScaleY = y; }
                // 扩展段：X下限|X上限|Y下限|Y上限|角度下限|角度上限（旧文件没有这段，保持默认）
                if (parts.Length >= 8)
                {
                    tpl.TolXMin = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    tpl.TolXMax = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    tpl.TolYMin = double.Parse(parts[4], CultureInfo.InvariantCulture);
                    tpl.TolYMax = double.Parse(parts[5], CultureInfo.InvariantCulture);
                    tpl.TolAngMin = double.Parse(parts[6], CultureInfo.InvariantCulture);
                    tpl.TolAngMax = double.Parse(parts[7], CultureInfo.InvariantCulture);
                }
            }
            catch { }
        }

        public static void Save(double x, double y)
        {
            try
            {
                File.WriteAllText(PathOf(),
                    x.ToString("R", CultureInfo.InvariantCulture) + "|"
                    + y.ToString("R", CultureInfo.InvariantCulture));
            }
            catch { }
        }

        /// <summary>保存标定 + 判定容差（保留已有的标定值）。</summary>
        public static void SaveAll(TemplateData tpl)
        {
            try
            {
                File.WriteAllText(PathOf(),
                    tpl.ScaleX.ToString("R", CultureInfo.InvariantCulture) + "|"
                    + tpl.ScaleY.ToString("R", CultureInfo.InvariantCulture) + "|"
                    + tpl.TolXMin.ToString("R", CultureInfo.InvariantCulture) + "|"
                    + tpl.TolXMax.ToString("R", CultureInfo.InvariantCulture) + "|"
                    + tpl.TolYMin.ToString("R", CultureInfo.InvariantCulture) + "|"
                    + tpl.TolYMax.ToString("R", CultureInfo.InvariantCulture) + "|"
                    + tpl.TolAngMin.ToString("R", CultureInfo.InvariantCulture) + "|"
                    + tpl.TolAngMax.ToString("R", CultureInfo.InvariantCulture));
            }
            catch { }
        }
    }

    /// <summary>
    /// 像素 ↔ 物理尺寸标定窗口：
    /// 载入标定件 → 在图上量取已知尺寸的区域 → 计算 mm/px → 应用到 ScaleX / ScaleY。
    /// </summary>
    public sealed partial class CalibrationForm : Form
    {
        // 6 款矩形标定板：每格像素尺寸与对应实际毫米数
        private static readonly string[] BoardNames = new string[]
        {
            "标准棋盘 50px / 10mm",
            "细密棋盘 25px / 5mm",
            "粗棋盘 100px / 20mm",
            "长方格 80x50px / 16x10mm",
            "单矩形块 200x150px / 40x30mm",
            "噪声棋盘 50px / 10mm"
        };
        private static readonly int[] BoardCellW = { 50, 25, 100, 80, 200, 50 };
        private static readonly int[] BoardCellH = { 50, 25, 100, 50, 150, 50 };
        private static readonly double[] BoardMmW = { 10, 5, 20, 16, 40, 10 };
        private static readonly double[] BoardMmH = { 10, 5, 20, 10, 30, 10 };

        private PictureBox _pic;
        private Bitmap _image;
        private ComboBox _mode, _boardKind;
        private Label _pxMeasure, _resX, _resY, _resDemo, _status, _curLabel;
        private NumericUpDown _mmW, _mmH, _mmLen, _manualX, _manualY;
        private Panel _grpRect, _grpRuler, _grpManual;
        private Label _labW, _labH, _autoInfo, _measureTip;
        private Button _btnAuto;
        private readonly List<int> _gridX = new List<int>();
        private readonly List<int> _gridY = new List<int>();
        private readonly List<Rectangle> _rects = new List<Rectangle>();
        private readonly List<bool> _rectOk = new List<bool>();
        private readonly List<Rectangle> _manualRects = new List<Rectangle>();
        private bool _autoOk, _multiOk;
        private int _rectUsed;
        private int _manualW, _manualH, _manualUsed;
        private int _cellW, _cellH;
        private bool _dragging, _hasMeasure;
        private Point _dragA, _dragB;      // client 坐标（拖拽预览）
        private Point _mA, _mB;            // 图像坐标（已完成的测量）
        private double _scaleX, _scaleY;
        private readonly double _origX, _origY;

        public double ResultScaleX { get { return _scaleX; } }
        public double ResultScaleY { get { return _scaleY; } }

        /// <summary>无参构造函数：仅供 VS 窗体设计器实例化并显示控件，不执行业务逻辑。</summary>
        public CalibrationForm()
        {
            InitializeComponent();
        }

        public CalibrationForm(double curX, double curY)
        {
            InitializeComponent();
            _origX = _scaleX = curX;
            _origY = _scaleY = curY;
            // 手动输入模式的初值跟随当前标定系数
            decimal cx = (decimal)curX, cy = (decimal)curY;
            if (cx < _manualX.Minimum) cx = _manualX.Minimum;
            if (cx > _manualX.Maximum) cx = _manualX.Maximum;
            if (cy < _manualY.Minimum) cy = _manualY.Minimum;
            if (cy > _manualY.Maximum) cy = _manualY.Maximum;
            _manualX.Value = cx;
            _manualY.Value = cy;
            SwitchMode();   // 初始化各模式面板的显示隐藏，避免三组控件叠在一起
            SetStatus("标定模块：载入标定件图片，或直接生成示例标定板。");
        }

        private void SwitchMode()
        {
            int m = _mode.SelectedIndex;
            bool auto = m == 4 || m == 5;
            _grpRect.Visible = m == 0 || auto;
            _grpRuler.Visible = m == 1 || m == 2;
            _grpManual.Visible = m == 3;
            _btnAuto.Visible = auto;
            _btnAuto.Text = m == 5 ? "自动抓取多矩形" : "自动抓取矩形";
            _labW.Text = m == 5 ? "每矩形实际宽 (mm)"
                : auto ? "每格实际宽 (mm)" : "实际宽度 (mm)";
            _labH.Text = m == 5 ? "每矩形实际高 (mm)"
                : auto ? "每格实际高 (mm)" : "实际高度 (mm)";
            _measureTip.Text = m == 5 ? "点【自动抓取多矩形】融合全部矩形"
                : auto ? "点【自动抓取矩形】识别格线"
                : "按住鼠标左键拖出已知区域";
            _hasMeasure = false;
            _autoOk = false;
            _multiOk = false;
            _gridX.Clear(); _gridY.Clear();
            _rects.Clear(); _rectOk.Clear(); _manualRects.Clear();
            _manualW = _manualH = _manualUsed = _rectUsed = 0;
            _autoInfo.Text = m == 5 ? "尚未检测多矩形"
                : auto ? "尚未检测" : "可连续拖多个矩形";
            _pic.Invalidate();
            Recalc();
            if (auto && _image != null)
            {
                if (m == 5) AutoDetectRects(); else AutoDetectGrid();
                return;
            }
            string tip = m == 0 ? "在图上拖一个已知宽高的矩形"
                : m == 1 ? "在图上沿水平方向拖一条已知长度的线"
                : m == 2 ? "在图上沿垂直方向拖一条已知长度的线"
                : m == 3 ? "直接填写每个像素对应的毫米数"
                : m == 4 ? "载入标定板后点【自动抓取矩形】，或先选板型并生成标定板"
                : "载入含多个矩形的标定板后点【自动抓取多矩形】";
            SetStatus("当前方式：" + _mode.SelectedItem + "。" + tip);
        }

        /// <summary>
        /// 自动抓取标定板矩形：灰度化 → 水平/垂直亮度梯度投影 → 峰值找格线
        /// → 相邻格线间距取中位数，得到每格的像素宽高。对棋盘格与单个矩形块都适用。
        /// </summary>
        private void AutoDetectGrid()
        {
            _autoOk = false;
            _hasMeasure = false;
            _gridX.Clear(); _gridY.Clear();
            _cellW = _cellH = 0;
            if (_image == null)
            {
                _autoInfo.Text = "请先载入标定图或生成示例标定板";
                SetStatus("自动抓取需要先有一张标定图。");
                _pic.Invalidate(); Recalc();
                return;
            }

            int w = _image.Width, h = _image.Height;
            byte[] gray = ToGray(_image, w, h);

            double[] col = new double[Math.Max(1, w - 1)];
            double[] row = new double[Math.Max(1, h - 1)];
            for (int y = 0; y < h - 1; y++)
            {
                int i = y * w;
                for (int x = 0; x < w - 1; x++)
                {
                    col[x] += Math.Abs(gray[i + x + 1] - gray[i + x]);
                    row[y] += Math.Abs(gray[i + x + w] - gray[i + x]);
                }
            }
            for (int x = 0; x < col.Length; x++) col[x] /= Math.Max(1, h - 1);
            for (int y = 0; y < row.Length; y++) row[y] /= Math.Max(1, w - 1);
            Smooth(col);
            Smooth(row);

            int gapX = Math.Max(8, w / 60), gapY = Math.Max(8, h / 60);
            _gridX.AddRange(FindPeaks(col, gapX));
            _gridY.AddRange(FindPeaks(row, gapY));
            // 先用相邻间距中位数作初值，再对整板格线做最小二乘拟合精化
            _cellW = FitSpacing(_gridX, MedianGap(_gridX, gapX));
            _cellH = FitSpacing(_gridY, MedianGap(_gridY, gapY));
            _autoOk = _cellW >= 5 && _cellH >= 5;

            if (_autoOk)
            {
                // 高亮框用「中位数格子尺寸」，避免个别误检格线把框画扁；
                // 起点选靠近画面中心、且右下还放得下完整一格的格线。
                int x0 = 0, y0 = 0;
                double bx = double.MaxValue, by = double.MaxValue;
                foreach (int g in _gridX)
                {
                    if (g + _cellW >= w) continue;
                    double d = Math.Abs(g + _cellW / 2.0 - w / 2.0);
                    if (d < bx) { bx = d; x0 = g; }
                }
                foreach (int g in _gridY)
                {
                    if (g + _cellH >= h) continue;
                    double d = Math.Abs(g + _cellH / 2.0 - h / 2.0);
                    if (d < by) { by = d; y0 = g; }
                }
                _mA = new Point(x0, y0);
                _mB = new Point(Math.Min(w - 1, x0 + _cellW),
                                Math.Min(h - 1, y0 + _cellH));
                _hasMeasure = true;
                _autoInfo.Text = "格线 X " + _gridX.Count + " / Y " + _gridY.Count
                    + " 条，整板拟合 " + _cellW + "×" + _cellH + " px";
                SetStatus("自动抓取完成：X " + _gridX.Count + " 条、Y " + _gridY.Count
                    + " 条格线，每格 " + _cellW + "×" + _cellH
                    + " px。核对每格实际毫米数后点【应用到软件】。");
            }
            else
            {
                _autoInfo.Text = "未找到规则矩形格线，可换图或改用手动量取";
                SetStatus("没找到规则的矩形格线。可换更清晰的标定板，或改用手动量取。");
            }
            _pic.Invalidate();
            Recalc();
        }

        private static byte[] ToGray(Bitmap img, int w, int h)
        {
            byte[] gray = new byte[w * h];
            using (Bitmap b24 = new Bitmap(w, h, PixelFormat.Format24bppRgb))
            {
                using (Graphics gg = Graphics.FromImage(b24)) gg.DrawImage(img, 0, 0, w, h);
                BitmapData bd = b24.LockBits(new Rectangle(0, 0, w, h),
                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                try
                {
                    int stride = bd.Stride;
                    byte[] raw = new byte[stride * h];
                    Marshal.Copy(bd.Scan0, raw, 0, raw.Length);
                    for (int y = 0; y < h; y++)
                    {
                        int ro = y * stride, go = y * w;
                        for (int x = 0; x < w; x++)
                        {
                            int i = ro + x * 3;
                            gray[go + x] = (byte)((raw[i] * 77 + raw[i + 1] * 150 + raw[i + 2] * 29) >> 8);
                        }
                    }
                }
                finally { b24.UnlockBits(bd); }
            }
            return gray;
        }

        /// <summary>整板最小二乘拟合 p_i ≈ a + b·i，斜率即格距；先按初值剔除离群峰。</summary>
        private static int FitSpacing(List<int> p, int med)
        {
            if (p.Count < 2 || med < 5) return med;
            List<double> xs = new List<double>(), ys = new List<double>();
            int first = p[0];
            foreach (int v in p)
            {
                double i = (v - first) / (double)med;
                if (Math.Abs(i - Math.Round(i)) > 0.35) continue;
                xs.Add(Math.Round(i)); ys.Add(v);
            }
            if (xs.Count < 2) return med;
            double mx = 0, my = 0;
            foreach (double v in xs) mx += v;
            foreach (double v in ys) my += v;
            mx /= xs.Count; my /= ys.Count;
            double num = 0, den = 0;
            for (int i = 0; i < xs.Count; i++)
            {
                num += (xs[i] - mx) * (ys[i] - my);
                den += (xs[i] - mx) * (xs[i] - mx);
            }
            if (den <= 0) return med;
            int fit = (int)Math.Round(num / den);
            return (fit >= 5 && Math.Abs(fit - med) <= Math.Max(2, med / 5)) ? fit : med;
        }

        /// <summary>检测画面中多个独立矩形块，剔除离群后按面积加权融合宽高。</summary>
        private void AutoDetectRects()
        {
            _multiOk = false;
            _hasMeasure = false;
            _rects.Clear();
            _rectUsed = 0;
            _cellW = _cellH = 0;
            _gridX.Clear(); _gridY.Clear();
            if (_image == null)
            {
                _autoInfo.Text = "请先载入标定图或生成标定板";
                SetStatus("多矩形融合需要先有一张标定图。");
                _pic.Invalidate(); Recalc(); return;
            }
            int w = _image.Width, h = _image.Height;
            byte[] gray = ToGray(_image, w, h);
            byte[] blur = new byte[w * h];
            for (int i = 0; i < blur.Length; i++) blur[i] = 255;
            for (int y = 1; y < h - 1; y++)
                for (int x = 1; x < w - 1; x++)
                {
                    int i = y * w + x;
                    int s = gray[i - w - 1] + gray[i - w] + gray[i - w + 1]
                          + gray[i - 1] + gray[i] + gray[i + 1]
                          + gray[i + w - 1] + gray[i + w] + gray[i + w + 1];
                    blur[i] = (byte)(s / 9);
                }
            byte[] mask = new byte[w * h];
            for (int i = 0; i < mask.Length; i++) mask[i] = (byte)(blur[i] < 128 ? 1 : 0);
            _rects.AddRange(FindRectangles(mask, w, h));
            List<bool> accepted;
            FuseRectangles(_rects, out _cellW, out _cellH, out _rectUsed, out accepted);
            _rectOk.Clear();
            _rectOk.AddRange(accepted);
            _multiOk = _cellW >= 5 && _cellH >= 5 && _rects.Count > 0;
            if (_multiOk)
            {
                Rectangle best = _rects[0];
                double bd = double.MaxValue;
                foreach (Rectangle r in _rects)
                {
                    double d = Math.Abs(r.X + r.Width / 2.0 - w / 2.0)
                             + Math.Abs(r.Y + r.Height / 2.0 - h / 2.0);
                    if (d < bd) { bd = d; best = r; }
                }
                _mA = new Point(best.X, best.Y);
                _mB = new Point(Math.Min(w - 1, best.Right), Math.Min(h - 1, best.Bottom));
                _hasMeasure = true;
                _autoInfo.Text = "检出 " + _rects.Count + " 个矩形，采用 " + _rectUsed
                    + " 个，融合 " + _cellW + "×" + _cellH + " px";
                SetStatus("多矩形融合完成：检出 " + _rects.Count + " 个矩形、采用 "
                    + _rectUsed + " 个，融合尺寸 " + _cellW + "×" + _cellH
                    + " px。核对实际毫米数后点【应用到软件】。");
            }
            else
            {
                _autoInfo.Text = "未找到合格矩形块，可换图或改用格线模式";
                SetStatus("没找到合格的矩形块。可换更清晰的标定板，或改用格线模式。");
            }
            _pic.Invalidate();
            Recalc();
        }

        private static List<Rectangle> FindRectangles(byte[] mask, int w, int h)
        {
            List<Rectangle> res = new List<Rectangle>();
            byte[] seen = new byte[w * h];
            int[] st = new int[w * h];
            int minArea = Math.Max(100, (w * h) / 800);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int idx = y * w + x;
                    if (mask[idx] == 0 || seen[idx] != 0) continue;
                    int sp = 0; st[sp++] = idx; seen[idx] = 1;
                    int minX = x, maxX = x, minY = y, maxY = y, area = 0;
                    while (sp > 0)
                    {
                        int c = st[--sp], cx = c % w, cy = c / w;
                        area++;
                        if (cx < minX) minX = cx;
                        if (cx > maxX) maxX = cx;
                        if (cy < minY) minY = cy;
                        if (cy > maxY) maxY = cy;
                        if (cx > 0) { int n = c - 1; if (mask[n] != 0 && seen[n] == 0) { seen[n] = 1; st[sp++] = n; } }
                        if (cx < w - 1) { int n = c + 1; if (mask[n] != 0 && seen[n] == 0) { seen[n] = 1; st[sp++] = n; } }
                        if (cy > 0) { int n = c - w; if (mask[n] != 0 && seen[n] == 0) { seen[n] = 1; st[sp++] = n; } }
                        if (cy < h - 1) { int n = c + w; if (mask[n] != 0 && seen[n] == 0) { seen[n] = 1; st[sp++] = n; } }
                    }
                    Rectangle r = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
                    double fill = (double)area / Math.Max(1, r.Width * r.Height);
                    double ar = (double)r.Width / Math.Max(1, r.Height);
                    if (area >= minArea && fill >= 0.72 && r.Width >= 10 && r.Height >= 10
                        && ar >= 0.2 && ar <= 5.0) res.Add(r);
                }
            return res;
        }

        /// <summary>清空全部量取结果（手动框、自动矩形、格线）。</summary>
        private void ClearMeasurement()
        {
            _hasMeasure = false;
            _autoOk = false;
            _multiOk = false;
            _gridX.Clear(); _gridY.Clear();
            _rects.Clear(); _rectOk.Clear(); _manualRects.Clear();
            _manualW = _manualH = _manualUsed = _rectUsed = 0;
            _cellW = _cellH = 0;
            if (_autoInfo != null) _autoInfo.Text = "已清除，请重新量取或抓取";
            SetStatus("已清除全部量取结果，可重新拖框或点自动抓取。");
            _pic.Invalidate();
            Recalc();
        }

        /// <summary>多个矩形融合：按宽/高中位数剔除离群，再按面积加权平均。
        /// 自动检测与手动多框共用，accepted 逐项标记是否被采用。</summary>
        private static void FuseRectangles(List<Rectangle> rects, out int w, out int h,
            out int used, out List<bool> accepted)
        {
            w = h = used = 0;
            accepted = new List<bool>();
            for (int i = 0; i < rects.Count; i++) accepted.Add(false);
            if (rects.Count == 0) return;
            int mw = MedianDim(rects, true), mh = MedianDim(rects, false);
            double sw = 0, sh = 0, sa = 0;
            for (int i = 0; i < rects.Count; i++)
            {
                Rectangle r = rects[i];
                if (Math.Abs(r.Width - mw) > Math.Max(2, mw / 4)) continue;
                if (Math.Abs(r.Height - mh) > Math.Max(2, mh / 4)) continue;
                double a = (double)r.Width * r.Height;
                sw += r.Width * a; sh += r.Height * a; sa += a;
                accepted[i] = true; used++;
            }
            if (sa > 0) { w = (int)Math.Round(sw / sa); h = (int)Math.Round(sh / sa); }
            else { w = mw; h = mh; }
        }

        private static int MedianDim(List<Rectangle> rs, bool width)
        {
            List<int> v = new List<int>();
            foreach (Rectangle r in rs) v.Add(width ? r.Width : r.Height);
            v.Sort();
            return v[v.Count / 2];
        }

        private static void Smooth(double[] v)
        {
            if (v.Length < 3) return;
            double[] a = (double[])v.Clone();
            for (int i = 1; i < v.Length - 1; i++)
                v[i] = (a[i - 1] + 2 * a[i] + a[i + 1]) / 4.0;
        }

        private static List<int> FindPeaks(double[] v, int minGap)
        {
            List<int> res = new List<int>();
            if (v.Length < 3) return res;
            double sum = 0, max = 0;
            foreach (double d in v) { sum += d; if (d > max) max = d; }
            if (max <= 0) return res;
            double mean = sum / v.Length;
            double th = Math.Max(mean * 2.2, max * 0.30);
            List<int> cand = new List<int>();
            for (int i = 1; i < v.Length - 1; i++)
                if (v[i] >= v[i - 1] && v[i] > v[i + 1] && v[i] > th) cand.Add(i);
            cand.Sort(delegate(int a, int b) { return v[b].CompareTo(v[a]); });
            foreach (int c in cand)
            {
                bool ok = true;
                foreach (int p in res)
                    if (Math.Abs(p - c) < minGap) { ok = false; break; }
                if (ok) res.Add(c);
            }
            res.Sort();
            return res;
        }

        private static int MedianGap(List<int> p, int minGap)
        {
            if (p.Count < 2) return 0;
            List<int> g = new List<int>();
            for (int i = 0; i < p.Count - 1; i++)
            {
                int d = p[i + 1] - p[i];
                if (d >= minGap) g.Add(d);
            }
            if (g.Count == 0) return 0;
            g.Sort();
            return g[g.Count / 2];
        }

        private void LoadImage()
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Title = "选择标定件图片（含已知尺寸的标准件 / 标定板）";
                d.Filter = "图片文件|*.bmp;*.png;*.jpg;*.jpeg|全部文件|*.*";
                if (d.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    if (_image != null) { _image.Dispose(); _image = null; }
                    _image = new Bitmap(d.FileName);
                    _pic.Image = _image;
                }
                catch { SetStatus("这张图打不开。"); return; }
            }
            _hasMeasure = false;
            _pic.Invalidate();
            if (_mode.SelectedIndex == 4) { AutoDetectGrid(); return; }
            if (_mode.SelectedIndex == 5) { AutoDetectRects(); return; }
            Recalc();
            SetStatus("已载入 " + _image.Width + "×" + _image.Height
                + " 标定图。在图上拖出已知尺寸区域，再核对左侧实际尺寸。");
        }

        /// <summary>按板型程序化绘制一张 800×600 矩形标定板（静态，供预览与批量导出复用）。</summary>
        private static Bitmap RenderBoard(int kind)
        {
            Bitmap b = new Bitmap(800, 600, PixelFormat.Format24bppRgb);
            int cw = BoardCellW[kind], ch = BoardCellH[kind];
            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(Color.White);
                if (kind == 4)
                {
                    // 单个矩形块：只有 4 条边界，验证「2 条格线也能标定」
                    using (Brush dark = new SolidBrush(Color.FromArgb(35, 42, 54)))
                        g.FillRectangle(dark, (800 - cw) / 2, (600 - ch) / 2, cw, ch);
                }
                else
                {
                    using (Brush dark = new SolidBrush(Color.FromArgb(40, 46, 58)))
                    using (Brush light = new SolidBrush(Color.FromArgb(225, 230, 238)))
                    {
                        for (int y = 0; y * ch < 600; y++)
                            for (int x = 0; x * cw < 800; x++)
                                g.FillRectangle(((x + y) % 2 == 0) ? dark : light,
                                    x * cw, y * ch, cw, ch);
                    }
                    // 不在位图里画网格线：色块交替本身就有足够梯度供格线检测，
                    // 且线条会切掉矩形边缘 1px，影响多矩形轮廓量取。格线只在显示层绘制。
                }
                if (kind == 5) AddNoise(b);
                using (Font f = new Font("Microsoft YaHei UI", 12f, FontStyle.Bold))
                using (Brush bg = new SolidBrush(Color.FromArgb(215, 10, 20, 34)))
                {
                    string tag = (kind + 1).ToString("00") + "  " + BoardNames[kind];
                    g.FillRectangle(bg, 14, 14, 380, 30);
                    g.DrawString(tag, f, Brushes.White, 22, 17);
                }
            }
            return b;
        }

        /// <summary>噪声板：轻微亮度噪声 + 暗角，模拟实拍并检验自动抓取鲁棒性。</summary>
        private static void AddNoise(Bitmap b)
        {
            int w = b.Width, h = b.Height;
            BitmapData bd = b.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            try
            {
                int stride = bd.Stride;
                byte[] raw = new byte[stride * h];
                Marshal.Copy(bd.Scan0, raw, 0, raw.Length);
                Random rnd = new Random(20261114);
                for (int y = 0; y < h; y++)
                {
                    int ro = y * stride;
                    for (int x = 0; x < w; x++)
                    {
                        int i = ro + x * 3;
                        double dx = (x - w / 2.0) / (w / 2.0);
                        double dy = (y - h / 2.0) / (h / 2.0);
                        double vig = 1.0 - 0.22 * Math.Min(1.0, dx * dx + dy * dy);
                        int n = rnd.Next(-11, 12);
                        for (int c = 0; c < 3; c++)
                        {
                            int v = (int)(raw[i + c] * vig) + n;
                            raw[i + c] = (byte)Math.Max(0, Math.Min(255, v));
                        }
                    }
                }
                Marshal.Copy(raw, 0, bd.Scan0, raw.Length);
            }
            finally { b.UnlockBits(bd); }
        }

        private void MakeCheckerboard()
        {
            int k = _boardKind.SelectedIndex;
            if (k < 0 || k >= BoardNames.Length) k = 0;
            if (_image != null) { _image.Dispose(); _image = null; }
            _image = RenderBoard(k);
            _mmW.Value = (decimal)BoardMmW[k];
            _mmH.Value = (decimal)BoardMmH[k];
            _mmLen.Value = (decimal)BoardMmW[k];
            _hasMeasure = false;
            _pic.Image = _image;
            _pic.Invalidate();
            if (_mode.SelectedIndex == 4) { AutoDetectGrid(); return; }
            if (_mode.SelectedIndex == 5) { AutoDetectRects(); return; }
            Recalc();
            SetStatus("已生成「" + BoardNames[k] + "」：每格 "
                + BoardCellW[k] + "×" + BoardCellH[k] + " px = "
                + BoardMmW[k] + "×" + BoardMmH[k] + " mm。现场请用真实标定件。");
        }

        /// <summary>一次性导出全部板型为 PNG，方便打印/实拍/批量测试自动抓取。</summary>
        private void ExportAllBoards()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "标定板");
            try
            {
                Directory.CreateDirectory(dir);
                for (int k = 0; k < BoardNames.Length; k++)
                {
                    using (Bitmap bmp = RenderBoard(k))
                    {
                        string name = (k + 1).ToString("00") + "_"
                            + SafeName(BoardNames[k]) + ".png";
                        bmp.Save(Path.Combine(dir, name), ImageFormat.Png);
                    }
                }
                SetStatus("已导出 " + BoardNames.Length + " 张矩形标定板到：" + dir);
            }
            catch (Exception ex) { SetStatus("导出失败：" + ex.Message); }
        }

        private static string SafeName(string s)
        {
            string r = s.Replace('×', 'x').Replace(" / ", "_").Replace(' ', '_');
            foreach (char c in Path.GetInvalidFileNameChars()) r = r.Replace(c, '_');
            return r;
        }

        private void Recalc()
        {
            _scaleX = _origX;
            _scaleY = _origY;
            int m = _mode == null ? 0 : _mode.SelectedIndex;
            string px = "—";
            if (m == 0 && _manualRects.Count > 0)
            {
                px = "手动 " + _manualRects.Count + " 框："
                    + _manualW + " × " + _manualH + " px";
                if (_manualW >= 2 && _manualH >= 2)
                {
                    _scaleX = (double)_mmW.Value / _manualW;
                    _scaleY = (double)_mmH.Value / _manualH;
                }
            }
            else if (m == 3)
            {
                _scaleX = (double)_manualX.Value;
                _scaleY = (double)_manualY.Value;
                px = "手动输入";
            }
            else if (m == 4 || m == 5)
            {
                bool ok = m == 5 ? _multiOk : _autoOk;
                if (ok)
                {
                    px = (m == 5 ? "多矩形融合：" : "整板拟合：")
                        + _cellW + " × " + _cellH + " px";
                    _scaleX = (double)_mmW.Value / _cellW;
                    _scaleY = (double)_mmH.Value / _cellH;
                }
                else px = "等待自动抓取";
            }
            else if (_hasMeasure)
            {
                int w = Math.Abs(_mB.X - _mA.X);
                int h = Math.Abs(_mB.Y - _mA.Y);
                if (m == 0)
                {
                    px = w + " × " + h + " px";
                    if (w >= 2 && h >= 2)
                    {
                        _scaleX = (double)_mmW.Value / w;
                        _scaleY = (double)_mmH.Value / h;
                    }
                }
                else if (m == 1)
                {
                    px = w + " px（水平）";
                    if (w >= 2) _scaleX = (double)_mmLen.Value / w;
                }
                else
                {
                    px = h + " px（垂直）";
                    if (h >= 2) _scaleY = (double)_mmLen.Value / h;
                }
            }
            if (_pxMeasure != null) _pxMeasure.Text = "像素测量：" + px;
            if (_resX != null)
            {
                _resX.Text = "ScaleX = " + _scaleX.ToString("F6", CultureInfo.InvariantCulture);
                _resY.Text = "ScaleY = " + _scaleY.ToString("F6", CultureInfo.InvariantCulture);
                _resDemo.Text = "100 px = "
                    + (_scaleX * 100).ToString("F3", CultureInfo.InvariantCulture) + " mm（X）";
            }
            if (_curLabel != null)
                _curLabel.Text = "当前 "
                    + _origX.ToString("F5", CultureInfo.InvariantCulture) + " / "
                    + _origY.ToString("F5", CultureInfo.InvariantCulture) + " mm/px";
        }

        private void Apply()
        {
            if (_scaleX <= 0 || _scaleY <= 0)
            { SetStatus("标定值必须大于 0。"); return; }
            if ((_mode.SelectedIndex == 4 && !_autoOk)
                || (_mode.SelectedIndex == 5 && !_multiOk))
            {
                MessageBox.Show(this, "请先点自动抓取，让程序识别出标定板格线或多个矩形。",
                    "还没有自动抓取结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_mode.SelectedIndex != 3 && !_hasMeasure)
            {
                MessageBox.Show(this, "请先在图上拖出已知尺寸的区域，或改用【手动输入 mm/px】。",
                    "还没有测量", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        // ---------- 图像区交互 ----------
        private bool ClientToImage(Point c, out Point p)
        {
            p = new Point();
            if (_image == null) return false;
            float scale = Math.Min(
                (float)_pic.ClientSize.Width / _image.Width,
                (float)_pic.ClientSize.Height / _image.Height);
            if (scale <= 0) return false;
            float ox = (_pic.ClientSize.Width - _image.Width * scale) / 2f;
            float oy = (_pic.ClientSize.Height - _image.Height * scale) / 2f;
            int x = (int)Math.Round((c.X - ox) / scale);
            int y = (int)Math.Round((c.Y - oy) / scale);
            p = new Point(Math.Max(0, Math.Min(_image.Width - 1, x)),
                          Math.Max(0, Math.Min(_image.Height - 1, y)));
            return true;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || _image == null) return;
            if (_mode.SelectedIndex == 3 || _mode.SelectedIndex == 4
                || _mode.SelectedIndex == 5) return;
            _dragA = _dragB = e.Location;
            _dragging = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            _dragB = e.Location;
            _pic.Invalidate();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            _dragging = false;
            _dragB = e.Location;
            Point a, b;
            if (!ClientToImage(_dragA, out a) || !ClientToImage(_dragB, out b))
            { _pic.Invalidate(); return; }
            if (Math.Abs(a.X - b.X) < 2 && Math.Abs(a.Y - b.Y) < 2)
            { _pic.Invalidate(); return; }
            _mA = a; _mB = b;
            _hasMeasure = true;
            if (_mode.SelectedIndex == 0)
            {
                // 手动模式：连续拖多个矩形，全部保留并自动融合
                Rectangle r = Rectangle.FromLTRB(
                    Math.Min(a.X, b.X), Math.Min(a.Y, b.Y),
                    Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
                _manualRects.Add(r);
                List<bool> acc;
                FuseRectangles(_manualRects, out _manualW, out _manualH,
                    out _manualUsed, out acc);
                _autoInfo.Text = "已标 " + _manualRects.Count + " 个矩形，采用 "
                    + _manualUsed + " 个，融合 " + _manualW + "×" + _manualH + " px";
                SetStatus("已标 " + _manualRects.Count + " 个矩形（采用 "
                    + _manualUsed + " 个），融合 " + _manualW + "×" + _manualH
                    + " px。可继续拖框提高精度，或点【应用到软件】。");
            }
            else
            {
                SetStatus("已量取 " + Math.Abs(a.X - b.X) + " × " + Math.Abs(a.Y - b.Y)
                    + " px，核对左侧实际尺寸后点【应用到软件】。");
            }
            _pic.Invalidate();
            Recalc();
        }

        private void OnPaintImage(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int w = _pic.ClientSize.Width, h = _pic.ClientSize.Height;

            if (_image == null)
            {
                int bw = Math.Min(520, w - 80), bh = 180;
                Rectangle rc = new Rectangle((w - bw) / 2, (h - bh) / 2, bw, bh);
                using (Pen dash = new Pen(Color.FromArgb(90, 90, 140, 175), 2f))
                {
                    dash.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(dash, rc);
                }
                using (Font f1 = new Font("Microsoft YaHei UI", 20f, FontStyle.Bold))
                using (Font f2 = new Font("Microsoft YaHei UI", 11f))
                {
                    string t = "载入标定图，开始标定";
                    SizeF a = g.MeasureString(t, f1);
                    g.DrawString(t, f1, Brushes.White, (w - a.Width) / 2, rc.Y + 30);
                    string s = "选板型后点【生成标定板】，或直接【导出全部板型】";
                    SizeF b = g.MeasureString(s, f2);
                    g.DrawString(s, f2, Brushes.LightSteelBlue, (w - b.Width) / 2, rc.Y + 88);
                }
                return;
            }

            float scale = Math.Min((float)w / _image.Width, (float)h / _image.Height);
            float ox = (w - _image.Width * scale) / 2f;
            float oy = (h - _image.Height * scale) / 2f;

            // 手动模式：所有拖过的框都画出来，逐个编号
            if (_mode.SelectedIndex == 0 && _manualRects.Count > 0)
                DrawRectSet(g, _manualRects, null, ox, oy, scale,
                    Color.FromArgb(0, 229, 255), false, true);

            // 格线模式：先给每个格子一个独立淡框，再画亮格线
            if (_mode.SelectedIndex == 4 && _gridX.Count > 1 && _gridY.Count > 1)
            {
                using (Pen faint = new Pen(Color.FromArgb(75, 0, 229, 255), 1f))
                    for (int i = 0; i < _gridX.Count - 1; i++)
                        for (int j = 0; j < _gridY.Count - 1; j++)
                        {
                            float cx = ox + _gridX[i] * scale + 2;
                            float cy = oy + _gridY[j] * scale + 2;
                            float cw = Math.Max(1, (_gridX[i + 1] - _gridX[i]) * scale - 4);
                            float ch = Math.Max(1, (_gridY[j + 1] - _gridY[j]) * scale - 4);
                            g.DrawRectangle(faint, cx, cy, cw, ch);
                        }
            }
            if (_mode.SelectedIndex == 4 && _gridX.Count > 0)
            {
                using (Pen pen = new Pen(Color.FromArgb(150, 0, 229, 255), 1.2f))
                {
                    float bottom = oy + _image.Height * scale;
                    float right = ox + _image.Width * scale;
                    foreach (int gx in _gridX)
                        g.DrawLine(pen, ox + gx * scale, oy, ox + gx * scale, bottom);
                    foreach (int gy in _gridY)
                        g.DrawLine(pen, ox, oy + gy * scale, right, oy + gy * scale);
                }
            }

            // 多矩形模式：全部框出并编号；被剔除的用灰色虚线
            if (_mode.SelectedIndex == 5 && _rects.Count > 0)
                DrawRectSet(g, _rects, _rectOk, ox, oy, scale,
                    Color.FromArgb(255, 197, 61), true, true);

            if (_hasMeasure) DrawMeasure(g, ox, oy, scale,
                Color.FromArgb(255, 70, 45), 3.2f, false);
            if (_dragging) DrawDrag(g, Color.Khaki);

            string tag;
            if (_mode.SelectedIndex == 0 && _manualRects.Count > 1)
                tag = "已标 " + _manualRects.Count + " 框 · 融合 "
                    + _manualW + "×" + _manualH + " px";
            else if (_mode.SelectedIndex == 5 && _rects.Count > 0)
                tag = "检出 " + _rects.Count + " 矩形 · 融合 "
                    + _cellW + "×" + _cellH + " px";
            else if (_hasMeasure)
                tag = "测量：" + Math.Abs(_mB.X - _mA.X) + " × "
                    + Math.Abs(_mB.Y - _mA.Y) + " px";
            else tag = "尚未测量";
            using (Font f = new Font("Microsoft YaHei UI", 10f, FontStyle.Bold))
            using (Brush bg = new SolidBrush(Color.FromArgb(190, 10, 20, 34)))
            {
                SizeF sz = g.MeasureString(tag, f);
                // 放左下角，避开标定板自带的左上角说明文字
                float ty = h - sz.Height - 24;
                g.FillRectangle(bg, 10, ty, sz.Width + 20, sz.Height + 12);
                g.DrawString(tag, f, Brushes.White, 20, ty + 6);
            }
        }

        /// <summary>把一组矩形全部框出并编号；未采用的用灰色虚线区分。</summary>
        private static void DrawRectSet(Graphics g, List<Rectangle> rects,
            List<bool> accepted, float ox, float oy, float scale,
            Color okColor, bool dashedReject, bool numbers)
        {
            using (Pen ok = new Pen(okColor, 2f))
            using (Pen bad = new Pen(Color.FromArgb(160, 160, 170, 180), 1f))
            using (Font nf = new Font("Microsoft YaHei UI", 9f, FontStyle.Bold))
            using (Brush nb = new SolidBrush(Color.FromArgb(210, 12, 20, 32)))
            {
                if (dashedReject) bad.DashStyle = DashStyle.Dash;
                for (int i = 0; i < rects.Count; i++)
                {
                    Rectangle r = rects[i];
                    bool take = accepted == null || i >= accepted.Count || accepted[i];
                    float x = ox + r.X * scale, y = oy + r.Y * scale;
                    float rw = Math.Max(1, r.Width * scale);
                    float rh = Math.Max(1, r.Height * scale);
                    g.DrawRectangle(take ? ok : bad, x, y, rw, rh);
                    if (numbers && take && rw >= 34 && rh >= 22)
                    {
                        string s = (i + 1).ToString();
                        SizeF sz = g.MeasureString(s, nf);
                        g.FillRectangle(nb, x + 2, y + 2, sz.Width + 6, sz.Height + 3);
                        g.DrawString(s, nf, Brushes.White, x + 5, y + 3);
                    }
                }
            }
        }

        private void DrawMeasure(Graphics g, float ox, float oy, float scale,
            Color color, float width, bool dash)
        {
            float x1 = ox + _mA.X * scale, y1 = oy + _mA.Y * scale;
            float x2 = ox + _mB.X * scale, y2 = oy + _mB.Y * scale;
            using (Pen pen = new Pen(color, width))
            {
                if (dash) pen.DashStyle = DashStyle.Dash;
                if (_mode.SelectedIndex == 0 || _mode.SelectedIndex == 4
                    || _mode.SelectedIndex == 5)
                    g.DrawRectangle(pen, Math.Min(x1, x2), Math.Min(y1, y2),
                        Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                else
                    g.DrawLine(pen, x1, y1, x2, y2);
            }
            using (Brush b = new SolidBrush(color))
            {
                g.FillEllipse(b, x1 - 4, y1 - 4, 8, 8);
                g.FillEllipse(b, x2 - 4, y2 - 4, 8, 8);
            }
        }

        private void DrawDrag(Graphics g, Color color)
        {
            float x1 = _dragA.X, y1 = _dragA.Y, x2 = _dragB.X, y2 = _dragB.Y;
            using (Pen pen = new Pen(color, 2f))
            {
                pen.DashStyle = DashStyle.Dash;
                if (_mode.SelectedIndex == 0)
                    g.DrawRectangle(pen, Math.Min(x1, x2), Math.Min(y1, y2),
                        Math.Abs(x2 - x1), Math.Abs(y2 - y1));
                else g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        private void SetStatus(string s) { if (_status != null) _status.Text = s; }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using ProductLocator.Imaging;

namespace TemplateMatchDemo
{
    /// <summary>绿色=判断位置的边界；紫色=没用的边界；黄色=必须有的特征但不参与位置判断。</summary>
    public enum FeatureRole { Position, Attached, Ignored, Unassigned }

    public sealed class FeatureLine
    {
        /// <summary>一条连续边界折线（程序只抓真实轮廓，不预设圆）。</summary>
        public List<PointF> Points = new List<PointF>();
        public FeatureRole Role = FeatureRole.Unassigned;
        public int Area;
        public Rectangle Bounds;

        // 圆形框选时按「真实边界」算出的圆度判定；绝不改动 Points
        public bool IsRoundCandidate;
        public double Roundness;

        public string RoundVerdict
        {
            get
            {
                if (!IsRoundCandidate) return "";
                if (Roundness >= 0.85) return "接近圆";
                if (Roundness >= 0.60) return "类圆";
                return "弱类圆";
            }
        }

        // 只有用户手动点「拟合成正圆」才会用到；RawPoints 保存真实轮廓以便还原
        public List<PointF> RawPoints;
        public bool IsFittedCircle;
        public PointF CircleCenter;
        public float CircleRadius;

        public void EnsureRaw()
        {
            if (RawPoints == null) RawPoints = new List<PointF>(Points);
        }

        public string RoleName
        {
            get
            {
                if (Role == FeatureRole.Position) return "绿色·判断位置";
                if (Role == FeatureRole.Attached) return "黄色·必须有";
                if (Role == FeatureRole.Ignored) return "紫色·无用";
                return "未定义（请选颜色）";
            }
        }
    }

    public sealed class MatchResult
    {
        public double DetX, DetY, Score;
        public int PixelDx, PixelDy;
        public double Angle;          // 角度偏移，单位「度」，正值＝产品图相对样板顺时针（与 GDI+ RotateTransform 同号）
        public bool AttachedFound;
        public double AttachedScore;
        public bool Found;             // 是否判定为「找到匹配」（相似度达标 + 必要特征存在）
        public double Threshold;       // 判定所用的相似度下限
        /// <summary>产品图独立抓到的线里，被判定为「位置特征」的索引（产品视图画红色高亮）。</summary>
        public List<int> PositionIndices = new List<int>();
    }

    /// <summary>一次框选区域。圆形框表示用户明确认为框内是圆/类圆，优先按圆处理。</summary>
    public sealed class Roi
    {
        public bool IsCircle;
        public Rectangle Bounds;   // 圆形框存正方形外接框

        public PointF Center
        {
            get { return new PointF(Bounds.X + Bounds.Width / 2f, Bounds.Y + Bounds.Height / 2f); }
        }

        public float Radius
        {
            get { return Math.Min(Bounds.Width, Bounds.Height) / 2f; }
        }
    }

    public sealed class TemplateData
    {
        public Rectangle Rect = new Rectangle(150, 150, 500, 220);
        public List<Roi> Rois = new List<Roi>();
        public int Sensitivity = 6;
        public int MinArea = 300;
        public int SmoothLevel = 3;
        public double ScaleX = 0.00500;
        public double ScaleY = 0.00402;
        /// <summary>判定容差：X/Y 各自的上、下限（mm），角度上、下限（度）。上下限可不对称。</summary>
        public double TolXMin = -0.10, TolXMax = 0.10;
        public double TolYMin = -0.10, TolYMax = 0.10;
        public double TolAngMin = -5.0, TolAngMax = 5.0;
        public string SamplePath = "";
        public string ProductPath = "";
        public List<FeatureLine> Lines = new List<FeatureLine>();
        public int LastThreshold;

        public PointF PositionCenter
        {
            get
            {
                double sx = 0, sy = 0;
                int n = 0;
                foreach (FeatureLine l in Lines)
                {
                    if (l.Role != FeatureRole.Position) continue;
                    foreach (PointF p in l.Points) { sx += p.X; sy += p.Y; n++; }
                }
                if (n == 0) return new PointF(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f);
                return new PointF((float)(sx / n), (float)(sy / n));
            }
        }

        public int CountByRole(FeatureRole r)
        {
            int n = 0;
            foreach (FeatureLine l in Lines) if (l.Role == r) n++;
            return n;
        }

        public List<FeatureLine> YellowLines
        {
            get
            {
                List<FeatureLine> r = new List<FeatureLine>();
                foreach (FeatureLine l in Lines)
                    if (l.Role == FeatureRole.Attached) r.Add(l);
                return r;
            }
        }

        public void Save(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TPL|1");
            sb.Append("RECT|").Append(Rect.X).Append('|').Append(Rect.Y).Append('|')
              .Append(Rect.Width).Append('|').Append(Rect.Height).AppendLine();
            sb.Append("PARA|").Append(Sensitivity).Append('|').Append(MinArea).Append('|')
              .Append(ScaleX.ToString("R", CultureInfo.InvariantCulture)).Append('|')
              .Append(ScaleY.ToString("R", CultureInfo.InvariantCulture)).AppendLine();
            sb.Append("SAMPLE|").Append(SamplePath).AppendLine();
            foreach (FeatureLine l in Lines)
                sb.Append("ROLE|").Append((int)l.Role).Append('|').Append(l.Area).AppendLine();
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        public static TemplateData Load(string path, out List<FeatureRole> roles)
        {
            TemplateData t = new TemplateData();
            roles = new List<FeatureRole>();
            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                string[] p = line.Split('|');
                if (p.Length >= 5 && p[0] == "RECT")
                    t.Rect = new Rectangle(int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]), int.Parse(p[4]));
                else if (p.Length >= 5 && p[0] == "PARA")
                {
                    t.Sensitivity = int.Parse(p[1]);
                    t.MinArea = int.Parse(p[2]);
                    t.ScaleX = double.Parse(p[3], CultureInfo.InvariantCulture);
                    t.ScaleY = double.Parse(p[4], CultureInfo.InvariantCulture);
                }
                else if (p.Length >= 2 && p[0] == "SAMPLE") t.SamplePath = p[1];
                else if (p.Length >= 2 && p[0] == "ROLE") roles.Add((FeatureRole)int.Parse(p[1]));
            }
            return t;
        }
    }

    /// <summary>
    /// 样板是含颗粒噪声的灰度图：背景中灰、亮特征灰度更高、内部有暗孔区域。
    /// 亮度分割 → 两轮 3x3 闭运算填颗粒洞 → 8 邻域连通 → Moore 邻域跟踪输出**一条连续外轮廓折线**。
    /// 匹配时复用同一套分割，保证样板与产品图的边界定义一致。
    /// </summary>
    public static class EdgeMap
    {
        /// <summary>
        /// Otsu 类间方差最大法：从灰度直方图求前景/背景最佳分割阈值。
        /// 比固定阈值更贴合真实边界——阈值落在背景峰与前景峰之间，
        /// 不会像旧公式那样压在背景边缘、把过渡带噪声也算进亮块（表现为轮廓外扩、冒小凸起）。
        /// 返回 0 表示直方图退化（几乎同色），调用方回退固定阈值。
        /// </summary>
        public static int OtsuThreshold(byte[] sm)
        {
            if (sm == null || sm.Length < 16) return 0;
            int[] hist = new int[256];
            for (int i = 0; i < sm.Length; i++) hist[sm[i]]++;
            long total = sm.Length, sumAll = 0;
            for (int t = 0; t < 256; t++) sumAll += (long)t * hist[t];
            long w0 = 0, sum0 = 0;
            double best = -1; int bestT = 0;
            for (int t = 0; t < 256; t++)
            {
                w0 += hist[t]; sum0 += (long)t * hist[t];
                long w1 = total - w0;
                if (w0 == 0 || w1 == 0) continue;
                double m0 = sum0 / (double)w0, m1 = (sumAll - sum0) / (double)w1;
                double between = w0 * w1 * (m0 - m1) * (m0 - m1);
                if (between > best) { best = between; bestT = t; }
            }
            return bestT;
        }

        /// <summary>
        /// 亮特征阈值：优先用 Otsu 自适应阈值（贴合真实边界、排除背景噪声），
        /// 灵敏度作为小范围偏移（灵敏度4=默认取 Otsu，越大阈值越低、抓得越多）；
        /// 直方图退化时回退旧固定公式。
        /// </summary>
        public static int Threshold(int sensitivity, byte[] sm)
        {
            int otsu = OtsuThreshold(sm);
            if (otsu > 0)
            {
                int t = otsu + (4 - sensitivity) * 2;
                if (t < 40) t = 40;
                if (t > 250) t = 250;
                return t;
            }
            return Threshold(sensitivity);
        }

        /// <summary>亮特征阈值（无灰度数据时的旧固定公式，仅供 UI 显示/兼容）。</summary>
        public static int Threshold(int sensitivity)
        {
            int t = 170 + (5 - sensitivity) * 2;
            if (t < 162) t = 162;
            if (t > 186) t = 186;
            return t;
        }

        /// <summary>匹配用的暗区阈值（宽松，保证边界响应）。</summary>
        public static int DarkThreshold(int sensitivity, byte[] sm)
        {
            int baseT = Threshold(sensitivity, sm);
            // 暗区比亮区更"宽松"：在 Otsu 基础上再抬一点，覆盖过渡带
            int t = baseT + 35;
            if (t > 245) t = 245;
            return t;
        }

        public static int DarkThreshold(int sensitivity)
        {
            int t = Threshold(sensitivity) + 35;
            if (t > 225) t = 225;
            return t;
        }

        /// <summary>
        /// 暗孔分割阈值：孔内部灰度偏低、亮块内部接近饱和、过渡带含颗粒噪声。
        /// 阈值需明显低于背景灰度才能只抓真正的暗孔，否则背景也会被判成孔。
        /// </summary>
        public static int HoleThreshold(int sensitivity)
        {
            int t = Threshold(sensitivity) + 14;
            if (t > 188) t = 188;
            return t;
        }

        private static byte[] GrayData(Bitmap bmp)
        {
            GrayImage gray = GrayImage.FromBitmap(bmp);
            return gray.Data;
        }

        public static byte[] Smooth(Bitmap bmp, out int w, out int h)
        {
            w = bmp.Width; h = bmp.Height;
#if SDK_BUILD
            EmbeddedDeps.Ensure();   // 独立 DLL 版：从嵌入资源加载 ProductLocator
#endif
            // 放到独立方法里，避免 JIT 在编译本方法时就解析外部类型
            // （那样会早于 Ensure 注册 AssemblyResolve）
            byte[] g = GrayData(bmp);
            float[] tmp = new float[w * h];
            byte[] sm = new byte[w * h];
            for (int y = 0; y < h; y++)
            {
                int row = y * w;
                for (int x = 0; x < w; x++)
                {
                    int s = 0;
                    for (int k = -2; k <= 2; k++)
                    {
                        int xx = x + k;
                        if (xx < 0) xx = 0;
                        if (xx >= w) xx = w - 1;
                        s += g[row + xx];
                    }
                    tmp[row + x] = s / 5f;
                }
            }
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    float s = 0;
                    for (int k = -2; k <= 2; k++)
                    {
                        int yy = y + k;
                        if (yy < 0) yy = 0;
                        if (yy >= h) yy = h - 1;
                        s += tmp[yy * w + x];
                    }
                    sm[y * w + x] = (byte)(s / 5f);
                }
            return sm;
        }

        private static bool[] Morph(bool[] src, int w, int h, bool dilate)
        {
            bool[] dst = new bool[w * h];
            for (int y = 1; y < h - 1; y++)
                for (int x = 1; x < w - 1; x++)
                {
                    bool v = dilate ? false : true;
                    for (int dy = -1; dy <= 1; dy++)
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            bool b = src[(y + dy) * w + x + dx];
                            if (dilate) v |= b; else v &= b;
                        }
                    dst[y * w + x] = v;
                }
            return dst;
        }

        /// <summary>亮特征实心掩膜（阈值 + 两轮闭运算填颗粒洞）。</summary>
        public static bool[] Segment(byte[] sm, int w, int h, int sensitivity)
        {
            int t = Threshold(sensitivity, sm);
            bool[] cur = new bool[w * h];
            for (int i = 0; i < cur.Length; i++) cur[i] = sm[i] > t;
            for (int pass = 0; pass < 2; pass++)
            {
                cur = Morph(cur, w, h, true);
                cur = Morph(cur, w, h, false);
            }
            return cur;
        }

        /// <summary>8 邻域连通分组。</summary>
        public static int Label(bool[] mask, int w, int h, out int[] label)
        {
            label = new int[w * h];
            int groups = 0;
            int[] dx = { 1, 1, 0, -1, -1, -1, 0, 1 };
            int[] dy = { 0, 1, 1, 1, 0, -1, -1, -1 };
            for (int i = 0; i < mask.Length; i++)
            {
                if (!mask[i] || label[i] != 0) continue;
                groups++;
                Stack<int> st = new Stack<int>();
                st.Push(i); label[i] = groups;
                while (st.Count > 0)
                {
                    int q = st.Pop();
                    int y = q / w, x = q - y * w;
                    for (int k = 0; k < 8; k++)
                    {
                        int nx = x + dx[k], ny = y + dy[k];
                        if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                        int ni = ny * w + nx;
                        if (mask[ni] && label[ni] == 0) { label[ni] = groups; st.Push(ni); }
                    }
                }
            }
            return groups;
        }

        /// <summary>3x3 闭运算，填颗粒噪声造成的小洞。</summary>
        public static bool[] CloseMask(bool[] src, int w, int h)
        {
            bool[] cur = src;
            for (int pass = 0; pass < 2; pass++)
            {
                bool[] dil = new bool[w * h];
                for (int y = 1; y < h - 1; y++)
                    for (int x = 1; x < w - 1; x++)
                    {
                        bool any = false;
                        for (int dy = -1; dy <= 1 && !any; dy++)
                            for (int dx = -1; dx <= 1; dx++)
                                if (cur[(y + dy) * w + x + dx]) { any = true; break; }
                        dil[y * w + x] = any;
                    }
                bool[] ero = new bool[w * h];
                for (int y = 1; y < h - 1; y++)
                    for (int x = 1; x < w - 1; x++)
                    {
                        bool all = true;
                        for (int dy = -1; dy <= 1 && all; dy++)
                            for (int dx = -1; dx <= 1; dx++)
                                if (!dil[(y + dy) * w + x + dx]) { all = false; break; }
                        ero[y * w + x] = all;
                    }
                cur = ero;
            }
            return cur;
        }

        /// <summary>暗区掩膜（圆孔、暗影）：用于匹配时提供亮块以外的第二条边界。</summary>
        public static bool[] DarkMask(byte[] sm, int w, int h, int sensitivity)
        {
            int t = DarkThreshold(sensitivity, sm);
            bool[] cur = new bool[w * h];
            for (int i = 0; i < cur.Length; i++) cur[i] = sm[i] < t;
            return CloseMask(cur, w, h);
        }

        /// <summary>边界像素串成一条连续折线；每点只访问一次，杜绝绕圈死循环。</summary>
        public static List<PointF> Trace(int[] label, int w, int h, int group, int startIdx)
        {
            int[] dx = { 1, 1, 0, -1, -1, -1, 0, 1 };
            int[] dy = { 0, 1, 1, 1, 0, -1, -1, -1 };
            bool[] isEdge = new bool[w * h];
            List<int> edges = new List<int>();
            for (int y = 1; y < h - 1; y++)
                for (int x = 1; x < w - 1; x++)
                {
                    int i = y * w + x;
                    if (label[i] != group) continue;
                    if (label[i - 1] != group || label[i + 1] != group
                        || label[i - w] != group || label[i + w] != group)
                    {
                        isEdge[i] = true;
                        edges.Add(i);
                    }
                }
            if (edges.Count < 8) return new List<PointF>();

            // 起点必须是边界点；再从多个起点各走一条链，取覆盖最多的一条。
            // 颗粒噪声会让边界出现 1~2px 断裂，单链跟踪会提前停。
            List<PointF> best = new List<PointF>();
            int tries = Math.Min(8, edges.Count);
            for (int t = 0; t < tries; t++)
            {
                int start = edges[t * edges.Count / tries];
                List<PointF> chain = WalkChain(isEdge, w, h, start, dx, dy);
                if (chain.Count > best.Count) best = chain;
            }
            return best;
        }

        /// <summary>8 邻域边界跟踪；走不动时在 5px 内跳到最近的未访问边界点续接。</summary>
        private static List<PointF> WalkChain(bool[] isEdge, int w, int h, int start,
            int[] dx, int[] dy)
        {
            bool[] used = new bool[w * h];
            int cx = start % w, cy = start / w, dir = 4;
            used[start] = true;
            List<PointF> pts = new List<PointF>();
            pts.Add(new PointF(cx, cy));
            int maxStep = w * h;
            for (int step = 0; step < maxStep; step++)
            {
                bool found = false;
                for (int k = 1; k <= 8; k++)
                {
                    int d = (dir + k) % 8;
                    int nx = cx + dx[d], ny = cy + dy[d];
                    if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                    int ni = ny * w + nx;
                    if (!isEdge[ni] || used[ni]) continue;
                    used[ni] = true;
                    pts.Add(new PointF(nx, ny));
                    dir = (d - 1 + 8) % 8;
                    cx = nx; cy = ny;
                    found = true;
                    break;
                }
                if (found) continue;

                int jump = -1, bestD = 25;   // 最多跨 5px 的断裂
                for (int jy = -4; jy <= 4; jy++)
                    for (int jx = -4; jx <= 4; jx++)
                    {
                        int dd = jx * jx + jy * jy;
                        if (dd >= bestD) continue;
                        int nx = cx + jx, ny = cy + jy;
                        if (nx < 1 || ny < 1 || nx >= w - 1 || ny >= h - 1) continue;
                        int ni = ny * w + nx;
                        if (!isEdge[ni] || used[ni]) continue;
                        bestD = dd; jump = ni;
                    }
                if (jump < 0) break;
                used[jump] = true;
                cx = jump % w; cy = jump / w;
                pts.Add(new PointF(cx, cy));
                dir = 4;
            }
            return pts;
        }

        /// <summary>从灰度直方图估计背景峰与前景峰（用于亚像素边缘定位的灰度中点）。</summary>
        public static void EstimateLevels(byte[] sm, out int lo, out int hi)
        {
            int[] hist = new int[256];
            for (int i = 0; i < sm.Length; i++) hist[sm[i]]++;
            // 背景：灰度最低处的最高峰；前景：灰度最高处的最高峰
            int loG = -1, loC = -1, hiG = -1, hiC = -1;
            for (int t = 0; t < 256; t++)
            {
                if (hist[t] <= 0) continue;
                if (loG < 0 || hist[t] > loC) { loG = t; loC = hist[t]; }
                if (hist[t] >= hiC) { hiG = t; hiC = hist[t]; }
            }
            lo = loG < 0 ? 160 : loG;
            // 前景峰应是"高灰度里像素最多的"，简单取后半段计数最大者，避免背景峰通吃
            hiC = -1; hiG = -1;
            int split = (lo + 255) / 2;
            for (int t = split; t < 256; t++)
                if (hist[t] > hiC) { hiC = hist[t]; hiG = t; }
            hi = hiG < 0 ? 245 : hiG;
            if (hi - lo < 20) hi = lo + 60;   // 前景不明显时的兜底
        }

        /// <summary>
        /// 亚像素边缘精修：二值阈值压在背景侧，抓到的轮廓点落在亮块外侧的半亮过渡像素上，
        /// 画出来比白色实体外扩一圈。此处按灰度梯度把每个点沿法线推到"背景/前景灰度中点"
        /// 的亚像素位置，让线精确贴合实体边缘。只移动点，不改变线的数量与拓扑。
        /// sm 为整幅（crop）平滑灰度；pts 为 crop 内坐标。
        /// </summary>
        public static List<PointF> RefineEdge(byte[] sm, int w, int h, List<PointF> pts,
            int loGray, int hiGray)
        {
            if (pts == null || pts.Count < 4 || sm == null) return pts;
            float mid = (loGray + hiGray) * 0.5f;   // 背景/前景灰度中点，物理边界所在
            List<PointF> outp = new List<PointF>(pts.Count);
            for (int i = 0; i < pts.Count; i++)
            {
                int x = (int)Math.Round(pts[i].X), y = (int)Math.Round(pts[i].Y);
                if (x < 1 || y < 1 || x >= w - 1 || y >= h - 1)
                { outp.Add(pts[i]); continue; }
                int idx = y * w + x;
                float gx = sm[idx + 1] - sm[idx - 1];
                float gy = sm[idx + w] - sm[idx - w];
                float gm = (float)Math.Sqrt(gx * gx + gy * gy);
                if (gm < 4f) { outp.Add(pts[i]); continue; }
                // 沿梯度（灰度增加=指向亮块内部）移动，使灰度达到中点
                float d = (mid - sm[idx]) / gm;
                if (d < -3f) d = -3f;
                if (d > 3f) d = 3f;
                outp.Add(new PointF(pts[i].X + gx / gm * d, pts[i].Y + gy / gm * d));
            }
            return outp;
        }

        /// <summary>
        /// 去除轮廓上的小毛刺/小环：二值掩膜边缘的噪点经闭运算放大后连在主体上，
        /// 轮廓跟踪会绕着它走一小圈（视觉上"多出一块"）。用"路径长度 / 弦长"识别：
        /// 一段点的路径远长于首尾直线距离（绕了远路），且跨度很短，即为毛刺，直接跳过。
        /// 只删这种小环，不改变主体形状。
        /// </summary>
        public static List<PointF> RemoveSpurs(List<PointF> src,
            float maxChord = 15f, float minRatio = 2.2f, float maxPath = 40f)
        {
            if (src == null || src.Count < 8) return src;
            List<PointF> pts = new List<PointF>(src);
            // 去掉与首点重合的尾点，按开放序列处理
            if (pts.Count > 1)
            {
                float dx0 = pts[0].X - pts[pts.Count - 1].X, dy0 = pts[0].Y - pts[pts.Count - 1].Y;
                if (dx0 * dx0 + dy0 * dy0 < 1f) pts.RemoveAt(pts.Count - 1);
            }
            // 单遍剪除：span 上限要能覆盖一个完整小环（本图约 22 点），
            // 但不能循环多遍（会连带吃掉紧邻的正常圆角）。
            List<PointF> outp = new List<PointF>(pts.Count);
            int i = 0, n = pts.Count;
            while (i < n)
            {
                int cut = -1;
                for (int span = 4; span <= 28 && i + span < n; span++)
                {
                    int j = i + span;
                    float cx = pts[j].X - pts[i].X, cy = pts[j].Y - pts[i].Y;
                    float chord = (float)Math.Sqrt(cx * cx + cy * cy);
                    if (chord < 0.1f || chord > maxChord) continue;
                    float path = 0f;
                    for (int k = i; k < j; k++)
                    {
                        float dx = pts[k + 1].X - pts[k].X, dy = pts[k + 1].Y - pts[k].Y;
                        path += (float)Math.Sqrt(dx * dx + dy * dy);
                    }
                    if (path > 0.1f && path / chord > minRatio && path < maxPath)
                    { cut = j; break; }
                }
                outp.Add(pts[i]);
                i = cut > 0 ? cut : i + 1;   // 有毛刺就跳过中间点，直接接到出口
            }
            return outp;
        }

        /// <summary>
        /// 轮廓圆滑：level 0 = 原始轮廓；1~5 用越来越大的循环窗口做两轮均值，
        /// 再等弧长重采样让点分布均匀。只改线条形状，不增删任何一条线。
        /// </summary>
        public static List<PointF> SmoothLine(List<PointF> src, int level)
        {
            if (src == null || src.Count < 8 || level <= 0) return src;
            int n = src.Count;
            float ex = src[0].X - src[n - 1].X, ey = src[0].Y - src[n - 1].Y;
            bool closed = ex * ex + ey * ey < 400f;   // 首尾 <20px 视为闭合轮廓
            int radius = 1 + level * 2;
            if (radius > 13) radius = 13;
            List<PointF> cur = new List<PointF>(src);
            for (int pass = 0; pass < 2; pass++)
            {
                List<PointF> next = new List<PointF>(n);
                for (int i = 0; i < n; i++)
                {
                    float sx = 0, sy = 0; int cnt = 0;
                    for (int k = -radius; k <= radius; k++)
                    {
                        int j = i + k;
                        if (closed) j = ((j % n) + n) % n;
                        else { if (j < 0) j = 0; if (j >= n) j = n - 1; }
                        sx += cur[j].X; sy += cur[j].Y; cnt++;
                    }
                    next.Add(new PointF(sx / cnt, sy / cnt));
                }
                cur = next;
            }
            return Resample(cur, 120, closed);
        }

        /// <summary>等弧长重采样：点距均匀，轮廓看起来更顺、更接近几何形状。</summary>
        private static List<PointF> Resample(List<PointF> pts, int target, bool close)
        {
            if (pts.Count < 3) return pts;
            float[] acc = new float[pts.Count];
            for (int i = 1; i < pts.Count; i++)
            {
                float dx = pts[i].X - pts[i - 1].X, dy = pts[i].Y - pts[i - 1].Y;
                acc[i] = acc[i - 1] + (float)Math.Sqrt(dx * dx + dy * dy);
            }
            float total = acc[pts.Count - 1];
            if (total < 1f) return pts;
            List<PointF> r = new List<PointF>();
            int seg = 1;
            for (int k = 0; k < target; k++)
            {
                float d = total * k / target;
                while (seg < pts.Count - 1 && acc[seg] < d) seg++;
                float d0 = acc[seg - 1], d1 = acc[seg];
                float t = d1 > d0 ? (d - d0) / (d1 - d0) : 0f;
                r.Add(new PointF(
                    pts[seg - 1].X + (pts[seg].X - pts[seg - 1].X) * t,
                    pts[seg - 1].Y + (pts[seg].Y - pts[seg - 1].Y) * t));
            }
            if (close && r.Count > 0) r.Add(r[0]);
            return r;
        }

        /// <summary>圆形度 = 4πA/P²，正圆为 1，越接近圆越接近 1。</summary>
        public static double Circularity(FeatureLine l)
        {
            if (l == null || l.Points.Count < 8) return 0;
            double peri = 0, area2 = 0;
            for (int i = 0; i < l.Points.Count; i++)
            {
                PointF a = l.Points[i], b = l.Points[(i + 1) % l.Points.Count];
                peri += Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
                area2 += a.X * b.Y - b.X * a.Y;
            }
            if (peri < 1) return 0;
            return 4 * Math.PI * Math.Abs(area2 / 2) / (peri * peri);
        }

        /// <summary>
        /// 把用户选中的线拟合成正圆：质心剔除远处拖尾 → Kåsa 最小二乘圆
        /// → 96 点闭合正圆。程序绝不自动判定圆。
        /// </summary>
        public static bool FitCircle(FeatureLine l)
        {
            if (l == null || l.Points.Count < 12) return false;
            l.EnsureRaw();
            List<PointF> src = l.RawPoints;
            int n = src.Count;
            double cx = 0, cy = 0;
            foreach (PointF p in src) { cx += p.X; cy += p.Y; }
            cx /= n; cy /= n;
            double r0 = 0;
            foreach (PointF p in src)
                r0 += Math.Sqrt((p.X - cx) * (p.X - cx) + (p.Y - cy) * (p.Y - cy));
            r0 /= n;
            if (r0 < 3) return false;

            // 剔除离质心过远的外凸拖尾。实测对 W 形圆孔的拟合偏差仅 0.6px，
            // 比凸包 / 角度中位半径方案更准（后者会把半径拉大约 2.6px）。
            List<PointF> keep = new List<PointF>();
            foreach (PointF p in src)
            {
                double d = Math.Sqrt((p.X - cx) * (p.X - cx) + (p.Y - cy) * (p.Y - cy));
                if (d < r0 * 1.45) keep.Add(p);
            }
            if (keep.Count < 12) return false;

            double sxx = 0, syy = 0, sxy = 0, sx = 0, sy = 0, sxz = 0, syz = 0, sz = 0;
            int m = keep.Count;
            foreach (PointF p in keep)
            {
                double x = p.X, y = p.Y, z = x * x + y * y;
                sxx += x * x; syy += y * y; sxy += x * y; sx += x; sy += y;
                sxz += x * z; syz += y * z; sz += z;
            }
            double[,] a = {
                { sxx, sxy, sx, -sxz },
                { sxy, syy, sy, -syz },
                { sx,  sy,  m,  -sz  }
            };
            for (int c = 0; c < 3; c++)
            {
                int piv = c;
                for (int r = c + 1; r < 3; r++)
                    if (Math.Abs(a[r, c]) > Math.Abs(a[piv, c])) piv = r;
                if (Math.Abs(a[piv, c]) < 1e-9) return false;
                if (piv != c)
                    for (int k = c; k < 4; k++)
                    { double t = a[c, k]; a[c, k] = a[piv, k]; a[piv, k] = t; }
                for (int r = 0; r < 3; r++)
                {
                    if (r == c) continue;
                    double f = a[r, c] / a[c, c];
                    for (int k = c; k < 4; k++) a[r, k] -= f * a[c, k];
                }
            }
            double A = a[0, 3] / a[0, 0], B = a[1, 3] / a[1, 1], C = a[2, 3] / a[2, 2];
            double ccx = -A / 2, ccy = -B / 2;
            double rr = Math.Sqrt(Math.Max(1.0, ccx * ccx + ccy * ccy - C));
            if (rr < 3 || rr > 400) return false;

            l.CircleCenter = new PointF((float)ccx, (float)ccy);
            l.CircleRadius = (float)rr;
            l.Points = CirclePoints(l.CircleCenter, l.CircleRadius);
            l.Bounds = new Rectangle((int)(ccx - rr), (int)(ccy - rr),
                (int)(rr * 2), (int)(rr * 2));
            l.IsFittedCircle = true;
            return true;
        }

        public static List<PointF> CirclePoints(PointF c, float r)
        {
            List<PointF> pts = new List<PointF>();
            for (int i = 0; i <= 96; i++)
            {
                double a = i * Math.PI * 2 / 96;
                pts.Add(new PointF(c.X + (float)(r * Math.Cos(a)),
                                   c.Y + (float)(r * Math.Sin(a))));
            }
            return pts;
        }

        public static void RestoreCircle(FeatureLine l)
        {
            if (l == null) return;
            l.IsFittedCircle = false;
            if (l.RawPoints != null && l.RawPoints.Count > 0)
                l.Points = new List<PointF>(l.RawPoints);
        }
    }

    /// <summary>在设定矩形内抓出全部边界，小于最小面积的组当噪点丢弃；每个特征一条连续线。</summary>
    public static class FeatureExtractor
    {
        public static byte[] Smooth(Bitmap bmp, out int w, out int h)
        { return EdgeMap.Smooth(bmp, out w, out h); }

        /// <summary>
        /// 多个框选区域依次提取后合并（可连续框选多次）。
        /// 圆形框只表示「在这个圆范围内优先按边界判断是不是圆/类圆」：
        /// 用真实边界算圆度、标出最像圆的那条，输出的仍是原始边界线，
        /// 绝不自动生成规则正圆；框内没抓到边界就不输出，不造假线。
        /// </summary>
        public static List<FeatureLine> ExtractAll(Bitmap src, List<Roi> rois,
            int sensitivity, int minArea, int smoothLevel,
            out int usedThreshold, out int rejected, out List<int> filteredAreas)
        {
            usedThreshold = EdgeMap.Threshold(sensitivity);
            rejected = 0;
            filteredAreas = new List<int>();
            List<FeatureLine> all = new List<FeatureLine>();
            if (src == null || rois == null || rois.Count == 0) return all;

            foreach (Roi roi in rois)
            {
                Rectangle rc = Rectangle.Intersect(roi.Bounds,
                    new Rectangle(0, 0, src.Width, src.Height));
                if (rc.Width < 16 || rc.Height < 16) continue;
                // 圆形框做分割时把范围外扩 40px：小 crop 会把暗孔四周的亮区切掉，
                // 暗区经闭运算就撑到框边、变成圆角方块。扩开后暗孔仍是孤立圆块。
                Rectangle searchRc = roi.IsCircle
                    ? Rectangle.Intersect(Rectangle.Inflate(rc, 40, 40),
                        new Rectangle(0, 0, src.Width, src.Height))
                    : rc;
                int th, rej; List<int> flt;
                List<FeatureLine> part = Extract(src, searchRc, sensitivity, minArea,
                    smoothLevel, out th, out rej, out flt);
                usedThreshold = th;   // 报告真实的 Otsu 阈值
                rejected += rej;
                filteredAreas.AddRange(flt);

                if (!roi.IsCircle) { all.AddRange(part); continue; }

                // 只保留中心落在圆 ROI 内的线
                List<FeatureLine> inside = new List<FeatureLine>();
                foreach (FeatureLine l in part)
                {
                    float cx = l.Bounds.X + l.Bounds.Width / 2f;
                    float cy = l.Bounds.Y + l.Bounds.Height / 2f;
                    float dx = cx - roi.Center.X, dy = cy - roi.Center.Y;
                    if (dx * dx + dy * dy <= roi.Radius * roi.Radius * 1.25f)
                        inside.Add(l);
                }
                // 按真实边界的圆度挑出最像圆的一条，只打判定标记，不改任何一个点
                int best = -1;
                double bestC = -1;
                for (int i = 0; i < inside.Count; i++)
                {
                    double c = EdgeMap.Circularity(inside[i]);
                    if (c > bestC) { bestC = c; best = i; }
                }
                if (best >= 0)
                {
                    inside[best].IsRoundCandidate = true;
                    inside[best].Roundness = bestC;
                }
                all.AddRange(inside);
            }

            // 多个框重叠会抓到同一特征：按中心 + 面积去重，保留判定信息更全的那条
            List<FeatureLine> unique = new List<FeatureLine>();
            foreach (FeatureLine l in all)
            {
                // 小框把大特征切成 1px 细条的伪影，不算一条边界。
                // 注意用 &&：大方块的四条边本身就是 1px 宽的细长线（1x143、148x1），
                // 只有宽高都小才是真正的碎片伪影，长边线必须保留。
                if (l.Bounds.Width < 6 && l.Bounds.Height < 6) continue;
                float cx = l.Bounds.X + l.Bounds.Width / 2f;
                float cy = l.Bounds.Y + l.Bounds.Height / 2f;
                bool dup = false;
                for (int i = 0; i < unique.Count; i++)
                {
                    FeatureLine u = unique[i];
                    float ux = u.Bounds.X + u.Bounds.Width / 2f;
                    float uy = u.Bounds.Y + u.Bounds.Height / 2f;
                    float ddx = cx - ux, ddy = cy - uy;
                    double areaDiff = Math.Abs(l.Area - u.Area)
                        / (double)Math.Max(1, Math.Max(l.Area, u.Area));
                    if (ddx * ddx + ddy * ddy < 324.0 && areaDiff < 0.5)
                    {
                        // 手动拟合的正圆 > 圆形框按边界判定的类圆 > 普通边界
                        int lp = l.IsFittedCircle ? 2 : (l.IsRoundCandidate ? 1 : 0);
                        int up = u.IsFittedCircle ? 2 : (u.IsRoundCandidate ? 1 : 0);
                        if (lp > up) unique[i] = l;
                        dup = true;
                        break;
                    }
                }
                if (!dup) unique.Add(l);
            }
            return unique;
        }

        public static List<FeatureLine> Extract(Bitmap src, Rectangle rect,
            int sensitivity, int minArea, int smoothLevel, out int usedThreshold,
            out int rejected, out List<int> filteredAreas)
        {
            usedThreshold = EdgeMap.Threshold(sensitivity);
            rejected = 0;
            filteredAreas = new List<int>();
            List<FeatureLine> result = new List<FeatureLine>();
            if (src == null || rect.Width < 16 || rect.Height < 16) return result;
            rect = Rectangle.Intersect(rect, new Rectangle(0, 0, src.Width, src.Height));
            if (rect.Width < 16 || rect.Height < 16) return result;

            using (Bitmap crop = src.Clone(rect, PixelFormat.Format24bppRgb))
            {
                int w, h;
                byte[] sm = EdgeMap.Smooth(crop, out w, out h);
                usedThreshold = EdgeMap.Threshold(sensitivity, sm);
                int loGray, hiGray;
                EdgeMap.EstimateLevels(sm, out loGray, out hiGray);
                bool[] mask = EdgeMap.Segment(sm, w, h, sensitivity);
                int[] label;
                int groups = EdgeMap.Label(mask, w, h, out label);

                List<int> count = new List<int>(); List<int> minX = new List<int>();
                List<int> minY = new List<int>(); List<int> maxX = new List<int>();
                List<int> maxY = new List<int>(); List<int> first = new List<int>();
                count.Add(0); minX.Add(0); minY.Add(0); maxX.Add(0); maxY.Add(0); first.Add(0);
                for (int i = 0; i < mask.Length; i++)
                {
                    int k = label[i];
                    if (k == 0) continue;
                    while (count.Count <= k)
                    {
                        count.Add(0); minX.Add(w); minY.Add(h); maxX.Add(0); maxY.Add(0); first.Add(i);
                    }
                    int y = i / w, x = i - y * w;
                    count[k]++;
                    if (x < minX[k]) minX[k] = x;
                    if (y < minY[k]) minY[k] = y;
                    if (x > maxX[k]) maxX[k] = x;
                    if (y > maxY[k]) maxY[k] = y;
                    if (y < first[k] / w || (y == first[k] / w && x < first[k] % w)) first[k] = i;
                }

                for (int k = 1; k <= groups; k++)
                {
                    if (count[k] < minArea)   // 小噪点不选出来
                    {
                        rejected++;
                        if (count[k] >= 300) filteredAreas.Add(count[k]);
                        continue;
                    }
                    if (count[k] > 8000)
                    {
                        int bw = maxX[k] - minX[k] + 1;
                        int bh = maxY[k] - minY[k] + 1;
                        bool squat = (double)bw / Math.Max(1, bh) >= 0.5
                                  && (double)bw / Math.Max(1, bh) <= 2.0;
                        if (squat)
                        {
                            // 接近方形的大块：输出「完整闭合轮廓」。
                            // 旧做法是逐行取最左/最右点拆成四条边，但线条一旦倾斜（工件旋转 2°），
                            // 逐行取极值会把细线放大成粗带（模板上边 148x1 → 旋转后 152x71），
                            // 形状相似度骤降到 0.6 以下 → 几何比对直接判不匹配。
                            // 完整轮廓在旋转后仍然自相似，稳定得多。
                            List<PointF> bigChain = EdgeMap.RemoveSpurs(
                                EdgeMap.Trace(label, w, h, k, first[k]));
                            bigChain = EdgeMap.SmoothLine(bigChain, smoothLevel);
                            if (bigChain.Count >= 20)
                            {
                                bigChain = EdgeMap.RefineEdge(sm, w, h, bigChain, loGray, hiGray);
                                FeatureLine bigFl = new FeatureLine();
                                bigFl.Area = count[k];
                                bigFl.Bounds = new Rectangle(minX[k] + rect.X, minY[k] + rect.Y, bw, bh);
                                foreach (PointF q in bigChain)
                                    bigFl.Points.Add(new PointF(q.X + rect.X, q.Y + rect.Y));
                                result.Add(bigFl);
                            }
                            // 亮块内部的暗孔（例如圆孔）也抓成一条真实边界线，不预设成圆
                            AddHoles(result, sm, minX[k], minY[k], maxX[k], maxY[k],
                                w, h, sensitivity, minArea, rect, smoothLevel,
                                ref rejected, filteredAreas, loGray, hiGray);
                            continue;
                        }
                        // 细长块：保持原左右双边逻辑
                        List<PointF> leftEdge = new List<PointF>();
                        List<PointF> rightEdge = new List<PointF>();
                        for (int y = minY[k]; y <= maxY[k]; y++)
                        {
                            int lo = -1, hi = -1;
                            for (int x = minX[k]; x <= maxX[k]; x++)
                            {
                                if (label[y * w + x] != k) continue;
                                if (lo < 0) lo = x;
                                hi = x;
                            }
                            if (lo < 0) continue;
                            leftEdge.Add(new PointF(lo + rect.X, y + rect.Y));
                            rightEdge.Add(new PointF(hi + rect.X, y + rect.Y));
                        }
                        AddLine(result, leftEdge, count[k], smoothLevel, sm, w, h, rect, loGray, hiGray);
                        AddLine(result, rightEdge, count[k], smoothLevel, sm, w, h, rect, loGray, hiGray);
                        // 亮块内部的暗孔（例如圆孔）也抓成一条真实边界线，不预设成圆
                        AddHoles(result, sm, minX[k], minY[k], maxX[k], maxY[k],
                            w, h, sensitivity, minArea, rect, smoothLevel,
                            ref rejected, filteredAreas, loGray, hiGray);
                        continue;
                    }
                    List<PointF> chain = EdgeMap.RemoveSpurs(
                        EdgeMap.Trace(label, w, h, k, first[k]));
                    chain = EdgeMap.SmoothLine(chain, smoothLevel);
                    if (chain.Count < 20) continue;
                    // 亚像素精修：把轮廓点从半亮过渡像素推到灰度中点，贴合白色实体边缘
                    chain = EdgeMap.RefineEdge(sm, w, h, chain, loGray, hiGray);
                    FeatureLine fl = new FeatureLine();
                    fl.Area = count[k];
                    fl.Bounds = new Rectangle(minX[k] + rect.X, minY[k] + rect.Y,
                        maxX[k] - minX[k] + 1, maxY[k] - minY[k] + 1);
                    foreach (PointF q in chain)
                        fl.Points.Add(new PointF(q.X + rect.X, q.Y + rect.Y));
                    result.Add(fl);
                }

            }
            return result;
        }

        private static void AddLine(List<FeatureLine> result, List<PointF> pts, int area, int smoothLevel,
            byte[] sm, int w, int h, Rectangle rect, int loGray, int hiGray)
        {
            if (pts.Count < 16) return;
            // pts 带 rect 偏移（全图坐标）；转回 crop 坐标做亚像素精修后再加回偏移
            List<PointF> cropPts = new List<PointF>(pts.Count);
            foreach (PointF q in pts) cropPts.Add(new PointF(q.X - rect.X, q.Y - rect.Y));
            cropPts = EdgeMap.RefineEdge(sm, w, h, cropPts, loGray, hiGray);
            List<PointF> shaped = EdgeMap.SmoothLine(cropPts, smoothLevel);
            FeatureLine fl = new FeatureLine();
            fl.Area = area;
            fl.Points = new List<PointF>(shaped.Count);
            foreach (PointF q in shaped) fl.Points.Add(new PointF(q.X + rect.X, q.Y + rect.Y));
            float x0 = float.MaxValue, y0 = float.MaxValue, x1 = float.MinValue, y1 = float.MinValue;
            foreach (PointF q in pts)
            {
                if (q.X < x0) x0 = q.X;
                if (q.Y < y0) y0 = q.Y;
                if (q.X > x1) x1 = q.X;
                if (q.Y > y1) y1 = q.Y;
            }
            fl.Bounds = new Rectangle((int)x0, (int)y0, (int)(x1 - x0 + 1), (int)(y1 - y0 + 1));
            result.Add(fl);
        }

        /// <summary>
        /// 在最大亮块范围内找「实心暗孔」，按真实轮廓抓成连续线。
        /// 用填充率区分：圆孔约 0.75，铺满整框的背景暗块约 0.65；不预设成圆。
        /// </summary>
        private static void AddHoles(List<FeatureLine> result, byte[] sm,
            int minX, int minY, int maxX, int maxY, int w, int h,
            int sensitivity, int minArea, Rectangle rect, int smoothLevel,
            ref int rejected, List<int> filteredAreas, int loGray, int hiGray)
        {
            int darkT = EdgeMap.HoleThreshold(sensitivity);
            bool[] raw = new bool[w * h];
            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                    raw[y * w + x] = sm[y * w + x] < darkT;
            bool[] closed = EdgeMap.CloseMask(raw, w, h);
            bool[] dark = new bool[w * h];
            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                    dark[y * w + x] = closed[y * w + x];

            int[] lab;
            int groups = EdgeMap.Label(dark, w, h, out lab);
            if (groups == 0) return;

            int[] cnt = new int[groups + 1];
            int[] bx0 = new int[groups + 1], by0 = new int[groups + 1];
            int[] bx1 = new int[groups + 1], by1 = new int[groups + 1];
            int[] first = new int[groups + 1];
            for (int k = 0; k <= groups; k++)
            { bx0[k] = w; by0[k] = h; bx1[k] = 0; by1[k] = 0; first[k] = -1; }

            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                {
                    int i = y * w + x;
                    int k = lab[i];
                    if (k == 0) continue;
                    cnt[k]++;
                    if (x < bx0[k]) bx0[k] = x;
                    if (y < by0[k]) by0[k] = y;
                    if (x > bx1[k]) bx1[k] = x;
                    if (y > by1[k]) by1[k] = y;
                    if (first[k] < 0) first[k] = i;
                }

            double boxArea = (maxX - minX + 1.0) * (maxY - minY + 1.0);
            for (int k = 1; k <= groups; k++)
            {
                if (cnt[k] < minArea)
                {
                    rejected++;
                    if (cnt[k] >= 300) filteredAreas.Add(cnt[k]);
                    continue;
                }
                int bw = bx1[k] - bx0[k] + 1, bh = by1[k] - by0[k] + 1;
                if (bw < 12 || bh < 12) continue;
                // 圆/椭圆填充率约 0.75，铺满框的背景暗块约 0.65：
                // 用填充率而不是面积占比，大小两种框都能正确区分（小圆框里圆孔占框 38%）
                if (cnt[k] / (double)(bw * bh) < 0.70) continue;
                if (cnt[k] > boxArea * 0.90) continue;   // 整框都是暗，不是孔
                List<PointF> chain = EdgeMap.RemoveSpurs(
                    EdgeMap.Trace(lab, w, h, k, first[k]));
                chain = EdgeMap.SmoothLine(chain, smoothLevel);
                if (chain.Count < 16) continue;
                // 孔的链是 crop 坐标，先亚像素精修（孔边界推到孔/亮块灰度中点）再加偏移
                int holeLo = loGray - 10; if (holeLo < 0) holeLo = 0;
                chain = EdgeMap.RefineEdge(sm, w, h, chain, holeLo, hiGray);
                FeatureLine fl = new FeatureLine();
                fl.Area = cnt[k];
                fl.Bounds = new Rectangle(bx0[k] + rect.X, by0[k] + rect.Y, bw, bh);
                foreach (PointF q in chain)
                    fl.Points.Add(new PointF(q.X + rect.X, q.Y + rect.Y));
                result.Add(fl);
            }
        }

    }

    /// <summary>只有绿色位置特征参与匹配；黄色只做"必须存在"检查，紫色完全忽略。</summary>
    public static class Matcher
    {
        public static MatchResult Match(Bitmap product, TemplateData tpl)
        { return Match(product, tpl, 60); }

        /// <summary>threshold = 相似度下限；低于它就判「未找到」，不再叠加模板线。</summary>
        public static MatchResult Match(Bitmap product, TemplateData tpl, double threshold)
        {
            MatchResult r = new MatchResult();
            r.Threshold = threshold;
            if (product == null || tpl == null) return r;
            using (Bitmap p24 = new Bitmap(product.Width, product.Height, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage(p24)) g.DrawImage(product, 0, 0);
                int w, h;
                byte[] sm = EdgeMap.Smooth(p24, out w, out h);
                bool[] mask = EdgeMap.Segment(sm, w, h, tpl.Sensitivity);
                bool[] dark = EdgeMap.DarkMask(sm, w, h, tpl.Sensitivity);
                // 亮特征边界 + 暗区边界（圆孔轮廓）取并集。
                // 注意不能先把两者并成一个掩膜再取边界，那样暗孔与亮块的交接处会互相抵消。
                bool[] target = new bool[w * h];
                for (int y = 1; y < h - 1; y++)
                {
                    for (int x = 1; x < w - 1; x++)
                    {
                        int i = y * w + x;
                        if (mask[i] && (!mask[i - 1] || !mask[i + 1]
                            || !mask[i - w] || !mask[i + w])) target[i] = true;
                        if (dark[i] && (!dark[i - 1] || !dark[i + 1]
                            || !dark[i - w] || !dark[i + w])) target[i] = true;
                    }
                }

                List<PointF> green = new List<PointF>();
                foreach (FeatureLine l in tpl.Lines)
                {
                    if (l.Role != FeatureRole.Position) continue;
                    green.AddRange(l.Points);
                }
                if (green.Count == 0) { r.Score = 0; return r; }
                if (green.Count > 1500)
                {
                    List<PointF> small = new List<PointF>();
                    int step = green.Count / 1500 + 1;
                    for (int i = 0; i < green.Count; i += step) small.Add(green[i]);
                    green = small;
                }

                // 分层搜索「角度 + 平移」：粗搜用抽稀点和大步长，再在最佳点附近细化。
                // 暴力遍历 角度×61×61×1500 太慢，分两层后约 200 万次判定，毫秒级。
                PointF ctr = tpl.PositionCenter;
                List<PointF> coarse = Subsample(green, 400);
                double bestAngle = 0, best = -1;
                int bestDx = 0, bestDy = 0;

                for (int ai = -5; ai <= 5; ai++)       // -10° ~ +10°，步长 2°
                {
                    double ang = ai * 2.0;
                    List<PointF> rot = Rotate(coarse, ctr, ang);
                    // 位移搜索范围 ±80px。粗搜步长8（误差≤4px 交给精搜补），
                    // 既覆盖大偏移又控制耗时；原来 ±30px/步长3 时偏移>30px 就搜不到正确位置，
                    // 导致"明明只是平移，相似度却暴跌"。
                    for (int dy = -80; dy <= 80; dy += 8)
                        for (int dx = -80; dx <= 80; dx += 8)
                        {
                            double sc = HitScore(rot, target, w, h, dx, dy);
                            if (sc > best) { best = sc; bestAngle = ang; bestDx = dx; bestDy = dy; }
                        }
                }

                double a0 = bestAngle;
                int x0 = bestDx, y0 = bestDy;
                best = -1;
                for (int ai = -4; ai <= 4; ai++)       // 最佳角度 ±2°，步长 0.5°
                {
                    double ang = a0 + ai * 0.5;
                    List<PointF> rot = Rotate(green, ctr, ang);
                    for (int dy = y0 - 8; dy <= y0 + 8; dy++)
                        for (int dx = x0 - 8; dx <= x0 + 8; dx++)
                        {
                            double sc = HitScore(rot, target, w, h, dx, dy);
                            if (sc > best) { best = sc; bestAngle = ang; bestDx = dx; bestDy = dy; }
                        }
                }

                r.PixelDx = bestDx;
                r.PixelDy = bestDy;
                r.Angle = bestAngle;
                r.DetX = bestDx * tpl.ScaleX;
                r.DetY = bestDy * tpl.ScaleY;
                r.Score = Math.Max(0, Math.Min(100, best));

                // 黄色必要特征：用户标黄的真实边界线上，在偏移后是否仍能命中产品图边界
                List<FeatureLine> yellows = tpl.YellowLines;
                if (yellows.Count == 0)
                {
                    r.AttachedFound = true;
                }
                else
                {
                    int hit = 0, total = 0;
                    foreach (FeatureLine l in yellows)
                    {
                        // 必要特征也要跟着检测到的角度一起转
                        List<PointF> rp = Rotate(l.Points, ctr, bestAngle);
                        foreach (PointF p in rp)
                        {
                            total++;
                            int x = (int)p.X + bestDx, y = (int)p.Y + bestDy;
                            if (x < 1 || y < 1 || x >= w - 1 || y >= h - 1) continue;
                            // 必要特征只判断「在不在」，允许成像差异造成的 3px 轮廓偏差
                            if (NearTarget(target, w, h, x, y, 3)) hit++;
                        }
                    }
                    r.AttachedScore = total > 0 ? hit * 100.0 / total : 0;
                    r.AttachedFound = r.AttachedScore > 35;
                }
            }
            bool hasRequired = tpl.CountByRole(FeatureRole.Attached) > 0;
            r.Found = r.Score >= threshold && (!hasRequired || r.AttachedFound);
            return r;
        }

        private static List<PointF> Subsample(List<PointF> src, int max)
        {
            if (src.Count <= max) return src;
            List<PointF> r = new List<PointF>();
            int step = src.Count / max + 1;
            for (int i = 0; i < src.Count; i += step) r.Add(src[i]);
            return r;
        }

        /// <summary>把样板特征点绕旋转中心转 deg 度（正值逆时针）。</summary>
        private static List<PointF> Rotate(List<PointF> src, PointF c, double deg)
        {
            double a = deg * Math.PI / 180.0;
            float cs = (float)Math.Cos(a), sn = (float)Math.Sin(a);
            List<PointF> r = new List<PointF>(src.Count);
            foreach (PointF p in src)
            {
                float rx = p.X - c.X, ry = p.Y - c.Y;
                r.Add(new PointF(c.X + rx * cs - ry * sn,
                                 c.Y + rx * sn + ry * cs));
            }
            return r;
        }

        private static double HitScore(List<PointF> pts, bool[] target, int w, int h,
            int dx, int dy)
        {
            int hit = 0;
            foreach (PointF p in pts)
            {
                int x = (int)p.X + dx, y = (int)p.Y + dy;
                if (x < 1 || y < 1 || x >= w - 1 || y >= h - 1) continue;
                int i = y * w + x;
                if (target[i] || target[i + 1] || target[i - 1]
                    || target[i + w] || target[i - w]) hit++;
            }
            return pts.Count > 0 ? hit * 100.0 / pts.Count : 0;
        }

        /// <summary>在 tol 像素邻域内找边界，容忍成像/曝光带来的轮廓微小差异。</summary>
        private static bool NearTarget(bool[] target, int w, int h, int x, int y, int tol)
        {
            for (int dy = -tol; dy <= tol; dy++)
                for (int dx = -tol; dx <= tol; dx++)
                {
                    int xx = x + dx, yy = y + dy;
                    if (xx < 1 || yy < 1 || xx >= w - 1 || yy >= h - 1) continue;
                    if (target[yy * w + xx]) return true;
                }
            return false;
        }
    }

    /// <summary>
    /// 几何/形状比对：产品图用同一组参数独立抓边界，再与样板绿线做形状匹配，
    /// 位移投票排除错配，用线的 PCA 主轴方向求旋转角。不依赖像素边界命中率。
    /// </summary>
    public static class GeometryMatcher
    {
        private sealed class Pair
        {
            public PointF A, B;            // 样板绿线质心 / 产品线质心
            public double Sim;
            public int ShiftX, ShiftY;     // 量化位移，用于投票
            public double DirA, DirB;      // PCA 主轴方向（度）
            public double CircA;           // 圆形线主轴不稳，求角度时跳过
        }

        private static PointF Center(FeatureLine l)
        {
            return new PointF(l.Bounds.X + l.Bounds.Width / 2f,
                              l.Bounds.Y + l.Bounds.Height / 2f);
        }

        /// <summary>PCA 主轴方向（度，-90~90）；两条斜边质心共线时只有它能反映旋转。</summary>
        private static double PrincipalAngle(FeatureLine l)
        {
            int n = l.Points.Count;
            if (n < 3) return 0;
            double sx = 0, sy = 0;
            foreach (PointF p in l.Points) { sx += p.X; sy += p.Y; }
            sx /= n; sy /= n;
            double sxx = 0, syy = 0, sxy = 0;
            foreach (PointF p in l.Points)
            {
                double dx = p.X - sx, dy = p.Y - sy;
                sxx += dx * dx; syy += dy * dy; sxy += dx * dy;
            }
            return 0.5 * Math.Atan2(2 * sxy / n, (sxx - syy) / n) * 180.0 / Math.PI;
        }

        /// <summary>面积 / 圆形度 / 宽高比 / 尺寸 加权，0~1。</summary>
        public static double ShapeSimilarity(FeatureLine a, FeatureLine b)
        {
            double areaA = Math.Max(1, a.Area), areaB = Math.Max(1, b.Area);
            double areaSim = 1.0 - Math.Abs(areaA - areaB) / Math.Max(areaA, areaB);
            double circSim = 1.0 - Math.Abs(EdgeMap.Circularity(a) - EdgeMap.Circularity(b));
            double arA = a.Bounds.Width / (double)Math.Max(1, a.Bounds.Height);
            double arB = b.Bounds.Width / (double)Math.Max(1, b.Bounds.Height);
            double arSim = 1.0 - Math.Abs(arA - arB) / Math.Max(arA, Math.Max(arB, 0.01));
            double sizeSim = 1.0 - (Math.Abs(a.Bounds.Width - b.Bounds.Width)
                    + Math.Abs(a.Bounds.Height - b.Bounds.Height))
                / (double)Math.Max(1, a.Bounds.Width + b.Bounds.Width
                    + a.Bounds.Height + b.Bounds.Height);
            double s = 0.32 * areaSim + 0.30 * circSim + 0.20 * arSim + 0.18 * sizeSim;
            return Math.Max(0, Math.Min(1, s));
        }

        public static MatchResult Compare(TemplateData tpl,
            List<FeatureLine> productLines, double threshold)
        {
            MatchResult r = new MatchResult();
            r.Threshold = threshold;
            if (tpl == null || productLines == null || productLines.Count == 0) return r;

            List<FeatureLine> green = new List<FeatureLine>();
            foreach (FeatureLine l in tpl.Lines)
                if (l.Role == FeatureRole.Position) green.Add(l);
            List<FeatureLine> yellows = tpl.YellowLines;
            if (green.Count == 0) return r;

            // 1) 候选配对；同时记下每条绿线能找到的最佳形状相似度
            double[] bestSim = new double[green.Count];
            List<Pair> cand = new List<Pair>();
            for (int gi = 0; gi < green.Count; gi++)
            {
                FeatureLine g = green[gi];
                PointF ca = Center(g);
                foreach (FeatureLine p in productLines)
                {
                    double s = ShapeSimilarity(g, p);
                    if (s > bestSim[gi]) bestSim[gi] = s;
                    if (s < 0.70) continue;
                    PointF cb = Center(p);
                    Pair pr = new Pair();
                    pr.A = ca; pr.B = cb; pr.Sim = s;
                    pr.DirA = PrincipalAngle(g);
                    pr.DirB = PrincipalAngle(p);
                    pr.CircA = EdgeMap.Circularity(g);
                    pr.ShiftX = (int)Math.Round((cb.X - ca.X) / 4.0);
                    pr.ShiftY = (int)Math.Round((cb.Y - ca.Y) / 4.0);
                    cand.Add(pr);
                }
            }
            // 必要特征与整体位置是否对齐无关（纯按形状独立匹配），
            // 必须放在后面几个 early return 之前算好；否则大旋转/大偏移导致配对为空时
            // 会直接返回，把「圆孔明明抓到了」误报成「缺失」。
            ComputeAttached(tpl, productLines, r);

            if (cand.Count == 0) return r;

            // 2) 位移投票：左右斜边形状几乎一样，靠一致的位移排除错配
            Dictionary<Point, List<Pair>> bins = new Dictionary<Point, List<Pair>>();
            foreach (Pair pr in cand)
            {
                Point key = new Point(pr.ShiftX, pr.ShiftY);
                List<Pair> lst;
                if (!bins.TryGetValue(key, out lst))
                { lst = new List<Pair>(); bins[key] = lst; }
                lst.Add(pr);
            }
            List<Pair> bestBin = null;
            Point bestKey = Point.Empty;
            foreach (KeyValuePair<Point, List<Pair>> kv in bins)
            {
                bool take = bestBin == null || kv.Value.Count > bestBin.Count;
                if (!take && kv.Value.Count == bestBin.Count)
                {
                    int d1 = Math.Abs(kv.Key.X) + Math.Abs(kv.Key.Y);
                    int d2 = Math.Abs(bestKey.X) + Math.Abs(bestKey.Y);
                    take = d1 < d2;
                }
                if (take) { bestBin = kv.Value; bestKey = kv.Key; }
            }
            if (bestBin == null) return r;

            // 把最佳位移箱「邻域」（±1 格 ≈ ±4px）的配对也纳入候选。
            // 工件旋转或轻微缩放时，各特征的位移会差几个像素，严格只取单箱
            // 会把大部分绿线甩掉，只剩 1 条 → 角度只能退化为 PCA 主轴，
            // 而方形轮廓的主轴有 90° 歧义，读数会乱跳（例如旋转-2° 报成 -23°）。
            List<Pair> pool = new List<Pair>();
            foreach (KeyValuePair<Point, List<Pair>> kv in bins)
                if (Math.Abs(kv.Key.X - bestKey.X) <= 1 && Math.Abs(kv.Key.Y - bestKey.Y) <= 1)
                    pool.AddRange(kv.Value);

            // 3) 每条绿线只保留一个最佳配对
            Dictionary<int, Pair> perGreen = new Dictionary<int, Pair>();
            for (int gi = 0; gi < green.Count; gi++)
            {
                PointF ga = Center(green[gi]);
                Pair best = null;
                foreach (Pair pr in pool)
                {
                    if (Math.Abs(pr.A.X - ga.X) > 0.5f
                        || Math.Abs(pr.A.Y - ga.Y) > 0.5f) continue;
                    if (best == null || pr.Sim > best.Sim) best = pr;
                }
                if (best != null) perGreen[gi] = best;
            }
            if (perGreen.Count == 0) return r;

            // 4) 旋转角估计：优先用「两两绿线质心连线」的方向差投票。
            //    质心连线随刚体旋转一起转，比 PCA 主轴准得多（方形轮廓的主轴有 90° 歧义）。
            //    绿线只有 1 条时没有连线可用，退回到 PCA 主轴差。
            int n = perGreen.Count;
            double mxA = 0, myA = 0, mxB = 0, myB = 0;
            List<double> angles = new List<double>();
            List<Pair> pv = new List<Pair>(perGreen.Values);
            foreach (Pair pr in pv)
            {
                mxA += pr.A.X; myA += pr.A.Y; mxB += pr.B.X; myB += pr.B.Y;
            }
            for (int i = 0; i < pv.Count; i++)
                for (int j = i + 1; j < pv.Count; j++)
                {
                    double ta = Math.Atan2(pv[j].A.Y - pv[i].A.Y, pv[j].A.X - pv[i].A.X);
                    double tb = Math.Atan2(pv[j].B.Y - pv[i].B.Y, pv[j].B.X - pv[i].B.X);
                    double d = (tb - ta) * 180.0 / Math.PI;
                    while (d > 90) d -= 180;
                    while (d < -90) d += 180;
                    angles.Add(d);
                }
            if (angles.Count == 0)   // 单条绿线：只能用主轴方向
            {
                foreach (Pair pr in pv)
                {
                    if (pr.CircA > 0.92) continue;
                    double d = pr.DirB - pr.DirA;
                    while (d > 90) d -= 180;
                    while (d < -90) d += 180;
                    angles.Add(d);
                }
            }
            mxA /= n; myA /= n; mxB /= n; myB /= n;
            double deg = 0;
            if (angles.Count > 0)
            {
                // 角度投票：形状相似的特征（如两条长条）可能配错，中位数会被带偏。
                // 用 1° 分桶找「共识最多」的角度区间，再取该区间内的中位数。
                int[] votes = new int[182];
                foreach (double a in angles)
                {
                    int b = (int)Math.Round(a) + 90;
                    if (b >= 0 && b <= 181) votes[b]++;
                }
                int peak = 0, peakCnt = -1;
                for (int b = 1; b < 181; b++)
                {
                    int c = votes[b - 1] + votes[b] + votes[b + 1];   // ±1° 平滑
                    if (c > peakCnt) { peakCnt = c; peak = b; }
                }
                List<double> near = new List<double>();
                foreach (double a in angles)
                    if (Math.Abs(a - (peak - 90)) <= 2.5) near.Add(a);
                if (near.Count > 0) { near.Sort(); deg = near[near.Count / 2]; }
                else { angles.Sort(); deg = angles[angles.Count / 2]; }
            }
            // 角度范围放宽到 ±45°（原来 ±10°，25°/40° 的旋转根本测不出来）。
            if (deg > 45) deg = 45;
            if (deg < -45) deg = -45;
            double theta = deg * Math.PI / 180.0;
            double cs = Math.Cos(theta), sn = Math.Sin(theta);

            // 界面 Stroke 是「绕 ctr 旋转 + 平移 d」；令变换后绿线质心均值落到产品侧均值
            PointF ctr = tpl.PositionCenter;
            double ax = mxA - ctr.X, ay = myA - ctr.Y;
            double dx = mxB - ctr.X - (cs * ax - sn * ay);
            double dy = myB - ctr.Y - (sn * ax + cs * ay);
            r.PixelDx = (int)Math.Round(dx);
            r.PixelDy = (int)Math.Round(dy);
            r.Angle = deg;
            r.DetX = r.PixelDx * tpl.ScaleX;
            r.DetY = r.PixelDy * tpl.ScaleY;

            // 5) 相似度 = 每条绿线「最佳形状匹配」的平均。
            //    产品图独立分割可能出现粘连/断裂，不能按严格 1:1 配对数计分。
            //    原来「相似度<0.80 直接不计」会让大孔这类"面积变了但形状还在"的工件直接 0 分，
            //    分数失去区分度。改为按实际相似度累加，只把明显无关（<0.40）的当 0。
            double qualSum = 0;
            for (int i = 0; i < bestSim.Length; i++)
            {
                double s = bestSim[i];
                if (s < 0.40) s = 0;
                qualSum += s;
            }
            r.Score = Math.Max(0, Math.Min(100, 100.0 * qualSum / green.Count));

            // 6) 必要特征在函数开头已算出（ComputeAttached），此处不再重复
            r.Found = r.Score >= threshold
                && (yellows.Count == 0 || r.AttachedFound);
            // 注意：这里不再把 AttachedFound 清零。
            // 必要特征是「形状独立匹配」得出的（不依赖整体偏移是否对齐），
            // 若因大偏移/大旋转导致整体未命中就清零，会把「圆孔明明在图上」硬说成「缺失」，
            // 反而掩盖了真实原因（是位置偏了，不是特征没了）。判定仍由 r.Found 决定。
            return r;
        }

        /// <summary>
        /// 必要特征（黄色）判定：只看「产品图里有没有形状相符的线」，与整体偏移/旋转是否对齐无关。
        /// 圆形必要特征不允许用方块凑数；一条产品线只能充当一个必要特征（一对一）。
        /// 「必须有」= 每个都得在。两种比对模式（模板/几何）都调用本方法，保证语义一致。
        /// </summary>
        public static void ComputeAttached(TemplateData tpl,
            List<FeatureLine> productLines, MatchResult r)
        {
            List<FeatureLine> yellows = tpl.YellowLines;
            if (yellows.Count == 0) { r.AttachedFound = true; }
            else
            {
                // 一对一匹配：一条产品线只能充当一个必要特征。
                // 否则「缺右长条」时，右长条会被别的相似线顶替，误判成「存在」。
                bool[] used = new bool[productLines.Count];
                int ok = 0;
                foreach (FeatureLine y in yellows)
                {
                    double yc = EdgeMap.Circularity(y);
                    double bestS = 0; int bi = -1;
                    for (int i = 0; i < productLines.Count; i++)
                    {
                        if (used[i]) continue;
                        FeatureLine p = productLines[i];
                        if (yc > 0.80 && EdgeMap.Circularity(p) < 0.85) continue;   // 圆不能拿方块凑数
                        double s = ShapeSimilarity(y, p);
                        if (s > bestS) { bestS = s; bi = i; }
                    }
                    if (bestS >= 0.80 && bi >= 0) { ok++; used[bi] = true; }
                }
                r.AttachedScore = 100.0 * ok / yellows.Count;
                // 「必须有」= 每个必要特征都得在。旧逻辑用 >35%，4个缺1个仍有75% 判「存在」，
                // 缺件产品（缺右长条/缺底座/缺圆孔）就检不出来。
                r.AttachedFound = ok == yellows.Count;
            }
            // 一致地重算「找到匹配」：相似度达标 + 必要特征齐全
            bool hasRequired = tpl.CountByRole(FeatureRole.Attached) > 0;
            r.Found = r.Score >= r.Threshold && (!hasRequired || r.AttachedFound);
        }
    }
}

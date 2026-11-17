using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace TemplateMatchDemo
{
    public static class DemoState
    {
        public static readonly List<string> Images = new List<string>();
        public static readonly TemplateData Template = new TemplateData();
        public static int CurrentIndex;
        public static string ImageDir = "";

        public static void Init()
        {
            string exe = AppDomain.CurrentDomain.BaseDirectory;
            // 只从自带的演示图目录加载（程序生成的抽象测试件）。
            // 只从程序自带的演示图目录加载，避免依赖开发机上的临时路径。
            string[] candidates = new string[]
            {
                Path.Combine(exe, "测试产品图"),
                Path.GetFullPath(Path.Combine(exe, @"..\..\..\测试产品图")),
                Path.GetFullPath(Path.Combine(exe, @"..\..\测试产品图"))
            };
            foreach (string d in candidates)
                if (Directory.Exists(d) && Directory.GetFiles(d, "*.bmp").Length > 0)
                { ImageDir = d; break; }
            Images.Clear();
            if (ImageDir.Length == 0) return;
            string[] files = Directory.GetFiles(ImageDir, "*.bmp");
            Array.Sort(files, delegate(string a, string b)
            {
                return Num(Path.GetFileNameWithoutExtension(a)).CompareTo(Num(Path.GetFileNameWithoutExtension(b)));
            });
            Images.AddRange(files);
            if (Images.Count > 0) Template.SamplePath = Images[0];
            CalibrationStore.Load(Template);
        }

        private static int Num(string s)
        {
            string d = "";
            foreach (char c in s) if (char.IsDigit(c)) d += c;
            int v;
            return int.TryParse(d, out v) ? v : 0;
        }
    }

    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DemoState.Init();
            Application.Run(new ProductMainForm());
            return 0;
        }

    }
}

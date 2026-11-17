using System;
using System.Drawing;
using System.Windows.Forms;

namespace TemplateMatchDemo
{
    /// <summary>
    /// 设计器可编辑的界面布局：所有控件均为字段，InitializeComponent 只含标准语句，
    /// 可在 VS 窗体设计器里直接查看与拖拽。业务逻辑位于 CalibrationForm.cs。
    /// </summary>
    public sealed partial class CalibrationForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            if (disposing && _image != null) { _image.Dispose(); _image = null; }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.bar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblBoard = new System.Windows.Forms.Label();
            this._boardKind = new System.Windows.Forms.ComboBox();
            this.btnMake = new System.Windows.Forms.Button();
            this.btnExportAll = new System.Windows.Forms.Button();
            this.lblMode = new System.Windows.Forms.Label();
            this._mode = new System.Windows.Forms.ComboBox();
            this._curLabel = new System.Windows.Forms.Label();
            this._status = new System.Windows.Forms.Label();
            this.right = new System.Windows.Forms.Panel();
            this.g1 = new System.Windows.Forms.GroupBox();
            this._grpRect = new System.Windows.Forms.Panel();
            this._labW = new System.Windows.Forms.Label();
            this._mmW = new System.Windows.Forms.NumericUpDown();
            this._labH = new System.Windows.Forms.Label();
            this._mmH = new System.Windows.Forms.NumericUpDown();
            this._grpRuler = new System.Windows.Forms.Panel();
            this.lblLen = new System.Windows.Forms.Label();
            this._mmLen = new System.Windows.Forms.NumericUpDown();
            this._grpManual = new System.Windows.Forms.Panel();
            this.lblManX = new System.Windows.Forms.Label();
            this._manualX = new System.Windows.Forms.NumericUpDown();
            this.lblManY = new System.Windows.Forms.Label();
            this._manualY = new System.Windows.Forms.NumericUpDown();
            this.tip = new System.Windows.Forms.Label();
            this.g2 = new System.Windows.Forms.GroupBox();
            this._measureTip = new System.Windows.Forms.Label();
            this._btnAuto = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this._pxMeasure = new System.Windows.Forms.Label();
            this._autoInfo = new System.Windows.Forms.Label();
            this.g3 = new System.Windows.Forms.GroupBox();
            this._resX = new System.Windows.Forms.Label();
            this._resY = new System.Windows.Forms.Label();
            this._resDemo = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this._pic = new System.Windows.Forms.PictureBox();
            this.bar.SuspendLayout();
            this.g1.SuspendLayout();
            this._grpRect.SuspendLayout();
            this._grpRuler.SuspendLayout();
            this._grpManual.SuspendLayout();
            this.g2.SuspendLayout();
            this.g3.SuspendLayout();
            this.right.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._mmW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._mmH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._mmLen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._manualX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._manualY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pic)).BeginInit();
            this.SuspendLayout();
            // bar
            this.bar.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar.Height = 46;
            this.bar.Padding = new System.Windows.Forms.Padding(8, 8, 8, 2);
            this.bar.WrapContents = false;
            this.bar.Name = "bar";
            this.bar.Controls.Add(this.btnLoad);
            this.bar.Controls.Add(this.lblBoard);
            this.bar.Controls.Add(this._boardKind);
            this.bar.Controls.Add(this.btnMake);
            this.bar.Controls.Add(this.btnExportAll);
            this.bar.Controls.Add(this.lblMode);
            this.bar.Controls.Add(this._mode);
            this.bar.Controls.Add(this._curLabel);
            this.btnLoad.Text = "载入标定图";
            this.btnLoad.Width = 110;
            this.btnLoad.Height = 30;
            this.btnLoad.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            this.lblBoard.Text = "板型";
            this.lblBoard.SetBounds(0, 0, 40, 22);
            this.lblBoard.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this.lblBoard.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblBoard.Margin = new System.Windows.Forms.Padding(10, 7, 0, 3);
            this.lblBoard.Name = "lblBoard";
            this._boardKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._boardKind.Items.Add("标准棋盘 50px / 10mm");
            this._boardKind.Items.Add("细密棋盘 25px / 5mm");
            this._boardKind.Items.Add("粗棋盘 100px / 20mm");
            this._boardKind.Items.Add("长方格 80x50px / 16x10mm");
            this._boardKind.Items.Add("单矩形块 200x150px / 40x30mm");
            this._boardKind.Items.Add("噪声棋盘 50px / 10mm");
            this._boardKind.SelectedIndex = 0;
            this._boardKind.Width = 178;
            this._boardKind.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this._boardKind.Name = "_boardKind";
            this.btnMake.Text = "生成标定板";
            this.btnMake.Width = 100;
            this.btnMake.Height = 30;
            this.btnMake.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnMake.Name = "btnMake";
            this.btnMake.Click += new System.EventHandler(this.btnMake_Click);
            this.btnExportAll.Text = "导出全部板型";
            this.btnExportAll.Width = 110;
            this.btnExportAll.Height = 30;
            this.btnExportAll.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnExportAll.Name = "btnExportAll";
            this.btnExportAll.Click += new System.EventHandler(this.btnExportAll_Click);
            this.lblMode.Text = "标定方式";
            this.lblMode.SetBounds(0, 0, 62, 22);
            this.lblMode.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this.lblMode.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblMode.Margin = new System.Windows.Forms.Padding(12, 7, 0, 3);
            this.lblMode.Name = "lblMode";
            this._mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._mode.Width = 150;
            this._mode.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this._mode.Items.Add("已知矩形（X+Y）");
            this._mode.Items.Add("水平标尺（仅 X）");
            this._mode.Items.Add("垂直标尺（仅 Y）");
            this._mode.Items.Add("手动输入 mm/px");
            this._mode.Items.Add("自动：标定板矩形");
            this._mode.Items.Add("自动：多矩形融合");
            this._mode.SelectedIndex = 0;
            this._mode.Name = "_mode";
            this._mode.SelectedIndexChanged += new System.EventHandler(this._mode_SelectedIndexChanged);
            this._curLabel.Text = "";
            this._curLabel.SetBounds(0, 0, 300, 22);
            this._curLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f, System.Drawing.FontStyle.Bold);
            this._curLabel.ForeColor = System.Drawing.Color.DarkGreen;
            this._curLabel.Margin = new System.Windows.Forms.Padding(16, 7, 0, 3);
            this._curLabel.Name = "_curLabel";
            // _status
            this._status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._status.Height = 30;
            this._status.BackColor = System.Drawing.Color.FromArgb(235, 240, 246);
            this._status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._status.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this._status.Name = "_status";
            // right
            this.right.Dock = System.Windows.Forms.DockStyle.Right;
            this.right.Width = 268;
            this.right.BackColor = System.Drawing.Color.FromArgb(243, 247, 251);
            this.right.AutoScroll = true;
            this.right.Name = "right";
            this.right.Controls.Add(this.g1);
            this.right.Controls.Add(this.g2);
            this.right.Controls.Add(this.g3);
            this.right.Controls.Add(this.btnApply);
            this.right.Controls.Add(this.btnCancel);
            // g1
            this.g1.Text = "① 已知实际尺寸";
            this.g1.SetBounds(8, 8, 246, 176);
            this.g1.Name = "g1";
            this.g1.Controls.Add(this._grpRect);
            this.g1.Controls.Add(this._grpRuler);
            this.g1.Controls.Add(this._grpManual);
            this.g1.Controls.Add(this.tip);
            // _grpRect
            this._grpRect.SetBounds(8, 18, 228, 76);
            this._grpRect.Name = "_grpRect";
            this._grpRect.Controls.Add(this._labW);
            this._grpRect.Controls.Add(this._mmW);
            this._grpRect.Controls.Add(this._labH);
            this._grpRect.Controls.Add(this._mmH);
            this._labW.Text = "实际宽度 (mm)";
            this._labW.SetBounds(6, 6, 120, 22);
            this._labW.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this._labW.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._labW.Name = "_labW";
            this._mmW.SetBounds(132, 4, 88, 24);
            this._mmW.Minimum = 0.001M;
            this._mmW.Maximum = 100000M;
            this._mmW.Value = 10M;
            this._mmW.DecimalPlaces = 3;
            this._mmW.Increment = 0.5M;
            this._mmW.ThousandsSeparator = false;
            this._mmW.Name = "_mmW";
            this._mmW.ValueChanged += new System.EventHandler(this.Recalc_Event);
            this._labH.Text = "实际高度 (mm)";
            this._labH.SetBounds(6, 40, 120, 22);
            this._labH.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this._labH.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._labH.Name = "_labH";
            this._mmH.SetBounds(132, 38, 88, 24);
            this._mmH.Minimum = 0.001M;
            this._mmH.Maximum = 100000M;
            this._mmH.Value = 10M;
            this._mmH.DecimalPlaces = 3;
            this._mmH.Increment = 0.5M;
            this._mmH.ThousandsSeparator = false;
            this._mmH.Name = "_mmH";
            this._mmH.ValueChanged += new System.EventHandler(this.Recalc_Event);
            // _grpRuler
            this._grpRuler.SetBounds(8, 18, 228, 50);
            this._grpRuler.Name = "_grpRuler";
            this._grpRuler.Controls.Add(this.lblLen);
            this._grpRuler.Controls.Add(this._mmLen);
            this.lblLen.Text = "实际长度 (mm)";
            this.lblLen.SetBounds(6, 8, 120, 22);
            this.lblLen.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this.lblLen.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblLen.Name = "lblLen";
            this._mmLen.SetBounds(132, 6, 88, 24);
            this._mmLen.Minimum = 0.001M;
            this._mmLen.Maximum = 100000M;
            this._mmLen.Value = 10M;
            this._mmLen.DecimalPlaces = 3;
            this._mmLen.Increment = 0.5M;
            this._mmLen.ThousandsSeparator = false;
            this._mmLen.Name = "_mmLen";
            this._mmLen.ValueChanged += new System.EventHandler(this.Recalc_Event);
            // _grpManual
            this._grpManual.SetBounds(8, 18, 228, 82);
            this._grpManual.Name = "_grpManual";
            this._grpManual.Controls.Add(this.lblManX);
            this._grpManual.Controls.Add(this._manualX);
            this._grpManual.Controls.Add(this.lblManY);
            this._grpManual.Controls.Add(this._manualY);
            this.lblManX.Text = "X：mm / px";
            this.lblManX.SetBounds(6, 6, 100, 22);
            this.lblManX.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this.lblManX.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblManX.Name = "lblManX";
            this._manualX.SetBounds(110, 4, 110, 24);
            this._manualX.Minimum = 0.000001M;
            this._manualX.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this._manualX.Value = new decimal(new int[] { 0, 0, 0, 0 });
            this._manualX.DecimalPlaces = 6;
            this._manualX.Increment = 0.0001M;
            this._manualX.ThousandsSeparator = false;
            this._manualX.Name = "_manualX";
            this._manualX.ValueChanged += new System.EventHandler(this.Recalc_Event);
            this.lblManY.Text = "Y：mm / px";
            this.lblManY.SetBounds(6, 40, 100, 22);
            this.lblManY.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this.lblManY.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblManY.Name = "lblManY";
            this._manualY.SetBounds(110, 38, 110, 24);
            this._manualY.Minimum = 0.000001M;
            this._manualY.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this._manualY.Value = new decimal(new int[] { 0, 0, 0, 0 });
            this._manualY.DecimalPlaces = 6;
            this._manualY.Increment = 0.0001M;
            this._manualY.ThousandsSeparator = false;
            this._manualY.Name = "_manualY";
            this._manualY.ValueChanged += new System.EventHandler(this.Recalc_Event);
            this.tip.Text = "示例板每格 = 10 mm；现场请用真实标定件";
            this.tip.SetBounds(10, 132, 226, 38);
            this.tip.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.5f);
            this.tip.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.tip.Name = "tip";
            // g2
            this.g2.Text = "② 在图上量取";
            this.g2.SetBounds(8, 192, 246, 142);
            this.g2.Name = "g2";
            this.g2.Controls.Add(this._measureTip);
            this.g2.Controls.Add(this._btnAuto);
            this.g2.Controls.Add(this.btnClear);
            this.g2.Controls.Add(this._pxMeasure);
            this.g2.Controls.Add(this._autoInfo);
            this._measureTip.Text = "按住鼠标左键拖出已知区域";
            this._measureTip.SetBounds(10, 20, 226, 22);
            this._measureTip.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this._measureTip.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._measureTip.Name = "_measureTip";
            this._btnAuto.Text = "自动抓取矩形";
            this._btnAuto.SetBounds(10, 44, 112, 26);
            this._btnAuto.Name = "_btnAuto";
            this._btnAuto.Click += new System.EventHandler(this._btnAuto_Click);
            this.btnClear.Text = "清除测量";
            this.btnClear.SetBounds(128, 44, 100, 26);
            this.btnClear.Name = "btnClear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            this._pxMeasure.Text = "像素测量：—";
            this._pxMeasure.SetBounds(10, 76, 226, 22);
            this._pxMeasure.Font = new System.Drawing.Font("Microsoft YaHei UI", 10f, System.Drawing.FontStyle.Bold);
            this._pxMeasure.ForeColor = System.Drawing.Color.FromArgb(20, 60, 90);
            this._pxMeasure.Name = "_pxMeasure";
            this._autoInfo.Text = "尚未检测";
            this._autoInfo.SetBounds(10, 100, 226, 36);
            this._autoInfo.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.5f);
            this._autoInfo.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._autoInfo.Name = "_autoInfo";
            // g3
            this.g3.Text = "③ 标定结果 (mm / px)";
            this.g3.SetBounds(8, 342, 246, 132);
            this.g3.Name = "g3";
            this.g3.Controls.Add(this._resX);
            this.g3.Controls.Add(this._resY);
            this.g3.Controls.Add(this._resDemo);
            this._resX.Text = "ScaleX = —";
            this._resX.SetBounds(10, 24, 226, 22);
            this._resX.Font = new System.Drawing.Font("Microsoft YaHei UI", 10f, System.Drawing.FontStyle.Bold);
            this._resX.ForeColor = System.Drawing.Color.DarkGreen;
            this._resX.Name = "_resX";
            this._resY.Text = "ScaleY = —";
            this._resY.SetBounds(10, 48, 226, 22);
            this._resY.Font = new System.Drawing.Font("Microsoft YaHei UI", 10f, System.Drawing.FontStyle.Bold);
            this._resY.ForeColor = System.Drawing.Color.DarkGreen;
            this._resY.Name = "_resY";
            this._resDemo.Text = "100 px = — mm";
            this._resDemo.SetBounds(10, 76, 226, 22);
            this._resDemo.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this._resDemo.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._resDemo.Name = "_resDemo";
            // btnApply
            this.btnApply.Text = "应用到软件";
            this.btnApply.SetBounds(10, 488, 112, 36);
            this.btnApply.Font = new System.Drawing.Font("Microsoft YaHei UI", 10f, System.Drawing.FontStyle.Bold);
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(0, 170, 210);
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Name = "btnApply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // btnCancel
            this.btnCancel.Text = "取消";
            this.btnCancel.SetBounds(130, 488, 112, 36);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // _pic
            this._pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pic.BackColor = System.Drawing.Color.FromArgb(24, 28, 36);
            this._pic.Name = "_pic";
            this._pic.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintImage);
            this._pic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this._pic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this._pic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // CalibrationForm
            this.Text = "像素标定 — 模板特征比对";
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9f);
            this.Name = "CalibrationForm";
            // Dock 顺序：Fill 先 Add（占剩余空间），Top/Bottom/Right 后 Add（先切边条）。
            this.Controls.Add(this._pic);
            this.Controls.Add(this.bar);
            this.Controls.Add(this.right);
            this.Controls.Add(this._status);
            this.bar.ResumeLayout(false);
            this.g1.ResumeLayout(false);
            this._grpRect.ResumeLayout(false);
            this._grpRuler.ResumeLayout(false);
            this._grpManual.ResumeLayout(false);
            this.g2.ResumeLayout(false);
            this.g3.ResumeLayout(false);
            this.right.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._mmW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._mmH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._mmLen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._manualX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._manualY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pic)).EndInit();
            this.ResumeLayout(false);
        }

        // ---------- 由原匿名 delegate 提取的命名事件处理 ----------
        private void btnLoad_Click(object sender, EventArgs e) { LoadImage(); }
        private void btnMake_Click(object sender, EventArgs e) { MakeCheckerboard(); }
        private void btnExportAll_Click(object sender, EventArgs e) { ExportAllBoards(); }
        private void _mode_SelectedIndexChanged(object sender, EventArgs e) { SwitchMode(); }
        private void Recalc_Event(object sender, EventArgs e) { Recalc(); }
        private void _btnAuto_Click(object sender, EventArgs e)
        {
            if (_mode.SelectedIndex == 5) AutoDetectRects();
            else AutoDetectGrid();
        }
        private void btnClear_Click(object sender, EventArgs e) { ClearMeasurement(); }
        private void btnApply_Click(object sender, EventArgs e) { Apply(); }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // ---------- 本 Designer 新增控件字段 ----------
        private System.Windows.Forms.FlowLayoutPanel bar;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblBoard;
        private System.Windows.Forms.Button btnMake;
        private System.Windows.Forms.Button btnExportAll;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.Panel right;
        private System.Windows.Forms.GroupBox g1;
        private System.Windows.Forms.Label lblLen;
        private System.Windows.Forms.Label lblManX;
        private System.Windows.Forms.Label lblManY;
        private System.Windows.Forms.Label tip;
        private System.Windows.Forms.GroupBox g2;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox g3;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}

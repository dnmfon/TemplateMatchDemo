using System;
using System.Drawing;
using System.Windows.Forms;

namespace TemplateMatchDemo
{
    /// <summary>
    /// 设计器可编辑的界面布局：所有控件均为字段，InitializeComponent 只含标准语句，
    /// 可在 VS 窗体设计器里直接查看与拖拽。业务逻辑位于 TemplateEditorForm.cs。
    /// </summary>
    public sealed partial class TemplateEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            if (disposing)
            {
                if (_sample != null) { _sample.Dispose(); _sample = null; }
                if (_product != null) { _product.Dispose(); _product = null; }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.bar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnImport = new System.Windows.Forms.Button();
            this._roiShape = new System.Windows.Forms.ComboBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnWhole = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this._matchMode = new System.Windows.Forms.ComboBox();
            this.right = new System.Windows.Forms.Panel();
            this.gp = new System.Windows.Forms.GroupBox();
            this.lblSens = new System.Windows.Forms.Label();
            this._sens = new System.Windows.Forms.NumericUpDown();
            this.lblMinArea = new System.Windows.Forms.Label();
            this._minArea = new System.Windows.Forms.NumericUpDown();
            this.lblXY = new System.Windows.Forms.Label();
            this._rx = new System.Windows.Forms.NumericUpDown();
            this._ry = new System.Windows.Forms.NumericUpDown();
            this.lblWH = new System.Windows.Forms.Label();
            this._rw = new System.Windows.Forms.NumericUpDown();
            this._rh = new System.Windows.Forms.NumericUpDown();
            this.lblCalibXY = new System.Windows.Forms.Label();
            this.sx = new System.Windows.Forms.TextBox();
            this.sy = new System.Windows.Forms.TextBox();
            this.lblSmooth = new System.Windows.Forms.Label();
            this._smooth = new System.Windows.Forms.NumericUpDown();
            this.lblHint = new System.Windows.Forms.Label();
            this.gl = new System.Windows.Forms.GroupBox();
            this._lineBox = new System.Windows.Forms.ComboBox();
            this.bg = new System.Windows.Forms.Button();
            this.bp = new System.Windows.Forms.Button();
            this.by = new System.Windows.Forms.Button();
            this._btnFit = new System.Windows.Forms.Button();
            this.gr = new System.Windows.Forms.GroupBox();
            this._lblMode = new System.Windows.Forms.Label();
            this._lblFound = new System.Windows.Forms.Label();
            this._lblX = new System.Windows.Forms.Label();
            this._lblY = new System.Windows.Forms.Label();
            this._lblAngle = new System.Windows.Forms.Label();
            this._lblScore = new System.Windows.Forms.Label();
            this._lblRequired = new System.Windows.Forms.Label();
            this._status = new System.Windows.Forms.Label();
            this._pic = new System.Windows.Forms.PictureBox();
            this._batchPanel = new System.Windows.Forms.Panel();
            this.tools = new System.Windows.Forms.Panel();
            this.lblBatchScore = new System.Windows.Forms.Label();
            this._batchScore = new System.Windows.Forms.NumericUpDown();
            this.lblBatchTolXY = new System.Windows.Forms.Label();
            this._batchTolXY = new System.Windows.Forms.NumericUpDown();
            this.lblBatchTolAng = new System.Windows.Forms.Label();
            this._batchTolAng = new System.Windows.Forms.NumericUpDown();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this._batchSummary = new System.Windows.Forms.Label();
            this._grid = new System.Windows.Forms.DataGridView();
            this.gridColNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColDX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColDY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColAngle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColRequired = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColVerdict = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bar.SuspendLayout();
            this.right.SuspendLayout();
            this.gp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._sens)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._minArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._rx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._ry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._rw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._rh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._smooth)).BeginInit();
            this.gl.SuspendLayout();
            this.gr.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pic)).BeginInit();
            this._batchPanel.SuspendLayout();
            this.tools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._batchScore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._batchTolXY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._batchTolAng)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.SuspendLayout();
            // 
            // bar
            // 
            this.bar.Controls.Add(this.btnImport);
            this.bar.Controls.Add(this._roiShape);
            this.bar.Controls.Add(this.btnClear);
            this.bar.Controls.Add(this.btnWhole);
            this.bar.Controls.Add(this.btnDone);
            this.bar.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar.Location = new System.Drawing.Point(0, 0);
            this.bar.Name = "bar";
            this.bar.Padding = new System.Windows.Forms.Padding(6);
            this.bar.Size = new System.Drawing.Size(1120, 76);
            this.bar.TabIndex = 4;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(9, 9);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(120, 30);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "导入样板图";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // _roiShape
            // 
            this._roiShape.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._roiShape.Items.AddRange(new object[] {
            "矩形框选",
            "圆形框选（优先判圆）"});
            this._roiShape.Location = new System.Drawing.Point(135, 12);
            this._roiShape.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this._roiShape.Name = "_roiShape";
            this._roiShape.Size = new System.Drawing.Size(158, 25);
            this._roiShape.TabIndex = 1;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(299, 9);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 30);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "清空所有框";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnWhole
            // 
            this.btnWhole.Location = new System.Drawing.Point(405, 9);
            this.btnWhole.Name = "btnWhole";
            this.btnWhole.Size = new System.Drawing.Size(90, 30);
            this.btnWhole.TabIndex = 3;
            this.btnWhole.Text = "框住全图";
            this.btnWhole.Click += new System.EventHandler(this.btnWhole_Click);
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(501, 9);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(150, 30);
            this.btnDone.TabIndex = 4;
            this.btnDone.Text = "完成 · 返回主界面";
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // _matchMode
            // 
            this._matchMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._matchMode.Items.AddRange(new object[] {
            "比对：模板匹配",
            "比对：几何形状"});
            this._matchMode.Location = new System.Drawing.Point(0, 0);
            this._matchMode.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this._matchMode.Name = "_matchMode";
            this._matchMode.Size = new System.Drawing.Size(136, 23);
            this._matchMode.TabIndex = 0;
            // 
            // right
            // 
            this.right.BackColor = System.Drawing.SystemColors.Control;
            this.right.Controls.Add(this.gp);
            this.right.Controls.Add(this.gl);
            this.right.Controls.Add(this.gr);
            this.right.Dock = System.Windows.Forms.DockStyle.Right;
            this.right.Location = new System.Drawing.Point(848, 76);
            this.right.Name = "right";
            this.right.Size = new System.Drawing.Size(272, 664);
            this.right.TabIndex = 1;
            // 
            // gp
            // 
            this.gp.Controls.Add(this.lblSens);
            this.gp.Controls.Add(this._sens);
            this.gp.Controls.Add(this.lblMinArea);
            this.gp.Controls.Add(this._minArea);
            this.gp.Controls.Add(this.lblXY);
            this.gp.Controls.Add(this._rx);
            this.gp.Controls.Add(this._ry);
            this.gp.Controls.Add(this.lblWH);
            this.gp.Controls.Add(this._rw);
            this.gp.Controls.Add(this._rh);
            this.gp.Controls.Add(this.lblCalibXY);
            this.gp.Controls.Add(this.sx);
            this.gp.Controls.Add(this.sy);
            this.gp.Controls.Add(this.lblSmooth);
            this.gp.Controls.Add(this._smooth);
            this.gp.Controls.Add(this.lblHint);
            this.gp.Location = new System.Drawing.Point(8, 10);
            this.gp.Name = "gp";
            this.gp.Size = new System.Drawing.Size(254, 238);
            this.gp.TabIndex = 0;
            this.gp.TabStop = false;
            this.gp.Text = "抓取特征参数";
            // 
            // lblSens
            // 
            this.lblSens.Location = new System.Drawing.Point(10, 24);
            this.lblSens.Name = "lblSens";
            this.lblSens.Size = new System.Drawing.Size(150, 20);
            this.lblSens.TabIndex = 0;
            this.lblSens.Text = "灵敏度（越大越灵敏）";
            // 
            // _sens
            // 
            this._sens.Location = new System.Drawing.Point(178, 22);
            this._sens.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this._sens.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._sens.Name = "_sens";
            this._sens.Size = new System.Drawing.Size(72, 23);
            this._sens.TabIndex = 1;
            this._sens.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this._sens.ValueChanged += new System.EventHandler(this.ParamChanged);
            this._sens.Leave += new System.EventHandler(this.ParamChanged);
            // 
            // lblMinArea
            // 
            this.lblMinArea.Location = new System.Drawing.Point(10, 54);
            this.lblMinArea.Name = "lblMinArea";
            this.lblMinArea.Size = new System.Drawing.Size(155, 20);
            this.lblMinArea.TabIndex = 2;
            this.lblMinArea.Text = "最小特征面积（滤噪点）";
            // 
            // _minArea
            // 
            this._minArea.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this._minArea.Location = new System.Drawing.Point(168, 52);
            this._minArea.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this._minArea.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this._minArea.Name = "_minArea";
            this._minArea.Size = new System.Drawing.Size(82, 23);
            this._minArea.TabIndex = 3;
            this._minArea.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this._minArea.ValueChanged += new System.EventHandler(this.ParamChanged);
            this._minArea.Leave += new System.EventHandler(this.ParamChanged);
            // 
            // lblXY
            // 
            this.lblXY.Location = new System.Drawing.Point(10, 84);
            this.lblXY.Name = "lblXY";
            this.lblXY.Size = new System.Drawing.Size(88, 20);
            this.lblXY.TabIndex = 4;
            this.lblXY.Text = "框选 X / Y";
            // 
            // _rx
            // 
            this._rx.Location = new System.Drawing.Point(100, 82);
            this._rx.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this._rx.Name = "_rx";
            this._rx.Size = new System.Drawing.Size(66, 23);
            this._rx.TabIndex = 5;
            this._rx.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this._rx.ValueChanged += new System.EventHandler(this.ParamChanged);
            this._rx.Leave += new System.EventHandler(this.ParamChanged);
            // 
            // _ry
            // 
            this._ry.Location = new System.Drawing.Point(170, 82);
            this._ry.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this._ry.Name = "_ry";
            this._ry.Size = new System.Drawing.Size(80, 23);
            this._ry.TabIndex = 6;
            this._ry.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this._ry.ValueChanged += new System.EventHandler(this.ParamChanged);
            this._ry.Leave += new System.EventHandler(this.ParamChanged);
            // 
            // lblWH
            // 
            this.lblWH.Location = new System.Drawing.Point(10, 114);
            this.lblWH.Name = "lblWH";
            this.lblWH.Size = new System.Drawing.Size(88, 20);
            this.lblWH.TabIndex = 7;
            this.lblWH.Text = "框宽 / 高";
            // 
            // _rw
            // 
            this._rw.Location = new System.Drawing.Point(100, 112);
            this._rw.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this._rw.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this._rw.Name = "_rw";
            this._rw.Size = new System.Drawing.Size(66, 23);
            this._rw.TabIndex = 8;
            this._rw.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this._rw.ValueChanged += new System.EventHandler(this.ParamChanged);
            this._rw.Leave += new System.EventHandler(this.ParamChanged);
            // 
            // _rh
            // 
            this._rh.Location = new System.Drawing.Point(170, 112);
            this._rh.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this._rh.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this._rh.Name = "_rh";
            this._rh.Size = new System.Drawing.Size(80, 23);
            this._rh.TabIndex = 9;
            this._rh.Value = new decimal(new int[] {
            220,
            0,
            0,
            0});
            this._rh.ValueChanged += new System.EventHandler(this.ParamChanged);
            this._rh.Leave += new System.EventHandler(this.ParamChanged);
            // 
            // lblCalibXY
            // 
            this.lblCalibXY.Location = new System.Drawing.Point(10, 144);
            this.lblCalibXY.Name = "lblCalibXY";
            this.lblCalibXY.Size = new System.Drawing.Size(88, 20);
            this.lblCalibXY.TabIndex = 10;
            this.lblCalibXY.Text = "标定 X / Y";
            // 
            // sx
            // 
            this.sx.Location = new System.Drawing.Point(100, 142);
            this.sx.Name = "sx";
            this.sx.Size = new System.Drawing.Size(66, 23);
            this.sx.TabIndex = 11;
            this.sx.Text = "0.00500";
            this.sx.TextChanged += new System.EventHandler(this.sx_TextChanged);
            // 
            // sy
            // 
            this.sy.Location = new System.Drawing.Point(170, 142);
            this.sy.Name = "sy";
            this.sy.Size = new System.Drawing.Size(80, 23);
            this.sy.TabIndex = 12;
            this.sy.Text = "0.00402";
            this.sy.TextChanged += new System.EventHandler(this.sy_TextChanged);
            // 
            // lblSmooth
            // 
            this.lblSmooth.Location = new System.Drawing.Point(10, 174);
            this.lblSmooth.Name = "lblSmooth";
            this.lblSmooth.Size = new System.Drawing.Size(155, 20);
            this.lblSmooth.TabIndex = 13;
            this.lblSmooth.Text = "轮廓圆滑（0=原始）";
            // 
            // _smooth
            // 
            this._smooth.Location = new System.Drawing.Point(168, 172);
            this._smooth.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this._smooth.Name = "_smooth";
            this._smooth.Size = new System.Drawing.Size(82, 23);
            this._smooth.TabIndex = 14;
            this._smooth.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this._smooth.ValueChanged += new System.EventHandler(this.ParamChanged);
            this._smooth.Leave += new System.EventHandler(this.ParamChanged);
            // 
            // lblHint
            // 
            this.lblHint.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblHint.Location = new System.Drawing.Point(10, 204);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(234, 34);
            this.lblHint.TabIndex = 15;
            this.lblHint.Text = "可连续拖多个框累加；圆形框优先判圆；滤噪删线，圆滑改形状";
            // 
            // gl
            // 
            this.gl.Controls.Add(this._lineBox);
            this.gl.Controls.Add(this.bg);
            this.gl.Controls.Add(this.bp);
            this.gl.Controls.Add(this.by);
            this.gl.Controls.Add(this._btnFit);
            this.gl.Location = new System.Drawing.Point(8, 258);
            this.gl.Name = "gl";
            this.gl.Size = new System.Drawing.Size(254, 190);
            this.gl.TabIndex = 1;
            this.gl.TabStop = false;
            this.gl.Text = "边界线定义（先点图上一条线）";
            // 
            // _lineBox
            // 
            this._lineBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._lineBox.Location = new System.Drawing.Point(10, 22);
            this._lineBox.Name = "_lineBox";
            this._lineBox.Size = new System.Drawing.Size(232, 25);
            this._lineBox.TabIndex = 0;
            this._lineBox.SelectedIndexChanged += new System.EventHandler(this._lineBox_SelectedIndexChanged);
            // 
            // bg
            // 
            this.bg.ForeColor = System.Drawing.Color.Green;
            this.bg.Location = new System.Drawing.Point(10, 50);
            this.bg.Name = "bg";
            this.bg.Size = new System.Drawing.Size(232, 26);
            this.bg.TabIndex = 1;
            this.bg.Text = "绿色·判断位置";
            this.bg.Click += new System.EventHandler(this.bg_Click);
            // 
            // bp
            // 
            this.bp.ForeColor = System.Drawing.Color.Purple;
            this.bp.Location = new System.Drawing.Point(10, 80);
            this.bp.Name = "bp";
            this.bp.Size = new System.Drawing.Size(232, 26);
            this.bp.TabIndex = 2;
            this.bp.Text = "紫色·无用边界";
            this.bp.Click += new System.EventHandler(this.bp_Click);
            // 
            // by
            // 
            this.by.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(110)))), ((int)(((byte)(0)))));
            this.by.Location = new System.Drawing.Point(10, 110);
            this.by.Name = "by";
            this.by.Size = new System.Drawing.Size(232, 26);
            this.by.TabIndex = 3;
            this.by.Text = "黄色·必须有（不影响位置）";
            this.by.Click += new System.EventHandler(this.by_Click);
            // 
            // _btnFit
            // 
            this._btnFit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(160)))));
            this._btnFit.Location = new System.Drawing.Point(10, 140);
            this._btnFit.Name = "_btnFit";
            this._btnFit.Size = new System.Drawing.Size(232, 26);
            this._btnFit.TabIndex = 4;
            this._btnFit.Text = "把选中线拟合成正圆";
            this._btnFit.Click += new System.EventHandler(this._btnFit_Click);
            // 
            // gr
            // 
            this.gr.Controls.Add(this._lblMode);
            this.gr.Controls.Add(this._lblFound);
            this.gr.Controls.Add(this._lblX);
            this.gr.Controls.Add(this._lblY);
            this.gr.Controls.Add(this._lblAngle);
            this.gr.Controls.Add(this._lblScore);
            this.gr.Controls.Add(this._lblRequired);
            this.gr.Location = new System.Drawing.Point(8, 458);
            this.gr.Name = "gr";
            this.gr.Size = new System.Drawing.Size(254, 204);
            this.gr.TabIndex = 2;
            this.gr.TabStop = false;
            this.gr.Text = "比对结果";
            // 
            // _lblMode
            // 
            this._lblMode.Location = new System.Drawing.Point(10, 20);
            this._lblMode.Name = "_lblMode";
            this._lblMode.Size = new System.Drawing.Size(232, 22);
            this._lblMode.TabIndex = 0;
            this._lblMode.Text = "比对方式：—";
            // 
            // _lblFound
            // 
            this._lblFound.Location = new System.Drawing.Point(10, 44);
            this._lblFound.Name = "_lblFound";
            this._lblFound.Size = new System.Drawing.Size(232, 22);
            this._lblFound.TabIndex = 1;
            this._lblFound.Text = "匹配判定：—";
            // 
            // _lblX
            // 
            this._lblX.Location = new System.Drawing.Point(10, 68);
            this._lblX.Name = "_lblX";
            this._lblX.Size = new System.Drawing.Size(232, 22);
            this._lblX.TabIndex = 2;
            this._lblX.Text = "ΔX（偏移）：—";
            // 
            // _lblY
            // 
            this._lblY.Location = new System.Drawing.Point(10, 92);
            this._lblY.Name = "_lblY";
            this._lblY.Size = new System.Drawing.Size(232, 22);
            this._lblY.TabIndex = 3;
            this._lblY.Text = "ΔY（偏移）：—";
            // 
            // _lblAngle
            // 
            this._lblAngle.Location = new System.Drawing.Point(10, 116);
            this._lblAngle.Name = "_lblAngle";
            this._lblAngle.Size = new System.Drawing.Size(232, 22);
            this._lblAngle.TabIndex = 4;
            this._lblAngle.Text = "角度偏移：—";
            // 
            // _lblScore
            // 
            this._lblScore.Location = new System.Drawing.Point(10, 140);
            this._lblScore.Name = "_lblScore";
            this._lblScore.Size = new System.Drawing.Size(232, 22);
            this._lblScore.TabIndex = 5;
            this._lblScore.Text = "相似度：—";
            // 
            // _lblRequired
            // 
            this._lblRequired.Location = new System.Drawing.Point(10, 164);
            this._lblRequired.Name = "_lblRequired";
            this._lblRequired.Size = new System.Drawing.Size(232, 22);
            this._lblRequired.TabIndex = 6;
            this._lblRequired.Text = "必要特征：—";
            // 
            // _status
            // 
            this._status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(240)))), ((int)(((byte)(246)))));
            this._status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._status.Location = new System.Drawing.Point(0, 710);
            this._status.Name = "_status";
            this._status.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this._status.Size = new System.Drawing.Size(848, 30);
            this._status.TabIndex = 0;
            this._status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _pic
            // 
            this._pic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(48)))), ((int)(((byte)(54)))));
            this._pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pic.Location = new System.Drawing.Point(0, 76);
            this._pic.Name = "_pic";
            this._pic.Size = new System.Drawing.Size(1120, 664);
            this._pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pic.TabIndex = 3;
            this._pic.TabStop = false;
            this._pic.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintImage);
            this._pic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this._pic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this._pic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            // 
            // _batchPanel
            // 
            // 先 Fill 的 _grid，再 Top 的 tools（反了表头会被工具条盖住）
            this._batchPanel.Controls.Add(this._grid);
            this._batchPanel.Controls.Add(this.tools);
            this._batchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._batchPanel.Location = new System.Drawing.Point(0, 76);
            this._batchPanel.Name = "_batchPanel";
            this._batchPanel.Size = new System.Drawing.Size(1120, 664);
            this._batchPanel.TabIndex = 2;
            this._batchPanel.Visible = false;
            // 
            // tools
            // 
            this.tools.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(240)))), ((int)(((byte)(246)))));
            this.tools.Controls.Add(this.lblBatchScore);
            this.tools.Controls.Add(this._batchScore);
            this.tools.Controls.Add(this.lblBatchTolXY);
            this.tools.Controls.Add(this._batchTolXY);
            this.tools.Controls.Add(this.lblBatchTolAng);
            this.tools.Controls.Add(this._batchTolAng);
            this.tools.Controls.Add(this.btnExport);
            this.tools.Controls.Add(this.btnBack);
            this.tools.Controls.Add(this._batchSummary);
            this.tools.Dock = System.Windows.Forms.DockStyle.Top;
            this.tools.Location = new System.Drawing.Point(0, 0);
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(1120, 40);
            this.tools.TabIndex = 0;
            // 
            // lblBatchScore
            // 
            this.lblBatchScore.Location = new System.Drawing.Point(8, 11);
            this.lblBatchScore.Name = "lblBatchScore";
            this.lblBatchScore.Size = new System.Drawing.Size(62, 20);
            this.lblBatchScore.TabIndex = 0;
            this.lblBatchScore.Text = "相似度 ≥";
            // 
            // _batchScore
            // 
            this._batchScore.Location = new System.Drawing.Point(70, 7);
            this._batchScore.Name = "_batchScore";
            this._batchScore.Size = new System.Drawing.Size(56, 23);
            this._batchScore.TabIndex = 1;
            this._batchScore.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this._batchScore.ValueChanged += new System.EventHandler(this._batchScore_ValueChanged);
            // 
            // lblBatchTolXY
            // 
            this.lblBatchTolXY.Location = new System.Drawing.Point(134, 11);
            this.lblBatchTolXY.Name = "lblBatchTolXY";
            this.lblBatchTolXY.Size = new System.Drawing.Size(76, 20);
            this.lblBatchTolXY.TabIndex = 2;
            this.lblBatchTolXY.Text = "偏移 ≤ mm";
            // 
            // _batchTolXY
            // 
            this._batchTolXY.DecimalPlaces = 2;
            this._batchTolXY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this._batchTolXY.Location = new System.Drawing.Point(210, 7);
            this._batchTolXY.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this._batchTolXY.Name = "_batchTolXY";
            this._batchTolXY.Size = new System.Drawing.Size(66, 23);
            this._batchTolXY.TabIndex = 3;
            this._batchTolXY.ValueChanged += new System.EventHandler(this._batchTolXY_ValueChanged);
            // 
            // lblBatchTolAng
            // 
            this.lblBatchTolAng.Location = new System.Drawing.Point(284, 11);
            this.lblBatchTolAng.Name = "lblBatchTolAng";
            this.lblBatchTolAng.Size = new System.Drawing.Size(66, 20);
            this.lblBatchTolAng.TabIndex = 4;
            this.lblBatchTolAng.Text = "角度 ≤ °";
            // 
            // _batchTolAng
            // 
            this._batchTolAng.Location = new System.Drawing.Point(350, 7);
            this._batchTolAng.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this._batchTolAng.Name = "_batchTolAng";
            this._batchTolAng.Size = new System.Drawing.Size(60, 23);
            this._batchTolAng.TabIndex = 5;
            this._batchTolAng.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this._batchTolAng.ValueChanged += new System.EventHandler(this._batchTolAng_ValueChanged);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(418, 6);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(90, 28);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "导出 CSV";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(514, 6);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(110, 28);
            this.btnBack.TabIndex = 7;
            this.btnBack.Text = "看样板 / 返回";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // _batchSummary
            // 
            this._batchSummary.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._batchSummary.Location = new System.Drawing.Point(632, 11);
            this._batchSummary.Name = "_batchSummary";
            this._batchSummary.Size = new System.Drawing.Size(320, 20);
            this._batchSummary.TabIndex = 8;
            // 
            // _grid
            // 
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._grid.BackgroundColor = System.Drawing.Color.White;
            this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.gridColNo,
            this.gridColName,
            this.gridColDX,
            this.gridColDY,
            this.gridColAngle,
            this.gridColScore,
            this.gridColRequired,
            this.gridColVerdict});
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Location = new System.Drawing.Point(0, 0);
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(1120, 664);
            this._grid.TabIndex = 1;
            this._grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._grid_CellDoubleClick);
            // 
            // No
            // 
            this.gridColNo.FillWeight = 30F;
            this.gridColNo.HeaderText = "#";
            this.gridColNo.Name = "No";
            this.gridColNo.ReadOnly = true;
            // 
            // Name
            // 
            this.gridColName.FillWeight = 140F;
            this.gridColName.HeaderText = "文件名";
            this.gridColName.Name = "Name";
            this.gridColName.ReadOnly = true;
            // 
            // DX
            // 
            this.gridColDX.HeaderText = "ΔX (mm)";
            this.gridColDX.Name = "DX";
            this.gridColDX.ReadOnly = true;
            // 
            // DY
            // 
            this.gridColDY.HeaderText = "ΔY (mm)";
            this.gridColDY.Name = "DY";
            this.gridColDY.ReadOnly = true;
            // 
            // Angle
            // 
            this.gridColAngle.HeaderText = "角度 (°)";
            this.gridColAngle.Name = "Angle";
            this.gridColAngle.ReadOnly = true;
            // 
            // Score
            // 
            this.gridColScore.HeaderText = "相似度";
            this.gridColScore.Name = "Score";
            this.gridColScore.ReadOnly = true;
            // 
            // Required
            // 
            this.gridColRequired.HeaderText = "必要特征";
            this.gridColRequired.Name = "Required";
            this.gridColRequired.ReadOnly = true;
            // 
            // Verdict
            // 
            this.gridColVerdict.HeaderText = "判定";
            this.gridColVerdict.Name = "Verdict";
            this.gridColVerdict.ReadOnly = true;
            // 
            // TemplateEditorForm
            // 
            this.ClientSize = new System.Drawing.Size(1120, 740);
            // Dock 顺序：Fill 先 Add（占剩余空间），Top/Bottom/Right 后 Add（先切边条）。
            // 反了会让 Fill 控件占满容器、被边条遮盖，PictureBox.ClientSize 偏大导致点选坐标错位。
            this.Controls.Add(this._batchPanel);
            this.Controls.Add(this._pic);
            this.Controls.Add(this.bar);
            this.Controls.Add(this.right);
            this.Controls.Add(this._status);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "模板特征比对";
            this.bar.ResumeLayout(false);
            this.right.ResumeLayout(false);
            this.gp.ResumeLayout(false);
            this.gp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._sens)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._minArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._rx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._ry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._rw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._rh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._smooth)).EndInit();
            this.gl.ResumeLayout(false);
            this.gr.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pic)).EndInit();
            this._batchPanel.ResumeLayout(false);
            this.tools.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._batchScore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._batchTolXY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._batchTolAng)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.ResumeLayout(false);

        }

        // ---------- 由原匿名 delegate 提取的命名事件处理 ----------
        private void btnImport_Click(object sender, EventArgs e) { ImportSample(); }
        private void btnClear_Click(object sender, EventArgs e) { ClearRois(); }
        private void btnWhole_Click(object sender, EventArgs e) { SelectWholeImage(); }
        private void btnDone_Click(object sender, EventArgs e) { FinishTemplate(); }
        private void bg_Click(object sender, EventArgs e) { SetRole(FeatureRole.Position); }
        private void bp_Click(object sender, EventArgs e) { SetRole(FeatureRole.Ignored); }
        private void by_Click(object sender, EventArgs e) { SetRole(FeatureRole.Attached); }
        private void _btnFit_Click(object sender, EventArgs e) { ToggleFitCircle(); }
        private void btnExport_Click(object sender, EventArgs e) { ExportCsv(); }
        private void btnBack_Click(object sender, EventArgs e) { BackToSample(); }
        private void sx_TextChanged(object sender, EventArgs e)
        {
            double v;
            if (double.TryParse(sx.Text, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v)) Tpl.ScaleX = v;
        }
        private void sy_TextChanged(object sender, EventArgs e)
        {
            double v;
            if (double.TryParse(sy.Text, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out v)) Tpl.ScaleY = v;
        }
        private void _lineBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selected = _lineBox.SelectedIndex;
            UpdateFitButton(); ShowSelectionHint(); _pic.Invalidate();
        }
        private void _grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _batch.Count) ShowBatchItem(e.RowIndex);
        }
        private void _batchScore_ValueChanged(object sender, EventArgs e) { if (_batch.Count > 0) Rejudge(); }
        private void _batchTolXY_ValueChanged(object sender, EventArgs e) { if (_batch.Count > 0) Rejudge(); }
        private void _batchTolAng_ValueChanged(object sender, EventArgs e) { if (_batch.Count > 0) Rejudge(); }

        // ---------- 本 Designer 新增控件字段 ----------
        private System.Windows.Forms.FlowLayoutPanel bar;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnWhole;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Panel right;
        private System.Windows.Forms.GroupBox gp;
        private System.Windows.Forms.Label lblSens;
        private System.Windows.Forms.Label lblMinArea;
        private System.Windows.Forms.Label lblXY;
        private System.Windows.Forms.Label lblWH;
        private System.Windows.Forms.Label lblCalibXY;
        private System.Windows.Forms.TextBox sx;
        private System.Windows.Forms.TextBox sy;
        private System.Windows.Forms.Label lblSmooth;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.GroupBox gl;
        private System.Windows.Forms.Button bg;
        private System.Windows.Forms.Button bp;
        private System.Windows.Forms.Button by;
        private System.Windows.Forms.GroupBox gr;
        private System.Windows.Forms.Panel tools;
        private System.Windows.Forms.Label lblBatchScore;
        private System.Windows.Forms.Label lblBatchTolXY;
        private System.Windows.Forms.Label lblBatchTolAng;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColName;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColDX;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColDY;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColAngle;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColScore;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColRequired;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColVerdict;
    }
}

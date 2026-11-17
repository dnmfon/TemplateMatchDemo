using System;
using System.Drawing;
using System.Windows.Forms;

namespace TemplateMatchDemo
{
    /// <summary>设计器自动生成/维护的界面布局部分。业务代码请写在 ProductMainForm.cs。</summary>
    public sealed partial class ProductMainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            if (disposing && _viewImage != null)
            {
                _viewImage.Dispose();
                _viewImage = null;
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.bar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddTemplate = new System.Windows.Forms.Button();
            this.btnEditTemplate = new System.Windows.Forms.Button();
            this.btnAddProduct = new System.Windows.Forms.Button();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.btnCalibrate = new System.Windows.Forms.Button();
            this._matchMode = new System.Windows.Forms.ComboBox();
            this.lblSim = new System.Windows.Forms.Label();
            this._scoreMin = new System.Windows.Forms.NumericUpDown();
            this.lblTolTitle = new System.Windows.Forms.Label();
            this.lblTX = new System.Windows.Forms.Label();
            this.lblTY = new System.Windows.Forms.Label();
            this.lblTA = new System.Windows.Forms.Label();
            this.lblTildeX = new System.Windows.Forms.Label();
            this.lblTildeY = new System.Windows.Forms.Label();
            this.lblTildeA = new System.Windows.Forms.Label();
            this.lblTolUnit = new System.Windows.Forms.Label();
            this._tolXMin = new System.Windows.Forms.NumericUpDown();
            this._tolXMax = new System.Windows.Forms.NumericUpDown();
            this._tolYMin = new System.Windows.Forms.NumericUpDown();
            this._tolYMax = new System.Windows.Forms.NumericUpDown();
            this._tolAngMin = new System.Windows.Forms.NumericUpDown();
            this._tolAngMax = new System.Windows.Forms.NumericUpDown();
            this.info = new System.Windows.Forms.Panel();
            this._lblCount = new System.Windows.Forms.Label();
            this._lblTemplate = new System.Windows.Forms.Label();
            this._status = new System.Windows.Forms.Label();
            this.body = new System.Windows.Forms.Panel();
            this.right = new System.Windows.Forms.Panel();
            this.g1 = new System.Windows.Forms.GroupBox();
            this._curName = new System.Windows.Forms.Label();
            this._resFound = new System.Windows.Forms.Label();
            this._resDX = new System.Windows.Forms.Label();
            this._resDY = new System.Windows.Forms.Label();
            this._resAngle = new System.Windows.Forms.Label();
            this._resScore = new System.Windows.Forms.Label();
            this._resRequired = new System.Windows.Forms.Label();
            this._resVerdict = new System.Windows.Forms.Label();
            this.g3 = new System.Windows.Forms.GroupBox();
            this._roiList = new System.Windows.Forms.ListBox();
            this._roiKind = new System.Windows.Forms.ComboBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblRoiX = new System.Windows.Forms.Label();
            this._roiX = new System.Windows.Forms.NumericUpDown();
            this.lblRoiY = new System.Windows.Forms.Label();
            this._roiY = new System.Windows.Forms.NumericUpDown();
            this.lblRoiW = new System.Windows.Forms.Label();
            this._roiW = new System.Windows.Forms.NumericUpDown();
            this.lblRoiH = new System.Windows.Forms.Label();
            this._roiH = new System.Windows.Forms.NumericUpDown();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnClr = new System.Windows.Forms.Button();
            this.lblRoiHint = new System.Windows.Forms.Label();
            this.g2 = new System.Windows.Forms.GroupBox();
            this._resTemplate = new System.Windows.Forms.Label();
            this.lblLegend1 = new System.Windows.Forms.Label();
            this.lblLegend2 = new System.Windows.Forms.Label();
            this.lblLegend3 = new System.Windows.Forms.Label();
            this.lblLegend4 = new System.Windows.Forms.Label();
            this.left = new System.Windows.Forms.Panel();
            this._gridHost = new System.Windows.Forms.Panel();
            this.tools = new System.Windows.Forms.Panel();
            this.lblGridHint = new System.Windows.Forms.Label();
            this._summary = new System.Windows.Forms.Label();
            this._grid = new System.Windows.Forms.DataGridView();
            this.gridColNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColDX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColDY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColAngle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColRequired = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColVerdict = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._pic = new System.Windows.Forms.PictureBox();
            this.bar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._scoreMin)).BeginInit();
            this.info.SuspendLayout();
            this.body.SuspendLayout();
            this.right.SuspendLayout();
            this.g1.SuspendLayout();
            this.g3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._roiX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._roiY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._roiW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._roiH)).BeginInit();
            this.g2.SuspendLayout();
            this.left.SuspendLayout();
            this._gridHost.SuspendLayout();
            this.tools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pic)).BeginInit();
            this.SuspendLayout();
            // 
            // bar
            // 
            this.bar.Controls.Add(this.btnAddTemplate);
            this.bar.Controls.Add(this.btnEditTemplate);
            this.bar.Controls.Add(this.btnAddProduct);
            this.bar.Controls.Add(this.btnRunAll);
            this.bar.Controls.Add(this.btnExportCsv);
            this.bar.Controls.Add(this.btnCalibrate);
            this.bar.Controls.Add(this._matchMode);
            this.bar.Controls.Add(this.lblSim);
            this.bar.Controls.Add(this._scoreMin);

            this.bar.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar.Location = new System.Drawing.Point(0, 40);
            this.bar.Name = "bar";
            this.bar.Padding = new System.Windows.Forms.Padding(8, 8, 8, 2);
            this.bar.Size = new System.Drawing.Size(1280, 48);
            this.bar.TabIndex = 0;
            this.bar.WrapContents = false;
            // 
            // btnAddTemplate
            // 
            this.btnAddTemplate.Location = new System.Drawing.Point(11, 11);
            this.btnAddTemplate.Name = "btnAddTemplate";
            this.btnAddTemplate.Size = new System.Drawing.Size(110, 30);
            this.btnAddTemplate.TabIndex = 0;
            this.btnAddTemplate.Text = "添加模板";
            this.btnAddTemplate.UseVisualStyleBackColor = true;
            this.btnAddTemplate.Click += new System.EventHandler(this.btnAddTemplate_Click);
            // 
            // btnEditTemplate
            // 
            this.btnEditTemplate.Location = new System.Drawing.Point(127, 11);
            this.btnEditTemplate.Name = "btnEditTemplate";
            this.btnEditTemplate.Size = new System.Drawing.Size(110, 30);
            this.btnEditTemplate.TabIndex = 1;
            this.btnEditTemplate.Text = "编辑模板";
            this.btnEditTemplate.UseVisualStyleBackColor = true;
            this.btnEditTemplate.Click += new System.EventHandler(this.btnEditTemplate_Click);
            // 
            // btnAddProduct
            // 
            this.btnAddProduct.Location = new System.Drawing.Point(243, 11);
            this.btnAddProduct.Name = "btnAddProduct";
            this.btnAddProduct.Size = new System.Drawing.Size(110, 30);
            this.btnAddProduct.TabIndex = 2;
            this.btnAddProduct.Text = "添加产品";
            this.btnAddProduct.UseVisualStyleBackColor = true;
            this.btnAddProduct.Click += new System.EventHandler(this.btnAddProduct_Click);
            // 
            // btnRunAll
            // 
            this.btnRunAll.Location = new System.Drawing.Point(359, 11);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(100, 30);
            this.btnRunAll.TabIndex = 3;
            this.btnRunAll.Text = "重新比对";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Location = new System.Drawing.Point(465, 11);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(100, 30);
            this.btnExportCsv.TabIndex = 4;
            this.btnExportCsv.Text = "导出 CSV";
            this.btnExportCsv.UseVisualStyleBackColor = true;
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // btnCalibrate
            // 
            this.btnCalibrate.Location = new System.Drawing.Point(571, 11);
            this.btnCalibrate.Name = "btnCalibrate";
            this.btnCalibrate.Size = new System.Drawing.Size(80, 30);
            this.btnCalibrate.TabIndex = 5;
            this.btnCalibrate.Text = "标定";
            this.btnCalibrate.UseVisualStyleBackColor = true;
            this.btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);
            // 
            // _matchMode
            // 
            this._matchMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._matchMode.Items.AddRange(new object[] {
            "比对：模板匹配",
            "比对：几何形状"});
            // 必须显式指定默认项，否则 SelectedIndex=-1 时界面空白、用户不知道当前用哪种。
            // 默认「几何形状」：按形状独立匹配，不依赖像素重叠，
            // 平移/旋转/轻微缩放都不会误判低分（模板匹配只适合几乎不动的情形）。
            this._matchMode.SelectedIndex = 1;
            this._matchMode.Location = new System.Drawing.Point(670, 14);
            this._matchMode.Margin = new System.Windows.Forms.Padding(16, 6, 3, 3);
            this._matchMode.Name = "_matchMode";
            this._matchMode.Size = new System.Drawing.Size(140, 25);
            this._matchMode.TabIndex = 6;
            // 
            // lblSim
            // 
            this.lblSim.Location = new System.Drawing.Point(821, 14);
            this.lblSim.Margin = new System.Windows.Forms.Padding(8, 6, 0, 3);
            this.lblSim.Name = "lblSim";
            this.lblSim.Size = new System.Drawing.Size(66, 26);
            this.lblSim.TabIndex = 7;
            this.lblSim.Text = "相似度 ≥";
            this.lblSim.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _scoreMin
            // 
            this._scoreMin.Location = new System.Drawing.Point(890, 14);
            this._scoreMin.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this._scoreMin.Name = "_scoreMin";
            this._scoreMin.Size = new System.Drawing.Size(62, 23);
            this._scoreMin.TabIndex = 8;
            this._scoreMin.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // 判定容差（集成在信息栏：X/Y/角度 各自的上下限）
            // 
            this.lblTolTitle.Location = new System.Drawing.Point(12, 10);
            this.lblTolTitle.Size = new System.Drawing.Size(62, 22);
            this.lblTolTitle.Text = "判定容差";
            this.lblTolTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTolTitle.ForeColor = System.Drawing.Color.DimGray;
            this.lblTX.Location = new System.Drawing.Point(76, 10);
            this.lblTX.Size = new System.Drawing.Size(14, 22);
            this.lblTX.Text = "X";
            this.lblTX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._tolXMin.Location = new System.Drawing.Point(94, 9);
            this._tolXMin.Size = new System.Drawing.Size(54, 22);
            this._tolXMin.Minimum = new decimal(new int[] { 50, 0, 0, -2147483648 });
            this._tolXMin.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            this._tolXMin.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            this._tolXMin.DecimalPlaces = 2;
            this.lblTildeX.Location = new System.Drawing.Point(150, 10);
            this.lblTildeX.Size = new System.Drawing.Size(12, 22);
            this.lblTildeX.Text = "~";
            this.lblTildeX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTildeX.ForeColor = System.Drawing.Color.DimGray;
            this._tolXMax.Location = new System.Drawing.Point(164, 9);
            this._tolXMax.Size = new System.Drawing.Size(54, 22);
            this._tolXMax.Minimum = new decimal(new int[] { 50, 0, 0, -2147483648 });
            this._tolXMax.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            this._tolXMax.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            this._tolXMax.DecimalPlaces = 2;
            this.lblTY.Location = new System.Drawing.Point(224, 10);
            this.lblTY.Size = new System.Drawing.Size(14, 22);
            this.lblTY.Text = "Y";
            this.lblTY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._tolYMin.Location = new System.Drawing.Point(242, 9);
            this._tolYMin.Size = new System.Drawing.Size(54, 22);
            this._tolYMin.Minimum = new decimal(new int[] { 50, 0, 0, -2147483648 });
            this._tolYMin.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            this._tolYMin.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            this._tolYMin.DecimalPlaces = 2;
            this.lblTildeY.Location = new System.Drawing.Point(298, 10);
            this.lblTildeY.Size = new System.Drawing.Size(12, 22);
            this.lblTildeY.Text = "~";
            this.lblTildeY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTildeY.ForeColor = System.Drawing.Color.DimGray;
            this._tolYMax.Location = new System.Drawing.Point(312, 9);
            this._tolYMax.Size = new System.Drawing.Size(54, 22);
            this._tolYMax.Minimum = new decimal(new int[] { 50, 0, 0, -2147483648 });
            this._tolYMax.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            this._tolYMax.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            this._tolYMax.DecimalPlaces = 2;
            this.lblTA.Location = new System.Drawing.Point(372, 10);
            this.lblTA.Size = new System.Drawing.Size(18, 22);
            this.lblTA.Text = "角";
            this.lblTA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._tolAngMin.Location = new System.Drawing.Point(394, 9);
            this._tolAngMin.Size = new System.Drawing.Size(50, 22);
            this._tolAngMin.Minimum = new decimal(new int[] { 45, 0, 0, -2147483648 });
            this._tolAngMin.Maximum = new decimal(new int[] { 45, 0, 0, 0 });
            this._tolAngMin.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            this._tolAngMin.DecimalPlaces = 1;
            this.lblTildeA.Location = new System.Drawing.Point(446, 10);
            this.lblTildeA.Size = new System.Drawing.Size(12, 22);
            this.lblTildeA.Text = "~";
            this.lblTildeA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTildeA.ForeColor = System.Drawing.Color.DimGray;
            this._tolAngMax.Location = new System.Drawing.Point(460, 9);
            this._tolAngMax.Size = new System.Drawing.Size(50, 22);
            this._tolAngMax.Minimum = new decimal(new int[] { 45, 0, 0, -2147483648 });
            this._tolAngMax.Maximum = new decimal(new int[] { 45, 0, 0, 0 });
            this._tolAngMax.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            this._tolAngMax.DecimalPlaces = 1;
            this.lblTolUnit.Location = new System.Drawing.Point(514, 10);
            this.lblTolUnit.Size = new System.Drawing.Size(62, 22);
            this.lblTolUnit.Text = "mm / °";
            this.lblTolUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTolUnit.ForeColor = System.Drawing.Color.DimGray;
            // 
            // info
            // 
            this.info.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(240)))), ((int)(((byte)(248)))));
            this.info.Controls.Add(this.lblTolTitle);
            this.info.Controls.Add(this.lblTX);
            this.info.Controls.Add(this._tolXMin);
            this.info.Controls.Add(this.lblTildeX);
            this.info.Controls.Add(this._tolXMax);
            this.info.Controls.Add(this.lblTY);
            this.info.Controls.Add(this._tolYMin);
            this.info.Controls.Add(this.lblTildeY);
            this.info.Controls.Add(this._tolYMax);
            this.info.Controls.Add(this.lblTA);
            this.info.Controls.Add(this._tolAngMin);
            this.info.Controls.Add(this.lblTildeA);
            this.info.Controls.Add(this._tolAngMax);
            this.info.Controls.Add(this.lblTolUnit);
            this.info.Controls.Add(this._lblCount);
            this.info.Controls.Add(this._lblTemplate);
            this.info.Dock = System.Windows.Forms.DockStyle.Top;
            this.info.Location = new System.Drawing.Point(0, 0);
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(1280, 40);
            this.info.TabIndex = 1;
            // 
            // _lblCount
            // 
            this._lblCount.Dock = System.Windows.Forms.DockStyle.Right;
            this._lblCount.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._lblCount.Location = new System.Drawing.Point(1060, 0);
            this._lblCount.Name = "_lblCount";
            this._lblCount.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this._lblCount.Size = new System.Drawing.Size(220, 40);
            this._lblCount.TabIndex = 0;
            this._lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblTemplate
            // 
            this._lblTemplate.Dock = System.Windows.Forms.DockStyle.None;
            this._lblTemplate.Location = new System.Drawing.Point(596, 0);
            this._lblTemplate.Name = "_lblTemplate";
            this._lblTemplate.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this._lblTemplate.Size = new System.Drawing.Size(448, 40);
            this._lblTemplate.TabIndex = 1;
            this._lblTemplate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _status
            // 
            this._status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(240)))), ((int)(((byte)(246)))));
            this._status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._status.Location = new System.Drawing.Point(0, 790);
            this._status.Name = "_status";
            this._status.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this._status.Size = new System.Drawing.Size(1280, 30);
            this._status.TabIndex = 2;
            this._status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // body
            // 
            this.body.Controls.Add(this.left);
            this.body.Controls.Add(this.right);
            this.body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.body.Location = new System.Drawing.Point(0, 0);
            this.body.Name = "body";
            this.body.Size = new System.Drawing.Size(1280, 820);
            this.body.TabIndex = 3;
            // 
            // right
            // 
            this.right.AutoScroll = true;
            this.right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(247)))), ((int)(((byte)(251)))));
            this.right.Controls.Add(this.g1);
            this.right.Controls.Add(this.g3);
            this.right.Controls.Add(this.g2);
            this.right.Dock = System.Windows.Forms.DockStyle.Right;
            this.right.Location = new System.Drawing.Point(1028, 0);
            this.right.Name = "right";
            this.right.Size = new System.Drawing.Size(252, 820);
            this.right.TabIndex = 0;
            // 
            // g1
            // 
            this.g1.Controls.Add(this._curName);
            this.g1.Controls.Add(this._resFound);
            this.g1.Controls.Add(this._resDX);
            this.g1.Controls.Add(this._resDY);
            this.g1.Controls.Add(this._resAngle);
            this.g1.Controls.Add(this._resScore);
            this.g1.Controls.Add(this._resRequired);
            this.g1.Controls.Add(this._resVerdict);
            this.g1.Location = new System.Drawing.Point(8, 8);
            this.g1.Name = "g1";
            this.g1.Size = new System.Drawing.Size(230, 232);
            this.g1.TabIndex = 0;
            this.g1.TabStop = false;
            this.g1.Text = "当前产品 · 检测结果";
            // 
            // _curName
            // 
            this._curName.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold);
            this._curName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(60)))), ((int)(((byte)(90)))));
            this._curName.Location = new System.Drawing.Point(12, 20);
            this._curName.Name = "_curName";
            this._curName.Size = new System.Drawing.Size(206, 26);
            this._curName.TabIndex = 0;
            this._curName.Text = "— 未选择产品 —";
            // 
            // _resFound
            // 
            this._resFound.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this._resFound.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._resFound.Location = new System.Drawing.Point(14, 50);
            this._resFound.Name = "_resFound";
            this._resFound.Size = new System.Drawing.Size(208, 24);
            this._resFound.TabIndex = 1;
            this._resFound.Text = "匹配判定：—";
            this._resFound.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _resDX
            // 
            this._resDX.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this._resDX.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._resDX.Location = new System.Drawing.Point(14, 74);
            this._resDX.Name = "_resDX";
            this._resDX.Size = new System.Drawing.Size(208, 24);
            this._resDX.TabIndex = 2;
            this._resDX.Text = "ΔX：—";
            this._resDX.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _resDY
            // 
            this._resDY.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this._resDY.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._resDY.Location = new System.Drawing.Point(14, 98);
            this._resDY.Name = "_resDY";
            this._resDY.Size = new System.Drawing.Size(208, 24);
            this._resDY.TabIndex = 3;
            this._resDY.Text = "ΔY：—";
            this._resDY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _resAngle
            // 
            this._resAngle.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this._resAngle.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._resAngle.Location = new System.Drawing.Point(14, 122);
            this._resAngle.Name = "_resAngle";
            this._resAngle.Size = new System.Drawing.Size(208, 24);
            this._resAngle.TabIndex = 4;
            this._resAngle.Text = "角度偏移：—";
            this._resAngle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _resScore
            // 
            this._resScore.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this._resScore.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._resScore.Location = new System.Drawing.Point(14, 146);
            this._resScore.Name = "_resScore";
            this._resScore.Size = new System.Drawing.Size(208, 24);
            this._resScore.TabIndex = 5;
            this._resScore.Text = "相似度：—";
            this._resScore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _resRequired
            // 
            this._resRequired.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this._resRequired.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._resRequired.Location = new System.Drawing.Point(14, 170);
            this._resRequired.Name = "_resRequired";
            this._resRequired.Size = new System.Drawing.Size(208, 24);
            this._resRequired.TabIndex = 6;
            this._resRequired.Text = "必要特征：—";
            this._resRequired.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _resVerdict
            // 
            this._resVerdict.Font = new System.Drawing.Font("Microsoft YaHei UI", 13F, System.Drawing.FontStyle.Bold);
            this._resVerdict.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._resVerdict.Location = new System.Drawing.Point(14, 192);
            this._resVerdict.Name = "_resVerdict";
            this._resVerdict.Size = new System.Drawing.Size(206, 34);
            this._resVerdict.TabIndex = 7;
            this._resVerdict.Text = "判定：—";
            this._resVerdict.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // g3
            // 
            this.g3.Controls.Add(this._roiList);
            this.g3.Controls.Add(this._roiKind);
            this.g3.Controls.Add(this.btnApply);
            this.g3.Controls.Add(this.lblRoiX);
            this.g3.Controls.Add(this._roiX);
            this.g3.Controls.Add(this.lblRoiY);
            this.g3.Controls.Add(this._roiY);
            this.g3.Controls.Add(this.lblRoiW);
            this.g3.Controls.Add(this._roiW);
            this.g3.Controls.Add(this.lblRoiH);
            this.g3.Controls.Add(this._roiH);
            this.g3.Controls.Add(this.btnAll);
            this.g3.Controls.Add(this.btnClr);
            this.g3.Controls.Add(this.lblRoiHint);
            this.g3.Location = new System.Drawing.Point(8, 246);
            this.g3.Name = "g3";
            this.g3.Size = new System.Drawing.Size(230, 226);
            this.g3.TabIndex = 1;
            this.g3.TabStop = false;
            this.g3.Text = "识别区域 ROI";
            // 
            // _roiList
            // 
            this._roiList.ItemHeight = 17;
            this._roiList.Location = new System.Drawing.Point(12, 20);
            this._roiList.Name = "_roiList";
            this._roiList.Size = new System.Drawing.Size(206, 55);
            this._roiList.TabIndex = 0;
            this._roiList.SelectedIndexChanged += new System.EventHandler(this._roiList_SelectedIndexChanged);
            // 
            // _roiKind
            // 
            this._roiKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._roiKind.Items.AddRange(new object[] {
            "矩形框选",
            "圆形框选"});
            this._roiKind.Location = new System.Drawing.Point(12, 84);
            this._roiKind.Name = "_roiKind";
            this._roiKind.Size = new System.Drawing.Size(96, 25);
            this._roiKind.TabIndex = 1;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(116, 84);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(102, 24);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "应用数值";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lblRoiX
            // 
            this.lblRoiX.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblRoiX.Location = new System.Drawing.Point(12, 116);
            this.lblRoiX.Name = "lblRoiX";
            this.lblRoiX.Size = new System.Drawing.Size(18, 20);
            this.lblRoiX.TabIndex = 3;
            this.lblRoiX.Text = "X";
            // 
            // _roiX
            // 
            this._roiX.Location = new System.Drawing.Point(32, 114);
            this._roiX.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this._roiX.Name = "_roiX";
            this._roiX.Size = new System.Drawing.Size(72, 23);
            this._roiX.TabIndex = 4;
            // 
            // lblRoiY
            // 
            this.lblRoiY.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblRoiY.Location = new System.Drawing.Point(112, 116);
            this.lblRoiY.Name = "lblRoiY";
            this.lblRoiY.Size = new System.Drawing.Size(18, 20);
            this.lblRoiY.TabIndex = 5;
            this.lblRoiY.Text = "Y";
            // 
            // _roiY
            // 
            this._roiY.Location = new System.Drawing.Point(132, 114);
            this._roiY.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this._roiY.Name = "_roiY";
            this._roiY.Size = new System.Drawing.Size(72, 23);
            this._roiY.TabIndex = 6;
            // 
            // lblRoiW
            // 
            this.lblRoiW.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblRoiW.Location = new System.Drawing.Point(12, 142);
            this.lblRoiW.Name = "lblRoiW";
            this.lblRoiW.Size = new System.Drawing.Size(18, 20);
            this.lblRoiW.TabIndex = 7;
            this.lblRoiW.Text = "宽";
            // 
            // _roiW
            // 
            this._roiW.Location = new System.Drawing.Point(32, 140);
            this._roiW.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this._roiW.Name = "_roiW";
            this._roiW.Size = new System.Drawing.Size(72, 23);
            this._roiW.TabIndex = 8;
            // 
            // lblRoiH
            // 
            this.lblRoiH.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblRoiH.Location = new System.Drawing.Point(112, 142);
            this.lblRoiH.Name = "lblRoiH";
            this.lblRoiH.Size = new System.Drawing.Size(18, 20);
            this.lblRoiH.TabIndex = 9;
            this.lblRoiH.Text = "高";
            // 
            // _roiH
            // 
            this._roiH.Location = new System.Drawing.Point(132, 140);
            this._roiH.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this._roiH.Name = "_roiH";
            this._roiH.Size = new System.Drawing.Size(72, 23);
            this._roiH.TabIndex = 10;
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(12, 170);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(100, 24);
            this.btnAll.TabIndex = 11;
            this.btnAll.Text = "框住全图";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnClr
            // 
            this.btnClr.Location = new System.Drawing.Point(118, 170);
            this.btnClr.Name = "btnClr";
            this.btnClr.Size = new System.Drawing.Size(100, 24);
            this.btnClr.TabIndex = 12;
            this.btnClr.Text = "清空 ROI";
            this.btnClr.UseVisualStyleBackColor = true;
            this.btnClr.Click += new System.EventHandler(this.btnClr_Click);
            // 
            // lblRoiHint
            // 
            this.lblRoiHint.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.5F);
            this.lblRoiHint.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblRoiHint.Location = new System.Drawing.Point(12, 196);
            this.lblRoiHint.Name = "lblRoiHint";
            this.lblRoiHint.Size = new System.Drawing.Size(206, 26);
            this.lblRoiHint.TabIndex = 13;
            this.lblRoiHint.Text = "也可直接在左侧图像区按住鼠标拖框";
            // 
            // g2
            // 
            this.g2.Controls.Add(this._resTemplate);
            this.g2.Controls.Add(this.lblLegend1);
            this.g2.Controls.Add(this.lblLegend2);
            this.g2.Controls.Add(this.lblLegend3);
            this.g2.Controls.Add(this.lblLegend4);
            this.g2.Location = new System.Drawing.Point(8, 480);
            this.g2.Name = "g2";
            this.g2.Size = new System.Drawing.Size(230, 168);
            this.g2.TabIndex = 2;
            this.g2.TabStop = false;
            this.g2.Text = "模板与图例";
            // 
            // _resTemplate
            // 
            this._resTemplate.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this._resTemplate.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._resTemplate.Location = new System.Drawing.Point(12, 20);
            this._resTemplate.Name = "_resTemplate";
            this._resTemplate.Size = new System.Drawing.Size(206, 34);
            this._resTemplate.TabIndex = 0;
            // 
            // lblLegend1
            // 
            this.lblLegend1.ForeColor = System.Drawing.Color.Red;
            this.lblLegend1.Location = new System.Drawing.Point(12, 56);
            this.lblLegend1.Name = "lblLegend1";
            this.lblLegend1.Size = new System.Drawing.Size(206, 20);
            this.lblLegend1.TabIndex = 1;
            this.lblLegend1.Text = "■  红色 = 命中的位置特征";
            // 
            // lblLegend2
            // 
            this.lblLegend2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(175)))));
            this.lblLegend2.Location = new System.Drawing.Point(12, 76);
            this.lblLegend2.Name = "lblLegend2";
            this.lblLegend2.Size = new System.Drawing.Size(206, 20);
            this.lblLegend2.TabIndex = 2;
            this.lblLegend2.Text = "■  青色 = 本图其他特征";
            // 
            // lblLegend3
            // 
            this.lblLegend3.ForeColor = System.Drawing.Color.Green;
            this.lblLegend3.Location = new System.Drawing.Point(12, 96);
            this.lblLegend3.Name = "lblLegend3";
            this.lblLegend3.Size = new System.Drawing.Size(206, 20);
            this.lblLegend3.TabIndex = 3;
            this.lblLegend3.Text = "□  绿框 = 识别区域";
            // 
            // lblLegend4
            // 
            this.lblLegend4.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.5F);
            this.lblLegend4.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblLegend4.Location = new System.Drawing.Point(12, 120);
            this.lblLegend4.Name = "lblLegend4";
            this.lblLegend4.Size = new System.Drawing.Size(206, 38);
            this.lblLegend4.TabIndex = 4;
            this.lblLegend4.Text = "合格：相似度达标，且 |ΔX|、|ΔY| ≤ 0.10mm、|角度| ≤ 5°";
            // 
            // left
            // 
            this.left.Controls.Add(this._pic);
            this.left.Controls.Add(this._gridHost);
            this.left.Dock = System.Windows.Forms.DockStyle.Fill;
            this.left.Location = new System.Drawing.Point(0, 0);
            this.left.Name = "left";
            this.left.Size = new System.Drawing.Size(1280, 820);
            this.left.TabIndex = 1;
            // 
            // _gridHost
            // 
            // Dock 填充顺序：先加入 Fill 的 _grid，再加入 Dock=Top 的 tools。
            // 反过来会让 Fill 的表格先占满整个容器，再被 Top 工具条盖住顶部（表头被遮）。
            this._gridHost.Controls.Add(this._grid);
            this._gridHost.Controls.Add(this.tools);
            this._gridHost.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._gridHost.Location = new System.Drawing.Point(0, 594);
            this._gridHost.Name = "_gridHost";
            this._gridHost.Size = new System.Drawing.Size(1280, 226);
            this._gridHost.TabIndex = 0;
            // 
            // tools
            // 
            this.tools.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(240)))), ((int)(((byte)(246)))));
            this.tools.Controls.Add(this.lblGridHint);
            this.tools.Controls.Add(this._summary);
            this.tools.Dock = System.Windows.Forms.DockStyle.Top;
            this.tools.Location = new System.Drawing.Point(0, 0);
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(1280, 34);
            this.tools.TabIndex = 0;
            // 
            // lblGridHint
            // 
            this.lblGridHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblGridHint.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblGridHint.Location = new System.Drawing.Point(1030, 0);
            this.lblGridHint.Name = "lblGridHint";
            this.lblGridHint.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.lblGridHint.Size = new System.Drawing.Size(250, 34);
            this.lblGridHint.TabIndex = 0;
            this.lblGridHint.Text = "单击任一行 → 上方显示该产品高亮";
            this.lblGridHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _summary
            // 
            this._summary.Dock = System.Windows.Forms.DockStyle.Fill;
            this._summary.ForeColor = System.Drawing.Color.DarkSlateGray;
            this._summary.Location = new System.Drawing.Point(0, 0);
            this._summary.Name = "_summary";
            this._summary.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this._summary.Size = new System.Drawing.Size(1280, 34);
            this._summary.TabIndex = 1;
            this._summary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this._grid.Size = new System.Drawing.Size(1280, 226);
            this._grid.TabIndex = 1;
            this._grid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._grid_CellClick);
            this._grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._grid_CellClick);
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
            this.gridColName.FillWeight = 150F;
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
            // _pic
            // 
            this._pic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(26)))), ((int)(((byte)(36)))));
            this._pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pic.Location = new System.Drawing.Point(0, 0);
            this._pic.Name = "_pic";
            this._pic.Size = new System.Drawing.Size(1280, 820);
            this._pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pic.TabIndex = 2;
            this._pic.TabStop = false;
            this._pic.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintImage);
            this._pic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnPicMouseDown);
            this._pic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnPicMouseMove);
            this._pic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnPicMouseUp);
            // 
            // ProductMainForm
            // 
            this.ClientSize = new System.Drawing.Size(1280, 820);
            // Dock 顺序：Fill 控件先 Add（占剩余空间），Dock=Top/Bottom/Right 的后 Add（先切走边条）。
            // 顺序反了会让 Fill 控件占满整个容器、再被边条覆盖，导致被遮部分不可见、
            // 且 PictureBox 的 ClientSize 偏大 → 鼠标点选坐标换算错位。
            this.Controls.Add(this.body);
            this.Controls.Add(this.bar);
            this.Controls.Add(this.info);
            this.Controls.Add(this._status);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.Name = "ProductMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模板特征比对 — 产品检测";
            this.bar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._scoreMin)).EndInit();
            this.info.ResumeLayout(false);
            this.body.ResumeLayout(false);
            this.right.ResumeLayout(false);
            this.g1.ResumeLayout(false);
            this.g3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._roiX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._roiY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._roiW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._roiH)).EndInit();
            this.g2.ResumeLayout(false);
            this.left.ResumeLayout(false);
            this._gridHost.ResumeLayout(false);
            this.tools.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pic)).EndInit();
            this.ResumeLayout(false);

        }



        private System.Windows.Forms.FlowLayoutPanel bar;
        private System.Windows.Forms.Button btnAddTemplate;
        private System.Windows.Forms.Button btnEditTemplate;
        private System.Windows.Forms.Button btnAddProduct;
        private System.Windows.Forms.Button btnRunAll;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnCalibrate;
        private System.Windows.Forms.ComboBox _matchMode;
        private System.Windows.Forms.Label lblSim;
        private System.Windows.Forms.NumericUpDown _scoreMin;
        private System.Windows.Forms.Label lblTolTitle;
        private System.Windows.Forms.Label lblTX, lblTY, lblTA;
        private System.Windows.Forms.Label lblTildeX, lblTildeY, lblTildeA, lblTolUnit;
        private System.Windows.Forms.NumericUpDown _tolXMin, _tolXMax;
        private System.Windows.Forms.NumericUpDown _tolYMin, _tolYMax;
        private System.Windows.Forms.NumericUpDown _tolAngMin, _tolAngMax;
        private System.Windows.Forms.Panel info;
        private System.Windows.Forms.Label _lblCount;
        private System.Windows.Forms.Label _lblTemplate;
        private System.Windows.Forms.Label _status;
        private System.Windows.Forms.Panel body;
        private System.Windows.Forms.Panel right;
        private System.Windows.Forms.GroupBox g1;
        private System.Windows.Forms.Label _curName;
        private System.Windows.Forms.Label _resFound;
        private System.Windows.Forms.Label _resDX;
        private System.Windows.Forms.Label _resDY;
        private System.Windows.Forms.Label _resAngle;
        private System.Windows.Forms.Label _resScore;
        private System.Windows.Forms.Label _resRequired;
        private System.Windows.Forms.Label _resVerdict;
        private System.Windows.Forms.GroupBox g3;
        private System.Windows.Forms.ListBox _roiList;
        private System.Windows.Forms.ComboBox _roiKind;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblRoiX;
        private System.Windows.Forms.NumericUpDown _roiX;
        private System.Windows.Forms.Label lblRoiY;
        private System.Windows.Forms.NumericUpDown _roiY;
        private System.Windows.Forms.Label lblRoiW;
        private System.Windows.Forms.NumericUpDown _roiW;
        private System.Windows.Forms.Label lblRoiH;
        private System.Windows.Forms.NumericUpDown _roiH;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnClr;
        private System.Windows.Forms.Label lblRoiHint;
        private System.Windows.Forms.GroupBox g2;
        private System.Windows.Forms.Label _resTemplate;
        private System.Windows.Forms.Label lblLegend1;
        private System.Windows.Forms.Label lblLegend2;
        private System.Windows.Forms.Label lblLegend3;
        private System.Windows.Forms.Label lblLegend4;
        private System.Windows.Forms.Panel left;
        private System.Windows.Forms.Panel _gridHost;
        private System.Windows.Forms.Panel tools;
        private System.Windows.Forms.Label lblGridHint;
        private System.Windows.Forms.Label _summary;
        private System.Windows.Forms.DataGridView _grid;
        private System.Windows.Forms.PictureBox _pic;
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

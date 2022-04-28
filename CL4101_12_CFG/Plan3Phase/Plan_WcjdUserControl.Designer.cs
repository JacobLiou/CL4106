namespace CL4100.Plan3Phase
{
    partial class Plan_WcjdUserControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Tab_Info = new System.Windows.Forms.TabControl();
            this.Pag_Result = new System.Windows.Forms.TabPage();
            this.cmbGPBS = new System.Windows.Forms.ComboBox();
            this.label52 = new System.Windows.Forms.Label();
            this.dgvPoint = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDeletePoint = new System.Windows.Forms.Button();
            this.btnAddPoint = new System.Windows.Forms.Button();
            this.cmbYJ = new System.Windows.Forms.ComboBox();
            this.cmbGLFX = new System.Windows.Forms.ComboBox();
            this.cmbHL = new System.Windows.Forms.ComboBox();
            this.cmbGLYS = new System.Windows.Forms.ComboBox();
            this.cmbDL = new System.Windows.Forms.ComboBox();
            this.cmbBLX = new System.Windows.Forms.ComboBox();
            this.cmbBLS = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.Tab_Info.SuspendLayout();
            this.Pag_Result.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoint)).BeginInit();
            this.SuspendLayout();
            // 
            // Tab_Info
            // 
            this.Tab_Info.Controls.Add(this.Pag_Result);
            this.Tab_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tab_Info.Location = new System.Drawing.Point(0, 0);
            this.Tab_Info.Name = "Tab_Info";
            this.Tab_Info.SelectedIndex = 0;
            this.Tab_Info.Size = new System.Drawing.Size(815, 416);
            this.Tab_Info.TabIndex = 6;
            // 
            // Pag_Result
            // 
            this.Pag_Result.Controls.Add(this.cmbGPBS);
            this.Pag_Result.Controls.Add(this.label52);
            this.Pag_Result.Controls.Add(this.dgvPoint);
            this.Pag_Result.Controls.Add(this.btnDeletePoint);
            this.Pag_Result.Controls.Add(this.btnAddPoint);
            this.Pag_Result.Controls.Add(this.cmbYJ);
            this.Pag_Result.Controls.Add(this.cmbGLFX);
            this.Pag_Result.Controls.Add(this.cmbHL);
            this.Pag_Result.Controls.Add(this.cmbGLYS);
            this.Pag_Result.Controls.Add(this.cmbDL);
            this.Pag_Result.Controls.Add(this.cmbBLX);
            this.Pag_Result.Controls.Add(this.cmbBLS);
            this.Pag_Result.Controls.Add(this.label16);
            this.Pag_Result.Controls.Add(this.label17);
            this.Pag_Result.Location = new System.Drawing.Point(4, 21);
            this.Pag_Result.Name = "Pag_Result";
            this.Pag_Result.Size = new System.Drawing.Size(807, 391);
            this.Pag_Result.TabIndex = 1;
            this.Pag_Result.Text = "误差检定";
            this.Pag_Result.UseVisualStyleBackColor = true;
            // 
            // cmbGPBS
            // 
            this.cmbGPBS.FormattingEnabled = true;
            this.cmbGPBS.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbGPBS.Location = new System.Drawing.Point(120, 9);
            this.cmbGPBS.Name = "cmbGPBS";
            this.cmbGPBS.Size = new System.Drawing.Size(57, 20);
            this.cmbGPBS.TabIndex = 72;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(15, 12);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(89, 12);
            this.label52.TabIndex = 71;
            this.label52.Text = "高频检定倍数：";
            // 
            // dgvPoint
            // 
            this.dgvPoint.AllowUserToAddRows = false;
            this.dgvPoint.AllowUserToResizeRows = false;
            this.dgvPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPoint.BackgroundColor = System.Drawing.Color.White;
            this.dgvPoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPoint.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn8,
            this.Column12,
            this.Column13,
            this.Column14,
            this.Column15,
            this.Column17,
            this.Column16});
            this.dgvPoint.Location = new System.Drawing.Point(8, 69);
            this.dgvPoint.Name = "dgvPoint";
            this.dgvPoint.RowHeadersVisible = false;
            this.dgvPoint.RowTemplate.Height = 23;
            this.dgvPoint.Size = new System.Drawing.Size(794, 318);
            this.dgvPoint.TabIndex = 70;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "WCPoint";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn1.HeaderText = "检校点";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 254;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "TurnsNO";
            this.dataGridViewTextBoxColumn2.HeaderText = "圈数";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 52;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "WCUpperLimit";
            this.dataGridViewTextBoxColumn5.HeaderText = "误差上限";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn5.Width = 78;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.DataPropertyName = "WCLowerLimit";
            this.dataGridViewTextBoxColumn8.HeaderText = "误差下限";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn8.Width = 78;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "HL";
            this.Column12.HeaderText = "回路";
            this.Column12.Name = "Column12";
            this.Column12.ReadOnly = true;
            this.Column12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column12.Width = 52;
            // 
            // Column13
            // 
            this.Column13.DataPropertyName = "GLFX";
            this.Column13.HeaderText = "功率方向";
            this.Column13.Name = "Column13";
            this.Column13.ReadOnly = true;
            this.Column13.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column13.Width = 78;
            // 
            // Column14
            // 
            this.Column14.DataPropertyName = "YJ";
            this.Column14.HeaderText = "元件";
            this.Column14.Name = "Column14";
            this.Column14.ReadOnly = true;
            this.Column14.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column14.Width = 52;
            // 
            // Column15
            // 
            this.Column15.DataPropertyName = "GLYS";
            this.Column15.HeaderText = "功率因素";
            this.Column15.Name = "Column15";
            this.Column15.ReadOnly = true;
            this.Column15.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column15.Width = 78;
            // 
            // Column17
            // 
            this.Column17.DataPropertyName = "DL";
            this.Column17.HeaderText = "电流";
            this.Column17.Name = "Column17";
            this.Column17.ReadOnly = true;
            this.Column17.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column17.Width = 52;
            // 
            // Column16
            // 
            this.Column16.DataPropertyName = "DLHp";
            this.Column16.HeaderText = "电流隐藏";
            this.Column16.Name = "Column16";
            this.Column16.ReadOnly = true;
            this.Column16.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column16.Visible = false;
            this.Column16.Width = 80;
            // 
            // btnDeletePoint
            // 
            this.btnDeletePoint.Location = new System.Drawing.Point(668, 39);
            this.btnDeletePoint.Name = "btnDeletePoint";
            this.btnDeletePoint.Size = new System.Drawing.Size(75, 23);
            this.btnDeletePoint.TabIndex = 69;
            this.btnDeletePoint.Text = "删  除";
            this.btnDeletePoint.UseVisualStyleBackColor = true;
            this.btnDeletePoint.Click += new System.EventHandler(this.btnDeletePoint_Click);
            // 
            // btnAddPoint
            // 
            this.btnAddPoint.Location = new System.Drawing.Point(587, 40);
            this.btnAddPoint.Name = "btnAddPoint";
            this.btnAddPoint.Size = new System.Drawing.Size(75, 23);
            this.btnAddPoint.TabIndex = 68;
            this.btnAddPoint.Text = "添  加";
            this.btnAddPoint.UseVisualStyleBackColor = true;
            this.btnAddPoint.Click += new System.EventHandler(this.btnAddPoint_Click);
            // 
            // cmbYJ
            // 
            this.cmbYJ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYJ.FormattingEnabled = true;
            this.cmbYJ.Items.AddRange(new object[] {
            "合元",
            "A元",
            "B元",
            "C元"});
            this.cmbYJ.Location = new System.Drawing.Point(234, 42);
            this.cmbYJ.Name = "cmbYJ";
            this.cmbYJ.Size = new System.Drawing.Size(100, 20);
            this.cmbYJ.TabIndex = 67;
            // 
            // cmbGLFX
            // 
            this.cmbGLFX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGLFX.FormattingEnabled = true;
            this.cmbGLFX.Items.AddRange(new object[] {
            "正向有功",
            "反向有功",
            "正向无功",
            "反向无功"});
            this.cmbGLFX.Location = new System.Drawing.Point(120, 42);
            this.cmbGLFX.Name = "cmbGLFX";
            this.cmbGLFX.Size = new System.Drawing.Size(100, 20);
            this.cmbGLFX.TabIndex = 66;
            // 
            // cmbHL
            // 
            this.cmbHL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHL.FormattingEnabled = true;
            this.cmbHL.Items.AddRange(new object[] {
            "回路1",
            "回路2"});
            this.cmbHL.Location = new System.Drawing.Point(7, 42);
            this.cmbHL.Name = "cmbHL";
            this.cmbHL.Size = new System.Drawing.Size(100, 20);
            this.cmbHL.TabIndex = 65;
            // 
            // cmbGLYS
            // 
            this.cmbGLYS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGLYS.FormattingEnabled = true;
            this.cmbGLYS.Items.AddRange(new object[] {
            "1.0",
            "0.5L",
            "0.8C",
            "0.5C",
            "0.25L"});
            this.cmbGLYS.Location = new System.Drawing.Point(349, 43);
            this.cmbGLYS.Name = "cmbGLYS";
            this.cmbGLYS.Size = new System.Drawing.Size(100, 20);
            this.cmbGLYS.TabIndex = 64;
            // 
            // cmbDL
            // 
            this.cmbDL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDL.FormattingEnabled = true;
            this.cmbDL.Items.AddRange(new object[] {
            "Imax",
            "0.5Imax",
            "3.0Ib",
            "2.0Ib",
            "(M-I)/2",
            "1.0Ib",
            "0.5Ib",
            "0.2Ib",
            "0.1Ib",
            "0.05Ib",
            "0.02Ib",
            "0.01Ib"});
            this.cmbDL.Location = new System.Drawing.Point(462, 44);
            this.cmbDL.Name = "cmbDL";
            this.cmbDL.Size = new System.Drawing.Size(100, 20);
            this.cmbDL.TabIndex = 63;
            // 
            // cmbBLX
            // 
            this.cmbBLX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBLX.FormattingEnabled = true;
            this.cmbBLX.Items.AddRange(new object[] {
            "1/128",
            "1/64",
            "1/32",
            "1/16",
            "1/8",
            "1/4",
            "1/2",
            "1",
            "2",
            "4",
            "6",
            "8",
            "16",
            "32",
            "64",
            "128"});
            this.cmbBLX.Location = new System.Drawing.Point(587, 10);
            this.cmbBLX.Name = "cmbBLX";
            this.cmbBLX.Size = new System.Drawing.Size(48, 20);
            this.cmbBLX.TabIndex = 62;
            this.cmbBLX.SelectedIndexChanged += new System.EventHandler(this.cmbBLX_SelectedIndexChanged);
            // 
            // cmbBLS
            // 
            this.cmbBLS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBLS.FormattingEnabled = true;
            this.cmbBLS.Items.AddRange(new object[] {
            "1/128",
            "1/64",
            "1/32",
            "1/16",
            "1/8",
            "1/4",
            "1/2",
            "1",
            "2",
            "4",
            "6",
            "8",
            "16",
            "32",
            "64",
            "128"});
            this.cmbBLS.Location = new System.Drawing.Point(349, 9);
            this.cmbBLS.Name = "cmbBLS";
            this.cmbBLS.Size = new System.Drawing.Size(48, 20);
            this.cmbBLS.TabIndex = 61;
            this.cmbBLS.SelectedIndexChanged += new System.EventHandler(this.cmbBLS_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(460, 13);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(89, 12);
            this.label16.TabIndex = 60;
            this.label16.Text = "下限统一比例：";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(232, 12);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(89, 12);
            this.label17.TabIndex = 59;
            this.label17.Text = "上限统一比例：";
            // 
            // Plan_WcjdUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Tab_Info);
            this.Name = "Plan_WcjdUserControl";
            this.Size = new System.Drawing.Size(815, 416);
            this.Tab_Info.ResumeLayout(false);
            this.Pag_Result.ResumeLayout(false);
            this.Pag_Result.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoint)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Tab_Info;
        private System.Windows.Forms.TabPage Pag_Result;
        private System.Windows.Forms.DataGridView dgvPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.Button btnDeletePoint;
        private System.Windows.Forms.Button btnAddPoint;
        private System.Windows.Forms.ComboBox cmbYJ;
        private System.Windows.Forms.ComboBox cmbGLFX;
        private System.Windows.Forms.ComboBox cmbHL;
        private System.Windows.Forms.ComboBox cmbGLYS;
        private System.Windows.Forms.ComboBox cmbDL;
        private System.Windows.Forms.ComboBox cmbBLX;
        private System.Windows.Forms.ComboBox cmbBLS;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox cmbGPBS;
        private System.Windows.Forms.Label label52;

    }
}

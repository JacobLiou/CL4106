namespace Frontier.MeterVerification.Communication
{
    partial class SetEquipParamDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Save = new System.Windows.Forms.Button();
            this.tabCotal_Equip = new System.Windows.Forms.TabControl();
            this.tabPag_Equip = new System.Windows.Forms.TabPage();
            this.dgw_Equip = new System.Windows.Forms.DataGridView();
            this._仪器仪表 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._仪器仪表参数 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPag_Meter = new System.Windows.Forms.TabPage();
            this.dgt_Meter = new System.Windows.Forms.DataGridView();
            this._表位号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._表位号端口参数 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage_EquipSet = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.cbx_Rs485 = new System.Windows.Forms.ComboBox();
            this.comBox_Error = new System.Windows.Forms.ComboBox();
            this.comBox_TimeBs = new System.Windows.Forms.ComboBox();
            this.comBox_Power = new System.Windows.Forms.ComboBox();
            this.comBox_Std = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.IsPushBox = new System.Windows.Forms.CheckBox();
            this.tabCotal_Equip.SuspendLayout();
            this.tabPag_Equip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgw_Equip)).BeginInit();
            this.tabPag_Meter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgt_Meter)).BeginInit();
            this.tabPage_EquipSet.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Save
            // 
            this.btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Save.Location = new System.Drawing.Point(422, 267);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(85, 36);
            this.btn_Save.TabIndex = 0;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // tabCotal_Equip
            // 
            this.tabCotal_Equip.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCotal_Equip.Controls.Add(this.tabPag_Equip);
            this.tabCotal_Equip.Controls.Add(this.tabPag_Meter);
            this.tabCotal_Equip.Controls.Add(this.tabPage_EquipSet);
            this.tabCotal_Equip.Location = new System.Drawing.Point(0, -1);
            this.tabCotal_Equip.Name = "tabCotal_Equip";
            this.tabCotal_Equip.SelectedIndex = 0;
            this.tabCotal_Equip.Size = new System.Drawing.Size(537, 262);
            this.tabCotal_Equip.TabIndex = 1;
            // 
            // tabPag_Equip
            // 
            this.tabPag_Equip.Controls.Add(this.dgw_Equip);
            this.tabPag_Equip.Location = new System.Drawing.Point(4, 21);
            this.tabPag_Equip.Name = "tabPag_Equip";
            this.tabPag_Equip.Padding = new System.Windows.Forms.Padding(3);
            this.tabPag_Equip.Size = new System.Drawing.Size(529, 237);
            this.tabPag_Equip.TabIndex = 0;
            this.tabPag_Equip.Text = "仪器仪表";
            this.tabPag_Equip.UseVisualStyleBackColor = true;
            // 
            // dgw_Equip
            // 
            this.dgw_Equip.AllowUserToAddRows = false;
            this.dgw_Equip.AllowUserToDeleteRows = false;
            this.dgw_Equip.AllowUserToResizeRows = false;
            this.dgw_Equip.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgw_Equip.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._仪器仪表,
            this._仪器仪表参数});
            this.dgw_Equip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgw_Equip.Location = new System.Drawing.Point(3, 3);
            this.dgw_Equip.Name = "dgw_Equip";
            this.dgw_Equip.RowTemplate.Height = 23;
            this.dgw_Equip.Size = new System.Drawing.Size(523, 231);
            this.dgw_Equip.TabIndex = 0;
            // 
            // _仪器仪表
            // 
            this._仪器仪表.HeaderText = "仪器仪表";
            this._仪器仪表.Name = "_仪器仪表";
            this._仪器仪表.ReadOnly = true;
            this._仪器仪表.Width = 120;
            // 
            // _仪器仪表参数
            // 
            this._仪器仪表参数.HeaderText = "端口参数";
            this._仪器仪表参数.Name = "_仪器仪表参数";
            this._仪器仪表参数.Width = 360;
            // 
            // tabPag_Meter
            // 
            this.tabPag_Meter.Controls.Add(this.dgt_Meter);
            this.tabPag_Meter.Location = new System.Drawing.Point(4, 21);
            this.tabPag_Meter.Name = "tabPag_Meter";
            this.tabPag_Meter.Padding = new System.Windows.Forms.Padding(3);
            this.tabPag_Meter.Size = new System.Drawing.Size(529, 237);
            this.tabPag_Meter.TabIndex = 1;
            this.tabPag_Meter.Text = "表位端口";
            this.tabPag_Meter.UseVisualStyleBackColor = true;
            // 
            // dgt_Meter
            // 
            this.dgt_Meter.AllowUserToAddRows = false;
            this.dgt_Meter.AllowUserToDeleteRows = false;
            this.dgt_Meter.AllowUserToResizeRows = false;
            this.dgt_Meter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgt_Meter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._表位号,
            this._表位号端口参数});
            this.dgt_Meter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgt_Meter.Location = new System.Drawing.Point(3, 3);
            this.dgt_Meter.Name = "dgt_Meter";
            this.dgt_Meter.RowTemplate.Height = 23;
            this.dgt_Meter.Size = new System.Drawing.Size(523, 231);
            this.dgt_Meter.TabIndex = 1;
            // 
            // _表位号
            // 
            this._表位号.HeaderText = "表位号";
            this._表位号.Name = "_表位号";
            this._表位号.ReadOnly = true;
            this._表位号.Width = 80;
            // 
            // _表位号端口参数
            // 
            this._表位号端口参数.HeaderText = "端口参数";
            this._表位号端口参数.Name = "_表位号端口参数";
            this._表位号端口参数.Width = 360;
            // 
            // tabPage_EquipSet
            // 
            this.tabPage_EquipSet.Controls.Add(this.IsPushBox);
            this.tabPage_EquipSet.Controls.Add(this.label6);
            this.tabPage_EquipSet.Controls.Add(this.cbx_Rs485);
            this.tabPage_EquipSet.Controls.Add(this.comBox_Error);
            this.tabPage_EquipSet.Controls.Add(this.comBox_TimeBs);
            this.tabPage_EquipSet.Controls.Add(this.comBox_Power);
            this.tabPage_EquipSet.Controls.Add(this.comBox_Std);
            this.tabPage_EquipSet.Controls.Add(this.label4);
            this.tabPage_EquipSet.Controls.Add(this.label5);
            this.tabPage_EquipSet.Controls.Add(this.label3);
            this.tabPage_EquipSet.Controls.Add(this.label2);
            this.tabPage_EquipSet.Controls.Add(this.label1);
            this.tabPage_EquipSet.Location = new System.Drawing.Point(4, 21);
            this.tabPage_EquipSet.Name = "tabPage_EquipSet";
            this.tabPage_EquipSet.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_EquipSet.Size = new System.Drawing.Size(529, 237);
            this.tabPage_EquipSet.TabIndex = 2;
            this.tabPage_EquipSet.Text = "设备配置";
            this.tabPage_EquipSet.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Location = new System.Drawing.Point(179, 165);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(191, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "注意:188G与188E误差板都选择188E";
            // 
            // cbx_Rs485
            // 
            this.cbx_Rs485.FormattingEnabled = true;
            this.cbx_Rs485.Items.AddRange(new object[] {
            "Open",
            "Close"});
            this.cbx_Rs485.Location = new System.Drawing.Point(181, 180);
            this.cbx_Rs485.Name = "cbx_Rs485";
            this.cbx_Rs485.Size = new System.Drawing.Size(137, 20);
            this.cbx_Rs485.TabIndex = 5;
            // 
            // comBox_Error
            // 
            this.comBox_Error.FormattingEnabled = true;
            this.comBox_Error.Items.AddRange(new object[] {
            "CL188E",
            "CL188L"});
            this.comBox_Error.Location = new System.Drawing.Point(181, 138);
            this.comBox_Error.Name = "comBox_Error";
            this.comBox_Error.Size = new System.Drawing.Size(137, 20);
            this.comBox_Error.TabIndex = 4;
            // 
            // comBox_TimeBs
            // 
            this.comBox_TimeBs.FormattingEnabled = true;
            this.comBox_TimeBs.Items.AddRange(new object[] {
            "CL191B"});
            this.comBox_TimeBs.Location = new System.Drawing.Point(181, 98);
            this.comBox_TimeBs.Name = "comBox_TimeBs";
            this.comBox_TimeBs.Size = new System.Drawing.Size(137, 20);
            this.comBox_TimeBs.TabIndex = 3;
            // 
            // comBox_Power
            // 
            this.comBox_Power.FormattingEnabled = true;
            this.comBox_Power.Items.AddRange(new object[] {
            "CL309",
            "CL303"});
            this.comBox_Power.Location = new System.Drawing.Point(181, 62);
            this.comBox_Power.Name = "comBox_Power";
            this.comBox_Power.Size = new System.Drawing.Size(137, 20);
            this.comBox_Power.TabIndex = 2;
            // 
            // comBox_Std
            // 
            this.comBox_Std.FormattingEnabled = true;
            this.comBox_Std.Items.AddRange(new object[] {
            "CL3115",
            "CL311V2"});
            this.comBox_Std.Location = new System.Drawing.Point(181, 23);
            this.comBox_Std.Name = "comBox_Std";
            this.comBox_Std.Size = new System.Drawing.Size(137, 20);
            this.comBox_Std.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(94, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "时基源";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(22, 184);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "是否启用485通讯";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(94, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "误差板";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(94, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "功率源";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(94, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "标准表";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(293, 278);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 24);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(201, 280);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // IsPushBox
            // 
            this.IsPushBox.AutoSize = true;
            this.IsPushBox.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.IsPushBox.Location = new System.Drawing.Point(325, 141);
            this.IsPushBox.Name = "IsPushBox";
            this.IsPushBox.Size = new System.Drawing.Size(96, 16);
            this.IsPushBox.TabIndex = 7;
            this.IsPushBox.Text = "是否是脉冲盒";
            this.IsPushBox.UseVisualStyleBackColor = true;
            // 
            // SetEquipParamDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 308);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tabCotal_Equip);
            this.Controls.Add(this.btn_Save);
            this.MaximizeBox = false;
            this.Name = "SetEquipParamDlg";
            this.Text = "装置控制参数配置";
            this.Load += new System.EventHandler(this.SetEquipParamDlg_Load);
            this.tabCotal_Equip.ResumeLayout(false);
            this.tabPag_Equip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgw_Equip)).EndInit();
            this.tabPag_Meter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgt_Meter)).EndInit();
            this.tabPage_EquipSet.ResumeLayout(false);
            this.tabPage_EquipSet.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.TabControl tabCotal_Equip;
        private System.Windows.Forms.TabPage tabPag_Equip;
        private System.Windows.Forms.TabPage tabPag_Meter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dgw_Equip;
        private System.Windows.Forms.DataGridView dgt_Meter;
        private System.Windows.Forms.DataGridViewTextBoxColumn _仪器仪表;
        private System.Windows.Forms.DataGridViewTextBoxColumn _仪器仪表参数;
        private System.Windows.Forms.DataGridViewTextBoxColumn _表位号;
        private System.Windows.Forms.DataGridViewTextBoxColumn _表位号端口参数;
        private System.Windows.Forms.TabPage tabPage_EquipSet;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comBox_Error;
        private System.Windows.Forms.ComboBox comBox_TimeBs;
        private System.Windows.Forms.ComboBox comBox_Power;
        private System.Windows.Forms.ComboBox comBox_Std;
        private System.Windows.Forms.ComboBox cbx_Rs485;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox IsPushBox;
    }
}
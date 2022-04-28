namespace ClAmMeterController
{
    partial class frmParameter
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lvwConfig = new System.Windows.Forms.ListView();
            this.clhChannelIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhChannelComType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhChannelComPara = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhChannelComSetting = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlEditPara = new System.Windows.Forms.Panel();
            this.cmb_Setting = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btnOkEdit = new System.Windows.Forms.Button();
            this.btnCancelEdit = new System.Windows.Forms.Button();
            this.txtBindPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtRomtPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtIpAddr = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbComNo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbComClass = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbComLib = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtChannelNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlEditPara.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(705, 346);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lvwConfig);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(697, 319);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "参数配置";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lvwConfig
            // 
            this.lvwConfig.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhChannelIndex,
            this.clhChannelComType,
            this.clhChannelComPara,
            this.clhChannelComSetting});
            this.lvwConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwConfig.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvwConfig.FullRowSelect = true;
            this.lvwConfig.GridLines = true;
            this.lvwConfig.Location = new System.Drawing.Point(3, 3);
            this.lvwConfig.Name = "lvwConfig";
            this.lvwConfig.Size = new System.Drawing.Size(691, 313);
            this.lvwConfig.TabIndex = 0;
            this.lvwConfig.UseCompatibleStateImageBehavior = false;
            this.lvwConfig.View = System.Windows.Forms.View.Details;
            this.lvwConfig.SelectedIndexChanged += new System.EventHandler(this.lvwConfig_SelectedIndexChanged);
            this.lvwConfig.DoubleClick += new System.EventHandler(this.lvwConfig_DoubleClick);
            // 
            // clhChannelIndex
            // 
            this.clhChannelIndex.Text = "通道号";
            this.clhChannelIndex.Width = 72;
            // 
            // clhChannelComType
            // 
            this.clhChannelComType.Text = "通道端口库/端口类";
            this.clhChannelComType.Width = 200;
            // 
            // clhChannelComPara
            // 
            this.clhChannelComPara.Text = "端口参数";
            this.clhChannelComPara.Width = 250;
            // 
            // clhChannelComSetting
            // 
            this.clhChannelComSetting.Text = "通信参数";
            this.clhChannelComSetting.Width = 140;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.Location = new System.Drawing.Point(195, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(92, 39);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "保存";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(292, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 39);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "退出";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 346);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(705, 50);
            this.panel1.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(315, 50);
            this.label9.TabIndex = 12;
            this.label9.Text = "注意：此参数配置界面是用于设置各表的接口的布线方式。请勿随意改动！";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Controls.Add(this.btnOK);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(315, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(390, 50);
            this.panel3.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(705, 346);
            this.panel2.TabIndex = 4;
            // 
            // pnlEditPara
            // 
            this.pnlEditPara.Controls.Add(this.cmb_Setting);
            this.pnlEditPara.Controls.Add(this.label17);
            this.pnlEditPara.Controls.Add(this.btnOkEdit);
            this.pnlEditPara.Controls.Add(this.btnCancelEdit);
            this.pnlEditPara.Controls.Add(this.txtBindPort);
            this.pnlEditPara.Controls.Add(this.label7);
            this.pnlEditPara.Controls.Add(this.txtRomtPort);
            this.pnlEditPara.Controls.Add(this.label6);
            this.pnlEditPara.Controls.Add(this.txtIpAddr);
            this.pnlEditPara.Controls.Add(this.label5);
            this.pnlEditPara.Controls.Add(this.cmbComNo);
            this.pnlEditPara.Controls.Add(this.label4);
            this.pnlEditPara.Controls.Add(this.cmbComClass);
            this.pnlEditPara.Controls.Add(this.label3);
            this.pnlEditPara.Controls.Add(this.cmbComLib);
            this.pnlEditPara.Controls.Add(this.label2);
            this.pnlEditPara.Controls.Add(this.txtChannelNo);
            this.pnlEditPara.Controls.Add(this.label1);
            this.pnlEditPara.Location = new System.Drawing.Point(226, 55);
            this.pnlEditPara.Name = "pnlEditPara";
            this.pnlEditPara.Size = new System.Drawing.Size(263, 267);
            this.pnlEditPara.TabIndex = 5;
            this.pnlEditPara.Visible = false;
            // 
            // cmb_Setting
            // 
            this.cmb_Setting.FormattingEnabled = true;
            this.cmb_Setting.Items.AddRange(new object[] {
            "1200,e,8,1",
            "2400,e,8,1",
            "9600,e,8,1",
            "9600,n,8,1",
            "300,e,8,1",
            "300,e,7,1"});
            this.cmb_Setting.Location = new System.Drawing.Point(83, 186);
            this.cmb_Setting.Name = "cmb_Setting";
            this.cmb_Setting.Size = new System.Drawing.Size(157, 20);
            this.cmb_Setting.TabIndex = 31;
            this.cmb_Setting.Text = "9600,e,8,1";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(29, 189);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(49, 14);
            this.label17.TabIndex = 30;
            this.label17.Text = "波特率";
            // 
            // btnOkEdit
            // 
            this.btnOkEdit.Location = new System.Drawing.Point(84, 227);
            this.btnOkEdit.Name = "btnOkEdit";
            this.btnOkEdit.Size = new System.Drawing.Size(75, 30);
            this.btnOkEdit.TabIndex = 15;
            this.btnOkEdit.Text = "确定";
            this.btnOkEdit.UseVisualStyleBackColor = true;
            this.btnOkEdit.Click += new System.EventHandler(this.btnOkEdit_Click);
            // 
            // btnCancelEdit
            // 
            this.btnCancelEdit.Location = new System.Drawing.Point(165, 227);
            this.btnCancelEdit.Name = "btnCancelEdit";
            this.btnCancelEdit.Size = new System.Drawing.Size(75, 30);
            this.btnCancelEdit.TabIndex = 14;
            this.btnCancelEdit.Text = "取消";
            this.btnCancelEdit.UseVisualStyleBackColor = true;
            this.btnCancelEdit.Click += new System.EventHandler(this.btnCancelEdit_Click);
            // 
            // txtBindPort
            // 
            this.txtBindPort.Location = new System.Drawing.Point(83, 159);
            this.txtBindPort.Name = "txtBindPort";
            this.txtBindPort.Size = new System.Drawing.Size(157, 21);
            this.txtBindPort.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(14, 163);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 14);
            this.label7.TabIndex = 12;
            this.label7.Text = "绑定Port";
            // 
            // txtRomtPort
            // 
            this.txtRomtPort.Location = new System.Drawing.Point(83, 134);
            this.txtRomtPort.Name = "txtRomtPort";
            this.txtRomtPort.Size = new System.Drawing.Size(157, 21);
            this.txtRomtPort.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(14, 138);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 14);
            this.label6.TabIndex = 10;
            this.label6.Text = "远程Port";
            // 
            // txtIpAddr
            // 
            this.txtIpAddr.Location = new System.Drawing.Point(83, 109);
            this.txtIpAddr.Name = "txtIpAddr";
            this.txtIpAddr.Size = new System.Drawing.Size(157, 21);
            this.txtIpAddr.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(28, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 14);
            this.label5.TabIndex = 8;
            this.label5.Text = "IP地址";
            // 
            // cmbComNo
            // 
            this.cmbComNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComNo.FormattingEnabled = true;
            this.cmbComNo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33"});
            this.cmbComNo.Location = new System.Drawing.Point(83, 85);
            this.cmbComNo.Name = "cmbComNo";
            this.cmbComNo.Size = new System.Drawing.Size(157, 20);
            this.cmbComNo.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(28, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 14);
            this.label4.TabIndex = 6;
            this.label4.Text = "串口号";
            // 
            // cmbComClass
            // 
            this.cmbComClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComClass.FormattingEnabled = true;
            this.cmbComClass.Location = new System.Drawing.Point(83, 61);
            this.cmbComClass.Name = "cmbComClass";
            this.cmbComClass.Size = new System.Drawing.Size(157, 20);
            this.cmbComClass.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(28, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 14);
            this.label3.TabIndex = 4;
            this.label3.Text = "串口类";
            // 
            // cmbComLib
            // 
            this.cmbComLib.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComLib.FormattingEnabled = true;
            this.cmbComLib.Location = new System.Drawing.Point(83, 37);
            this.cmbComLib.Name = "cmbComLib";
            this.cmbComLib.Size = new System.Drawing.Size(157, 20);
            this.cmbComLib.TabIndex = 3;
            this.cmbComLib.SelectedValueChanged += new System.EventHandler(this.cmbComLib_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(28, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "串口库";
            // 
            // txtChannelNo
            // 
            this.txtChannelNo.Location = new System.Drawing.Point(83, 12);
            this.txtChannelNo.Name = "txtChannelNo";
            this.txtChannelNo.ReadOnly = true;
            this.txtChannelNo.Size = new System.Drawing.Size(157, 21);
            this.txtChannelNo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(28, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "通道号";
            // 
            // frmParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 396);
            this.Controls.Add(this.pnlEditPara);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmParameter";
            this.Text = "装置表位通信端口配置";
            this.Load += new System.EventHandler(this.frmParameter_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlEditPara.ResumeLayout(false);
            this.pnlEditPara.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListView lvwConfig;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ColumnHeader clhChannelIndex;
        private System.Windows.Forms.ColumnHeader clhChannelComType;
        private System.Windows.Forms.ColumnHeader clhChannelComPara;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnlEditPara;
        private System.Windows.Forms.TextBox txtChannelNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbComLib;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtIpAddr;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbComNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbComClass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBindPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtRomtPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnOkEdit;
        private System.Windows.Forms.Button btnCancelEdit;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmb_Setting;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ColumnHeader clhChannelComSetting;
    }
}
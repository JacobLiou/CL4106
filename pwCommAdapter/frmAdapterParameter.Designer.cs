namespace pwCommAdapter
{
    partial class frmAdapterParameter
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_Add188Channel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpg_CL3000D = new System.Windows.Forms.TabPage();
            this.pnl_3000_EditPara = new System.Windows.Forms.Panel();
            this.txt_3000_Channel = new System.Windows.Forms.TextBox();
            this.lab_Channel = new System.Windows.Forms.Label();
            this.cmb_3000_Setting = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cmb_3000_PClass = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.cmb_3000_PLib = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.btn_3000_OkEdit = new System.Windows.Forms.Button();
            this.btn_3000_Cancel = new System.Windows.Forms.Button();
            this.txt_3000_BPort = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_3000_RPort = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txt_3000_IP = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmb_3000_ComNo = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cmb_3000_ComClass = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cmb_3000_ComLib = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.lvw_Cl3000DPara = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tpg_CL3000D.SuspendLayout();
            this.pnl_3000_EditPara.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 317);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(769, 54);
            this.panel1.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(0, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(350, 54);
            this.label8.TabIndex = 29;
            this.label8.Text = "注意:此参数是用于控制台架功率源,标准表,误差板,时基源;实际参数请与装置布线相同。特别提示：前装台误差板A、B组序号同为：123；功耗板地址为：123456";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btn_Add188Channel);
            this.panel3.Controls.Add(this.btn_OK);
            this.panel3.Controls.Add(this.btn_Close);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(350, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(419, 54);
            this.panel3.TabIndex = 0;
            // 
            // btn_Add188Channel
            // 
            this.btn_Add188Channel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Add188Channel.Location = new System.Drawing.Point(54, 6);
            this.btn_Add188Channel.Name = "btn_Add188Channel";
            this.btn_Add188Channel.Size = new System.Drawing.Size(96, 41);
            this.btn_Add188Channel.TabIndex = 4;
            this.btn_Add188Channel.Text = "添加误差板通道";
            this.btn_Add188Channel.UseVisualStyleBackColor = true;
            this.btn_Add188Channel.Visible = false;
            this.btn_Add188Channel.Click += new System.EventHandler(this.btn_Add188Channel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_OK.Location = new System.Drawing.Point(217, 6);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(96, 41);
            this.btn_OK.TabIndex = 1;
            this.btn_OK.Text = "保存　";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Close.Location = new System.Drawing.Point(319, 6);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(96, 41);
            this.btn_Close.TabIndex = 0;
            this.btn_Close.Text = "退出";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(769, 317);
            this.panel2.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpg_CL3000D);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(769, 317);
            this.tabControl1.TabIndex = 0;
            // 
            // tpg_CL3000D
            // 
            this.tpg_CL3000D.Controls.Add(this.pnl_3000_EditPara);
            this.tpg_CL3000D.Controls.Add(this.lvw_Cl3000DPara);
            this.tpg_CL3000D.Location = new System.Drawing.Point(4, 23);
            this.tpg_CL3000D.Name = "tpg_CL3000D";
            this.tpg_CL3000D.Padding = new System.Windows.Forms.Padding(3);
            this.tpg_CL3000D.Size = new System.Drawing.Size(761, 290);
            this.tpg_CL3000D.TabIndex = 1;
            this.tpg_CL3000D.Text = "生产全自动综合台体";
            this.tpg_CL3000D.UseVisualStyleBackColor = true;
            // 
            // pnl_3000_EditPara
            // 
            this.pnl_3000_EditPara.Controls.Add(this.txt_3000_Channel);
            this.pnl_3000_EditPara.Controls.Add(this.lab_Channel);
            this.pnl_3000_EditPara.Controls.Add(this.cmb_3000_Setting);
            this.pnl_3000_EditPara.Controls.Add(this.label17);
            this.pnl_3000_EditPara.Controls.Add(this.cmb_3000_PClass);
            this.pnl_3000_EditPara.Controls.Add(this.label15);
            this.pnl_3000_EditPara.Controls.Add(this.cmb_3000_PLib);
            this.pnl_3000_EditPara.Controls.Add(this.label16);
            this.pnl_3000_EditPara.Controls.Add(this.btn_3000_OkEdit);
            this.pnl_3000_EditPara.Controls.Add(this.btn_3000_Cancel);
            this.pnl_3000_EditPara.Controls.Add(this.txt_3000_BPort);
            this.pnl_3000_EditPara.Controls.Add(this.label9);
            this.pnl_3000_EditPara.Controls.Add(this.txt_3000_RPort);
            this.pnl_3000_EditPara.Controls.Add(this.label10);
            this.pnl_3000_EditPara.Controls.Add(this.txt_3000_IP);
            this.pnl_3000_EditPara.Controls.Add(this.label11);
            this.pnl_3000_EditPara.Controls.Add(this.cmb_3000_ComNo);
            this.pnl_3000_EditPara.Controls.Add(this.label12);
            this.pnl_3000_EditPara.Controls.Add(this.cmb_3000_ComClass);
            this.pnl_3000_EditPara.Controls.Add(this.label13);
            this.pnl_3000_EditPara.Controls.Add(this.cmb_3000_ComLib);
            this.pnl_3000_EditPara.Controls.Add(this.label14);
            this.pnl_3000_EditPara.Location = new System.Drawing.Point(147, 3);
            this.pnl_3000_EditPara.Name = "pnl_3000_EditPara";
            this.pnl_3000_EditPara.Size = new System.Drawing.Size(263, 287);
            this.pnl_3000_EditPara.TabIndex = 6;
            this.pnl_3000_EditPara.Visible = false;
            // 
            // txt_3000_Channel
            // 
            this.txt_3000_Channel.Location = new System.Drawing.Point(91, 226);
            this.txt_3000_Channel.Name = "txt_3000_Channel";
            this.txt_3000_Channel.Size = new System.Drawing.Size(157, 23);
            this.txt_3000_Channel.TabIndex = 31;
            this.txt_3000_Channel.Text = "1/1";
            this.txt_3000_Channel.Visible = false;
            // 
            // lab_Channel
            // 
            this.lab_Channel.AutoSize = true;
            this.lab_Channel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Channel.Location = new System.Drawing.Point(8, 232);
            this.lab_Channel.Name = "lab_Channel";
            this.lab_Channel.Size = new System.Drawing.Size(77, 14);
            this.lab_Channel.TabIndex = 30;
            this.lab_Channel.Text = "误差板路数";
            this.lab_Channel.Visible = false;
            // 
            // cmb_3000_Setting
            // 
            this.cmb_3000_Setting.FormattingEnabled = true;
            this.cmb_3000_Setting.Items.AddRange(new object[] {
            "9600,n,8,1",
            "19200,n,8,1"});
            this.cmb_3000_Setting.Location = new System.Drawing.Point(91, 202);
            this.cmb_3000_Setting.Name = "cmb_3000_Setting";
            this.cmb_3000_Setting.Size = new System.Drawing.Size(157, 22);
            this.cmb_3000_Setting.TabIndex = 29;
            this.cmb_3000_Setting.Text = "19200,n,8,1";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(37, 205);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(49, 14);
            this.label17.TabIndex = 28;
            this.label17.Text = "波特率";
            // 
            // cmb_3000_PClass
            // 
            this.cmb_3000_PClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_3000_PClass.FormattingEnabled = true;
            this.cmb_3000_PClass.Location = new System.Drawing.Point(91, 29);
            this.cmb_3000_PClass.Name = "cmb_3000_PClass";
            this.cmb_3000_PClass.Size = new System.Drawing.Size(157, 22);
            this.cmb_3000_PClass.TabIndex = 19;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(36, 33);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(49, 14);
            this.label15.TabIndex = 18;
            this.label15.Text = "协议类";
            // 
            // cmb_3000_PLib
            // 
            this.cmb_3000_PLib.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_3000_PLib.FormattingEnabled = true;
            this.cmb_3000_PLib.Location = new System.Drawing.Point(91, 5);
            this.cmb_3000_PLib.Name = "cmb_3000_PLib";
            this.cmb_3000_PLib.Size = new System.Drawing.Size(157, 22);
            this.cmb_3000_PLib.TabIndex = 17;
            this.cmb_3000_PLib.SelectedIndexChanged += new System.EventHandler(this.cmb_3000_PLib_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(36, 9);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(49, 14);
            this.label16.TabIndex = 16;
            this.label16.Text = "协议库";
            // 
            // btn_3000_OkEdit
            // 
            this.btn_3000_OkEdit.Location = new System.Drawing.Point(92, 251);
            this.btn_3000_OkEdit.Name = "btn_3000_OkEdit";
            this.btn_3000_OkEdit.Size = new System.Drawing.Size(75, 30);
            this.btn_3000_OkEdit.TabIndex = 15;
            this.btn_3000_OkEdit.Text = "确定";
            this.btn_3000_OkEdit.UseVisualStyleBackColor = true;
            this.btn_3000_OkEdit.Click += new System.EventHandler(this.btn_3000_OkEdit_Click);
            // 
            // btn_3000_Cancel
            // 
            this.btn_3000_Cancel.Location = new System.Drawing.Point(173, 251);
            this.btn_3000_Cancel.Name = "btn_3000_Cancel";
            this.btn_3000_Cancel.Size = new System.Drawing.Size(75, 30);
            this.btn_3000_Cancel.TabIndex = 14;
            this.btn_3000_Cancel.Text = "取消";
            this.btn_3000_Cancel.UseVisualStyleBackColor = true;
            this.btn_3000_Cancel.Click += new System.EventHandler(this.btn_3000_Cancel_Click);
            // 
            // txt_3000_BPort
            // 
            this.txt_3000_BPort.Location = new System.Drawing.Point(91, 177);
            this.txt_3000_BPort.Name = "txt_3000_BPort";
            this.txt_3000_BPort.Size = new System.Drawing.Size(157, 23);
            this.txt_3000_BPort.TabIndex = 13;
            this.txt_3000_BPort.Text = "20000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(22, 181);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 14);
            this.label9.TabIndex = 12;
            this.label9.Text = "绑定Port";
            // 
            // txt_3000_RPort
            // 
            this.txt_3000_RPort.AcceptsReturn = true;
            this.txt_3000_RPort.Location = new System.Drawing.Point(91, 152);
            this.txt_3000_RPort.Name = "txt_3000_RPort";
            this.txt_3000_RPort.Size = new System.Drawing.Size(157, 23);
            this.txt_3000_RPort.TabIndex = 11;
            this.txt_3000_RPort.Text = "10003";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(22, 156);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 14);
            this.label10.TabIndex = 10;
            this.label10.Text = "远程Port";
            // 
            // txt_3000_IP
            // 
            this.txt_3000_IP.Location = new System.Drawing.Point(91, 127);
            this.txt_3000_IP.Name = "txt_3000_IP";
            this.txt_3000_IP.Size = new System.Drawing.Size(157, 23);
            this.txt_3000_IP.TabIndex = 9;
            this.txt_3000_IP.Text = "193.168.18.1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(36, 131);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(49, 14);
            this.label11.TabIndex = 8;
            this.label11.Text = "IP地址";
            // 
            // cmb_3000_ComNo
            // 
            this.cmb_3000_ComNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_3000_ComNo.FormattingEnabled = true;
            this.cmb_3000_ComNo.Items.AddRange(new object[] {
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
            this.cmb_3000_ComNo.Location = new System.Drawing.Point(91, 103);
            this.cmb_3000_ComNo.Name = "cmb_3000_ComNo";
            this.cmb_3000_ComNo.Size = new System.Drawing.Size(157, 22);
            this.cmb_3000_ComNo.TabIndex = 7;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(36, 107);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 14);
            this.label12.TabIndex = 6;
            this.label12.Text = "串口号";
            // 
            // cmb_3000_ComClass
            // 
            this.cmb_3000_ComClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_3000_ComClass.FormattingEnabled = true;
            this.cmb_3000_ComClass.Location = new System.Drawing.Point(91, 79);
            this.cmb_3000_ComClass.Name = "cmb_3000_ComClass";
            this.cmb_3000_ComClass.Size = new System.Drawing.Size(157, 22);
            this.cmb_3000_ComClass.TabIndex = 5;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(36, 83);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 14);
            this.label13.TabIndex = 4;
            this.label13.Text = "串口类";
            // 
            // cmb_3000_ComLib
            // 
            this.cmb_3000_ComLib.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_3000_ComLib.FormattingEnabled = true;
            this.cmb_3000_ComLib.Location = new System.Drawing.Point(91, 55);
            this.cmb_3000_ComLib.Name = "cmb_3000_ComLib";
            this.cmb_3000_ComLib.Size = new System.Drawing.Size(157, 22);
            this.cmb_3000_ComLib.TabIndex = 3;
            this.cmb_3000_ComLib.SelectedIndexChanged += new System.EventHandler(this.cmb_3000_ComLib_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(36, 59);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(49, 14);
            this.label14.TabIndex = 2;
            this.label14.Text = "串口库";
            // 
            // lvw_Cl3000DPara
            // 
            this.lvw_Cl3000DPara.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvw_Cl3000DPara.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvw_Cl3000DPara.FullRowSelect = true;
            this.lvw_Cl3000DPara.GridLines = true;
            this.lvw_Cl3000DPara.Location = new System.Drawing.Point(3, 3);
            this.lvw_Cl3000DPara.MultiSelect = false;
            this.lvw_Cl3000DPara.Name = "lvw_Cl3000DPara";
            this.lvw_Cl3000DPara.Size = new System.Drawing.Size(755, 284);
            this.lvw_Cl3000DPara.TabIndex = 0;
            this.lvw_Cl3000DPara.UseCompatibleStateImageBehavior = false;
            this.lvw_Cl3000DPara.View = System.Windows.Forms.View.Details;
            this.lvw_Cl3000DPara.DoubleClick += new System.EventHandler(this.lvw_Cl3000DPara_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "仪器仪表";
            this.columnHeader1.Width = 98;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "协议库/协议类";
            this.columnHeader2.Width = 129;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "串口库/串口类";
            this.columnHeader3.Width = 135;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "串口号参数";
            this.columnHeader4.Width = 124;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "通信参数";
            this.columnHeader5.Width = 133;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "控制参数";
            this.columnHeader6.Width = 113;
            // 
            // frmAdapterParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 371);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmAdapterParameter";
            this.Text = "装置控制模块参数配置";
            this.Load += new System.EventHandler(this.frmAdapterParameter_Load);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tpg_CL3000D.ResumeLayout(false);
            this.pnl_3000_EditPara.ResumeLayout(false);
            this.pnl_3000_EditPara.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpg_CL3000D;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListView lvw_Cl3000DPara;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Panel pnl_3000_EditPara;
        private System.Windows.Forms.Button btn_3000_OkEdit;
        private System.Windows.Forms.Button btn_3000_Cancel;
        private System.Windows.Forms.TextBox txt_3000_BPort;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txt_3000_RPort;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_3000_IP;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmb_3000_ComNo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmb_3000_ComClass;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmb_3000_ComLib;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cmb_3000_Setting;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox cmb_3000_PClass;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmb_3000_PLib;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txt_3000_Channel;
        private System.Windows.Forms.Label lab_Channel;
        private System.Windows.Forms.Button btn_Add188Channel;
    }
}
namespace CL4100
{
    partial class pw_Main
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.FaNamelist = new System.Windows.Forms.ListBox();
            this.labSchemaDecription = new System.Windows.Forms.Label();
            this.txtSchemaDescription = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbModel = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbProductName = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.PlantreeView = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.but_Delete = new System.Windows.Forms.Button();
            this.but_Add = new System.Windows.Forms.Button();
            this.but_Save = new System.Windows.Forms.Button();
            this.lab_Info = new System.Windows.Forms.Label();
            this.btnCoppy = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(874, 537);
            this.splitContainer1.SplitterDistance = 192;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(192, 537);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox1);
            this.groupBox4.Controls.Add(this.FaNamelist);
            this.groupBox4.Controls.Add(this.labSchemaDecription);
            this.groupBox4.Controls.Add(this.txtSchemaDescription);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(3, 313);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(186, 221);
            this.groupBox4.TabIndex = 62;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "方案名称";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(117, 144);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(49, 21);
            this.textBox1.TabIndex = 56;
            this.textBox1.Visible = false;
            // 
            // FaNamelist
            // 
            this.FaNamelist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FaNamelist.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FaNamelist.FormattingEnabled = true;
            this.FaNamelist.HorizontalScrollbar = true;
            this.FaNamelist.ItemHeight = 15;
            this.FaNamelist.Location = new System.Drawing.Point(3, 19);
            this.FaNamelist.Name = "FaNamelist";
            this.FaNamelist.Size = new System.Drawing.Size(180, 124);
            this.FaNamelist.TabIndex = 93;
            this.FaNamelist.SelectedIndexChanged += new System.EventHandler(this.FaNamelist_SelectedIndexChanged);
            // 
            // labSchemaDecription
            // 
            this.labSchemaDecription.AutoSize = true;
            this.labSchemaDecription.Location = new System.Drawing.Point(6, 153);
            this.labSchemaDecription.Name = "labSchemaDecription";
            this.labSchemaDecription.Size = new System.Drawing.Size(53, 12);
            this.labSchemaDecription.TabIndex = 37;
            this.labSchemaDecription.Text = "方案描述";
            // 
            // txtSchemaDescription
            // 
            this.txtSchemaDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSchemaDescription.Location = new System.Drawing.Point(3, 170);
            this.txtSchemaDescription.Multiline = true;
            this.txtSchemaDescription.Name = "txtSchemaDescription";
            this.txtSchemaDescription.Size = new System.Drawing.Size(180, 44);
            this.txtSchemaDescription.TabIndex = 37;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbModel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(186, 117);
            this.groupBox1.TabIndex = 59;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "表型号";
            // 
            // lbModel
            // 
            this.lbModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbModel.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbModel.FormattingEnabled = true;
            this.lbModel.ItemHeight = 15;
            this.lbModel.Location = new System.Drawing.Point(3, 17);
            this.lbModel.Name = "lbModel";
            this.lbModel.Size = new System.Drawing.Size(180, 97);
            this.lbModel.TabIndex = 92;
            this.lbModel.SelectedIndexChanged += new System.EventHandler(this.lbModel_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbProductName);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 126);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(186, 181);
            this.groupBox3.TabIndex = 61;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "成品料号";
            // 
            // lbProductName
            // 
            this.lbProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbProductName.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbProductName.FormattingEnabled = true;
            this.lbProductName.HorizontalScrollbar = true;
            this.lbProductName.ItemHeight = 15;
            this.lbProductName.Location = new System.Drawing.Point(3, 17);
            this.lbProductName.Name = "lbProductName";
            this.lbProductName.Size = new System.Drawing.Size(180, 161);
            this.lbProductName.TabIndex = 92;
            this.lbProductName.SelectedIndexChanged += new System.EventHandler(this.lbProductName_SelectedIndexChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lab_Info);
            this.splitContainer2.Size = new System.Drawing.Size(678, 537);
            this.splitContainer2.SplitterDistance = 161;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.btnCoppy);
            this.splitContainer3.Panel2.Controls.Add(this.label1);
            this.splitContainer3.Panel2.Controls.Add(this.but_Delete);
            this.splitContainer3.Panel2.Controls.Add(this.but_Add);
            this.splitContainer3.Panel2.Controls.Add(this.but_Save);
            this.splitContainer3.Size = new System.Drawing.Size(161, 537);
            this.splitContainer3.SplitterDistance = 305;
            this.splitContainer3.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.PlantreeView);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(161, 305);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "方案项目";
            // 
            // PlantreeView
            // 
            this.PlantreeView.CheckBoxes = true;
            this.PlantreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlantreeView.HideSelection = false;
            this.PlantreeView.Location = new System.Drawing.Point(3, 17);
            this.PlantreeView.Name = "PlantreeView";
            this.PlantreeView.Size = new System.Drawing.Size(155, 285);
            this.PlantreeView.TabIndex = 0;
            this.PlantreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.PlantreeView_BeforeSelect);
            this.PlantreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.PlantreeView_AfterSelect);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(7, 202);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 16);
            this.label1.TabIndex = 56;
            this.label1.Text = "三相检定方案配配置";
            // 
            // but_Delete
            // 
            this.but_Delete.Location = new System.Drawing.Point(17, 91);
            this.but_Delete.Name = "but_Delete";
            this.but_Delete.Size = new System.Drawing.Size(130, 29);
            this.but_Delete.TabIndex = 55;
            this.but_Delete.Text = "删除方案";
            this.but_Delete.UseVisualStyleBackColor = true;
            this.but_Delete.Click += new System.EventHandler(this.but_Delete_Click);
            // 
            // but_Add
            // 
            this.but_Add.Location = new System.Drawing.Point(17, 14);
            this.but_Add.Name = "but_Add";
            this.but_Add.Size = new System.Drawing.Size(130, 33);
            this.but_Add.TabIndex = 54;
            this.but_Add.Text = "新增方案";
            this.but_Add.UseVisualStyleBackColor = true;
            this.but_Add.Click += new System.EventHandler(this.but_Add_Click);
            // 
            // but_Save
            // 
            this.but_Save.Location = new System.Drawing.Point(17, 53);
            this.but_Save.Name = "but_Save";
            this.but_Save.Size = new System.Drawing.Size(130, 32);
            this.but_Save.TabIndex = 53;
            this.but_Save.Text = "保存方案";
            this.but_Save.UseVisualStyleBackColor = true;
            this.but_Save.Click += new System.EventHandler(this.but_Save_Click);
            // 
            // lab_Info
            // 
            this.lab_Info.BackColor = System.Drawing.Color.LightGray;
            this.lab_Info.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lab_Info.Location = new System.Drawing.Point(0, 525);
            this.lab_Info.Name = "lab_Info";
            this.lab_Info.Size = new System.Drawing.Size(513, 12);
            this.lab_Info.TabIndex = 0;
            this.lab_Info.Text = "sdfsafsasa";
            // 
            // btnCoppy
            // 
            this.btnCoppy.Location = new System.Drawing.Point(17, 126);
            this.btnCoppy.Name = "btnCoppy";
            this.btnCoppy.Size = new System.Drawing.Size(130, 29);
            this.btnCoppy.TabIndex = 57;
            this.btnCoppy.Text = "复制方案";
            this.btnCoppy.UseVisualStyleBackColor = true;
            this.btnCoppy.Click += new System.EventHandler(this.btnCoppy_Click);
            // 
            // pw_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 537);
            this.Controls.Add(this.splitContainer1);
            this.Name = "pw_Main";
            this.Text = "海外三相检定方案配置";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.pw_Main_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbModel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lbProductName;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button but_Add;
        private System.Windows.Forms.Label labSchemaDecription;
        private System.Windows.Forms.TextBox txtSchemaDescription;
        private System.Windows.Forms.Button but_Save;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView PlantreeView;
        private System.Windows.Forms.ListBox FaNamelist;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button but_Delete;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lab_Info;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCoppy;
    }
}
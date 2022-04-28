namespace pwFunction.pwSystemModel
{
    partial class UI_SystemManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI_SystemManager));
            this.Cmd_Ok = new System.Windows.Forms.Button();
            this.Cmd_Close = new System.Windows.Forms.Button();
            this.Page_System = new System.Windows.Forms.TabPage();
            this.SystemProperty = new PropertyGridEx.PropertyGridEx();
            this.Tab_Control = new System.Windows.Forms.TabControl();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.Page_System.SuspendLayout();
            this.Tab_Control.SuspendLayout();
            this.SuspendLayout();
            // 
            // Cmd_Ok
            // 
            this.Cmd_Ok.Location = new System.Drawing.Point(182, 396);
            this.Cmd_Ok.Name = "Cmd_Ok";
            this.Cmd_Ok.Size = new System.Drawing.Size(108, 23);
            this.Cmd_Ok.TabIndex = 1;
            this.Cmd_Ok.Text = "确  认(&O)";
            this.Cmd_Ok.UseVisualStyleBackColor = true;
            this.Cmd_Ok.Click += new System.EventHandler(this.Cmd_Ok_Click);
            // 
            // Cmd_Close
            // 
            this.Cmd_Close.Location = new System.Drawing.Point(295, 396);
            this.Cmd_Close.Name = "Cmd_Close";
            this.Cmd_Close.Size = new System.Drawing.Size(108, 23);
            this.Cmd_Close.TabIndex = 2;
            this.Cmd_Close.Text = "关  闭(&C)";
            this.Cmd_Close.UseVisualStyleBackColor = true;
            this.Cmd_Close.Click += new System.EventHandler(this.Cmd_Close_Click);
            // 
            // Page_System
            // 
            this.Page_System.Controls.Add(this.SystemProperty);
            this.Page_System.Location = new System.Drawing.Point(4, 21);
            this.Page_System.Name = "Page_System";
            this.Page_System.Padding = new System.Windows.Forms.Padding(3);
            this.Page_System.Size = new System.Drawing.Size(430, 353);
            this.Page_System.TabIndex = 0;
            this.Page_System.Text = "系统设置";
            this.Page_System.UseVisualStyleBackColor = true;
            // 
            // SystemProperty
            // 
            this.SystemProperty.CommandsDisabledLinkColor = System.Drawing.Color.Maroon;
            this.SystemProperty.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // 
            // 
            this.SystemProperty.DocCommentDescription.AutoEllipsis = true;
            this.SystemProperty.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.SystemProperty.DocCommentDescription.Location = new System.Drawing.Point(3, 19);
            this.SystemProperty.DocCommentDescription.Name = "";
            this.SystemProperty.DocCommentDescription.Size = new System.Drawing.Size(418, 36);
            this.SystemProperty.DocCommentDescription.TabIndex = 1;
            this.SystemProperty.DocCommentImage = null;
            // 
            // 
            // 
            this.SystemProperty.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.SystemProperty.DocCommentTitle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.SystemProperty.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.SystemProperty.DocCommentTitle.Name = "";
            this.SystemProperty.DocCommentTitle.Size = new System.Drawing.Size(418, 16);
            this.SystemProperty.DocCommentTitle.TabIndex = 0;
            this.SystemProperty.DocCommentTitle.UseMnemonic = false;
            this.SystemProperty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SystemProperty.LineColor = System.Drawing.SystemColors.ActiveBorder;
            this.SystemProperty.Location = new System.Drawing.Point(3, 3);
            this.SystemProperty.Margin = new System.Windows.Forms.Padding(0);
            this.SystemProperty.Name = "SystemProperty";
            this.SystemProperty.Size = new System.Drawing.Size(424, 347);
            this.SystemProperty.TabIndex = 0;
            // 
            // 
            // 
            this.SystemProperty.ToolStrip.AccessibleName = "工具栏";
            this.SystemProperty.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.SystemProperty.ToolStrip.AllowMerge = false;
            this.SystemProperty.ToolStrip.AutoSize = false;
            this.SystemProperty.ToolStrip.CanOverflow = false;
            this.SystemProperty.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.SystemProperty.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.SystemProperty.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.SystemProperty.ToolStrip.Name = "";
            this.SystemProperty.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.SystemProperty.ToolStrip.Size = new System.Drawing.Size(424, 25);
            this.SystemProperty.ToolStrip.TabIndex = 1;
            this.SystemProperty.ToolStrip.TabStop = true;
            this.SystemProperty.ToolStrip.Text = "PropertyGridToolBar";
            // 
            // Tab_Control
            // 
            this.Tab_Control.Controls.Add(this.Page_System);
            this.Tab_Control.Location = new System.Drawing.Point(2, 12);
            this.Tab_Control.Multiline = true;
            this.Tab_Control.Name = "Tab_Control";
            this.Tab_Control.SelectedIndex = 0;
            this.Tab_Control.Size = new System.Drawing.Size(438, 378);
            this.Tab_Control.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(280, 299);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "移  除";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(154, 299);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(109, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "添  加";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.listView1.LabelEdit = true;
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(154, 6);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(235, 287);
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // treeView1
            // 
            this.treeView1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeView1.FullRowSelect = true;
            this.treeView1.LineColor = System.Drawing.Color.Empty;
            this.treeView1.Location = new System.Drawing.Point(3, 6);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(149, 316);
            this.treeView1.TabIndex = 0;
            // 
            // UI_SystemManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(441, 431);
            this.Controls.Add(this.Cmd_Close);
            this.Controls.Add(this.Cmd_Ok);
            this.Controls.Add(this.Tab_Control);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(449, 465);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(449, 465);
            this.Name = "UI_SystemManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统信息配置";
            this.TopMost = true;
            this.Page_System.ResumeLayout(false);
            this.Tab_Control.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Cmd_Ok;
        private System.Windows.Forms.Button Cmd_Close;
        private System.Windows.Forms.TabPage Page_System;
        private PropertyGridEx.PropertyGridEx SystemProperty;
        private System.Windows.Forms.TabControl Tab_Control;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TreeView treeView1;
    }
}
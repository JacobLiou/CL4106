namespace pwClassLibrary
{
    partial class TopWaiting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopWaiting));
            this.Table_Main_2 = new System.Windows.Forms.TableLayoutPanel();
            this.Lab_Notice = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Lab_Close = new System.Windows.Forms.Label();
            this.Table_Main = new System.Windows.Forms.TableLayoutPanel();
            this.Table_Main_2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.Table_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // Table_Main_2
            // 
            this.Table_Main_2.ColumnCount = 5;
            this.Table_Main_2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.Table_Main_2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.Table_Main_2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.Table_Main_2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.Table_Main_2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.Table_Main_2.Controls.Add(this.Lab_Notice, 3, 1);
            this.Table_Main_2.Controls.Add(this.pictureBox1, 1, 1);
            this.Table_Main_2.Controls.Add(this.Lab_Close, 4, 0);
            this.Table_Main_2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Table_Main_2.Location = new System.Drawing.Point(3, 3);
            this.Table_Main_2.Margin = new System.Windows.Forms.Padding(1);
            this.Table_Main_2.Name = "Table_Main_2";
            this.Table_Main_2.RowCount = 3;
            this.Table_Main_2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Table_Main_2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.Table_Main_2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Table_Main_2.Size = new System.Drawing.Size(598, 94);
            this.Table_Main_2.TabIndex = 0;
            // 
            // Lab_Notice
            // 
            this.Lab_Notice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Lab_Notice.Font = new System.Drawing.Font("黑体", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Lab_Notice.ForeColor = System.Drawing.Color.Green;
            this.Lab_Notice.Location = new System.Drawing.Point(148, 18);
            this.Lab_Notice.Margin = new System.Windows.Forms.Padding(1);
            this.Lab_Notice.Name = "Lab_Notice";
            this.Lab_Notice.Size = new System.Drawing.Size(335, 58);
            this.Lab_Notice.TabIndex = 0;
            this.Lab_Notice.Text = "系统正在处理...";
            this.Lab_Notice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(112, 31);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0, 14, 0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 46);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // Lab_Close
            // 
            this.Lab_Close.AutoSize = true;
            this.Lab_Close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Lab_Close.Dock = System.Windows.Forms.DockStyle.Right;
            this.Lab_Close.Location = new System.Drawing.Point(578, 0);
            this.Lab_Close.Name = "Lab_Close";
            this.Lab_Close.Size = new System.Drawing.Size(17, 17);
            this.Lab_Close.TabIndex = 2;
            this.Lab_Close.Text = "×";
            this.Lab_Close.Click += new System.EventHandler(this.Lab_Close_Click);
            // 
            // Table_Main
            // 
            this.Table_Main.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.Table_Main.ColumnCount = 1;
            this.Table_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Table_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Table_Main.Controls.Add(this.Table_Main_2, 0, 0);
            this.Table_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Table_Main.Location = new System.Drawing.Point(0, 0);
            this.Table_Main.Margin = new System.Windows.Forms.Padding(0);
            this.Table_Main.Name = "Table_Main";
            this.Table_Main.RowCount = 1;
            this.Table_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Table_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Table_Main.Size = new System.Drawing.Size(604, 100);
            this.Table_Main.TabIndex = 1;
            // 
            // TopWaiting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(604, 100);
            this.Controls.Add(this.Table_Main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.HelpButton = true;
            this.Name = "TopWaiting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "系统正在处理";
            this.TopMost = true;
            this.Table_Main_2.ResumeLayout(false);
            this.Table_Main_2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.Table_Main.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel Table_Main_2;
        private System.Windows.Forms.Label Lab_Notice;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel Table_Main;
        private System.Windows.Forms.Label Lab_Close;
    }
}
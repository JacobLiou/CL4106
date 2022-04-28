namespace CL4100.Plan3Phase
{
    partial class Plan_AdjustErrorUserControl
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
            this.Tab_Info = new System.Windows.Forms.TabControl();
            this.Pag_Result = new System.Windows.Forms.TabPage();
            this.btnChooseiniFile = new System.Windows.Forms.Button();
            this.txt_Len = new System.Windows.Forms.TextBox();
            this.txt_Data = new System.Windows.Forms.TextBox();
            this.txt_Dot = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_Code = new System.Windows.Forms.TextBox();
            this.cmb_Xieyi = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_ParaFile = new System.Windows.Forms.ComboBox();
            this.dgv_Para = new System.Windows.Forms.DataGridView();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tab_Info.SuspendLayout();
            this.Pag_Result.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Para)).BeginInit();
            this.SuspendLayout();
            // 
            // Tab_Info
            // 
            this.Tab_Info.Controls.Add(this.Pag_Result);
            this.Tab_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tab_Info.Location = new System.Drawing.Point(0, 0);
            this.Tab_Info.Name = "Tab_Info";
            this.Tab_Info.SelectedIndex = 0;
            this.Tab_Info.Size = new System.Drawing.Size(594, 540);
            this.Tab_Info.TabIndex = 6;
            // 
            // Pag_Result
            // 
            this.Pag_Result.Controls.Add(this.dgv_Para);
            this.Pag_Result.Controls.Add(this.cmb_ParaFile);
            this.Pag_Result.Controls.Add(this.txt_Len);
            this.Pag_Result.Controls.Add(this.txt_Data);
            this.Pag_Result.Controls.Add(this.txt_Dot);
            this.Pag_Result.Controls.Add(this.label5);
            this.Pag_Result.Controls.Add(this.label3);
            this.Pag_Result.Controls.Add(this.label4);
            this.Pag_Result.Controls.Add(this.txt_Code);
            this.Pag_Result.Controls.Add(this.cmb_Xieyi);
            this.Pag_Result.Controls.Add(this.label1);
            this.Pag_Result.Controls.Add(this.label2);
            this.Pag_Result.Controls.Add(this.btnChooseiniFile);
            this.Pag_Result.Location = new System.Drawing.Point(4, 21);
            this.Pag_Result.Name = "Pag_Result";
            this.Pag_Result.Padding = new System.Windows.Forms.Padding(3);
            this.Pag_Result.Size = new System.Drawing.Size(586, 515);
            this.Pag_Result.TabIndex = 0;
            this.Pag_Result.Text = "校准误差";
            this.Pag_Result.UseVisualStyleBackColor = true;
            // 
            // btnChooseiniFile
            // 
            this.btnChooseiniFile.Enabled = false;
            this.btnChooseiniFile.Location = new System.Drawing.Point(53, 222);
            this.btnChooseiniFile.Name = "btnChooseiniFile";
            this.btnChooseiniFile.Size = new System.Drawing.Size(76, 23);
            this.btnChooseiniFile.TabIndex = 105;
            this.btnChooseiniFile.Text = "选择文件";
            this.btnChooseiniFile.UseVisualStyleBackColor = true;
            // 
            // txt_Len
            // 
            this.txt_Len.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Len.Location = new System.Drawing.Point(148, 107);
            this.txt_Len.Name = "txt_Len";
            this.txt_Len.Size = new System.Drawing.Size(193, 21);
            this.txt_Len.TabIndex = 113;
            this.txt_Len.Text = "12";
            // 
            // txt_Data
            // 
            this.txt_Data.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Data.Location = new System.Drawing.Point(148, 188);
            this.txt_Data.Name = "txt_Data";
            this.txt_Data.Size = new System.Drawing.Size(193, 21);
            this.txt_Data.TabIndex = 115;
            // 
            // txt_Dot
            // 
            this.txt_Dot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Dot.Location = new System.Drawing.Point(148, 148);
            this.txt_Dot.Name = "txt_Dot";
            this.txt_Dot.Size = new System.Drawing.Size(193, 21);
            this.txt_Dot.TabIndex = 114;
            this.txt_Dot.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 191);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 112;
            this.label5.Text = "⑤下发参数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(51, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 110;
            this.label3.Text = "③数据长度";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 111;
            this.label4.Text = "④小数点";
            // 
            // txt_Code
            // 
            this.txt_Code.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Code.Location = new System.Drawing.Point(148, 67);
            this.txt_Code.Name = "txt_Code";
            this.txt_Code.Size = new System.Drawing.Size(193, 21);
            this.txt_Code.TabIndex = 109;
            this.txt_Code.Text = "FFF9";
            // 
            // cmb_Xieyi
            // 
            this.cmb_Xieyi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Xieyi.FormattingEnabled = true;
            this.cmb_Xieyi.Items.AddRange(new object[] {
            "DLT645_1997",
            "DLT645_2007"});
            this.cmb_Xieyi.Location = new System.Drawing.Point(148, 26);
            this.cmb_Xieyi.Name = "cmb_Xieyi";
            this.cmb_Xieyi.Size = new System.Drawing.Size(193, 20);
            this.cmb_Xieyi.TabIndex = 108;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 106;
            this.label1.Text = "①协议";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 107;
            this.label2.Text = "②标识编码";
            // 
            // cmb_ParaFile
            // 
            this.cmb_ParaFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_ParaFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ParaFile.FormattingEnabled = true;
            this.cmb_ParaFile.Location = new System.Drawing.Point(148, 222);
            this.cmb_ParaFile.Name = "cmb_ParaFile";
            this.cmb_ParaFile.Size = new System.Drawing.Size(361, 20);
            this.cmb_ParaFile.TabIndex = 116;
            this.cmb_ParaFile.SelectedIndexChanged += new System.EventHandler(this.cmb_ParaFile_SelectedIndexChanged);
            // 
            // dgv_Para
            // 
            this.dgv_Para.AllowUserToAddRows = false;
            this.dgv_Para.AllowUserToDeleteRows = false;
            this.dgv_Para.AllowUserToResizeColumns = false;
            this.dgv_Para.AllowUserToResizeRows = false;
            this.dgv_Para.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_Para.ColumnHeadersHeight = 25;
            this.dgv_Para.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv_Para.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column6,
            this.dataGridViewComboBoxColumn3,
            this.dataGridViewComboBoxColumn5});
            this.dgv_Para.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgv_Para.Location = new System.Drawing.Point(148, 248);
            this.dgv_Para.MultiSelect = false;
            this.dgv_Para.Name = "dgv_Para";
            this.dgv_Para.RowHeadersWidth = 25;
            this.dgv_Para.RowTemplate.Height = 23;
            this.dgv_Para.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgv_Para.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_Para.Size = new System.Drawing.Size(361, 248);
            this.dgv_Para.TabIndex = 117;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "序号";
            this.Column6.Name = "Column6";
            this.Column6.Width = 50;
            // 
            // dataGridViewComboBoxColumn3
            // 
            this.dataGridViewComboBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewComboBoxColumn3.HeaderText = "参数名称";
            this.dataGridViewComboBoxColumn3.Name = "dataGridViewComboBoxColumn3";
            this.dataGridViewComboBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewComboBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewComboBoxColumn5
            // 
            this.dataGridViewComboBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewComboBoxColumn5.HeaderText = "参数值";
            this.dataGridViewComboBoxColumn5.Name = "dataGridViewComboBoxColumn5";
            this.dataGridViewComboBoxColumn5.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewComboBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Plan_AdjustErrorUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Tab_Info);
            this.Name = "Plan_AdjustErrorUserControl";
            this.Size = new System.Drawing.Size(594, 540);
            this.Tab_Info.ResumeLayout(false);
            this.Pag_Result.ResumeLayout(false);
            this.Pag_Result.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Para)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Tab_Info;
        private System.Windows.Forms.TabPage Pag_Result;
        private System.Windows.Forms.Button btnChooseiniFile;
        private System.Windows.Forms.TextBox txt_Len;
        private System.Windows.Forms.TextBox txt_Data;
        private System.Windows.Forms.TextBox txt_Dot;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_Code;
        private System.Windows.Forms.ComboBox cmb_Xieyi;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_ParaFile;
        private System.Windows.Forms.DataGridView dgv_Para;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewComboBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewComboBoxColumn5;

    }
}

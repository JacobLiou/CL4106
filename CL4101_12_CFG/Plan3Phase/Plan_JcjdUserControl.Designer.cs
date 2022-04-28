namespace CL4100.Plan3Phase
{
    partial class Plan_JcjdUserControl
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
            this.txtP = new System.Windows.Forms.TextBox();
            this.txtI = new System.Windows.Forms.TextBox();
            this.txtV = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvAllPoint = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tab_Info.SuspendLayout();
            this.Pag_Result.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllPoint)).BeginInit();
            this.SuspendLayout();
            // 
            // Tab_Info
            // 
            this.Tab_Info.Controls.Add(this.Pag_Result);
            this.Tab_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tab_Info.Location = new System.Drawing.Point(0, 0);
            this.Tab_Info.Name = "Tab_Info";
            this.Tab_Info.SelectedIndex = 0;
            this.Tab_Info.Size = new System.Drawing.Size(938, 527);
            this.Tab_Info.TabIndex = 6;
            // 
            // Pag_Result
            // 
            this.Pag_Result.Controls.Add(this.txtP);
            this.Pag_Result.Controls.Add(this.txtI);
            this.Pag_Result.Controls.Add(this.txtV);
            this.Pag_Result.Controls.Add(this.label3);
            this.Pag_Result.Controls.Add(this.label2);
            this.Pag_Result.Controls.Add(this.label1);
            this.Pag_Result.Controls.Add(this.dgvAllPoint);
            this.Pag_Result.Location = new System.Drawing.Point(4, 21);
            this.Pag_Result.Name = "Pag_Result";
            this.Pag_Result.Padding = new System.Windows.Forms.Padding(3);
            this.Pag_Result.Size = new System.Drawing.Size(930, 502);
            this.Pag_Result.TabIndex = 0;
            this.Pag_Result.Text = "交流采样测试";
            this.Pag_Result.UseVisualStyleBackColor = true;
            // 
            // txtP
            // 
            this.txtP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtP.Location = new System.Drawing.Point(157, 111);
            this.txtP.Name = "txtP";
            this.txtP.Size = new System.Drawing.Size(132, 21);
            this.txtP.TabIndex = 92;
            this.txtP.Text = "0.4";
            // 
            // txtI
            // 
            this.txtI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtI.Location = new System.Drawing.Point(157, 77);
            this.txtI.Name = "txtI";
            this.txtI.Size = new System.Drawing.Size(132, 21);
            this.txtI.TabIndex = 91;
            this.txtI.Text = "0.4";
            // 
            // txtV
            // 
            this.txtV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtV.Location = new System.Drawing.Point(157, 41);
            this.txtV.Name = "txtV";
            this.txtV.Size = new System.Drawing.Size(132, 21);
            this.txtV.TabIndex = 90;
            this.txtV.Text = "0.4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 12);
            this.label3.TabIndex = 89;
            this.label3.Text = "功率误差上限值(%)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 12);
            this.label2.TabIndex = 88;
            this.label2.Text = "电流误差上限值(%)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 87;
            this.label1.Text = "电压误差上限值(%)";
            // 
            // dgvAllPoint
            // 
            this.dgvAllPoint.AllowUserToAddRows = false;
            this.dgvAllPoint.AllowUserToResizeRows = false;
            this.dgvAllPoint.BackgroundColor = System.Drawing.Color.White;
            this.dgvAllPoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAllPoint.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
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
            this.dgvAllPoint.Location = new System.Drawing.Point(6, 210);
            this.dgvAllPoint.Name = "dgvAllPoint";
            this.dgvAllPoint.RowHeadersVisible = false;
            this.dgvAllPoint.RowTemplate.Height = 23;
            this.dgvAllPoint.Size = new System.Drawing.Size(780, 260);
            this.dgvAllPoint.TabIndex = 86;
            this.dgvAllPoint.Visible = false;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "AllPoint";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn1.HeaderText = "检测点";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 70;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Un";
            this.dataGridViewTextBoxColumn2.HeaderText = "电压";
            this.dataGridViewTextBoxColumn2.Items.AddRange(new object[] {
            "10%Un",
            "20%Un",
            "30%Un",
            "40%Un",
            "50%Un",
            "60%Un",
            "70%Un",
            "80%Un",
            "90%Un",
            "Un",
            "110%Un",
            "120%Un",
            "\t"});
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn2.Width = 78;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "I";
            this.dataGridViewTextBoxColumn5.HeaderText = "电流";
            this.dataGridViewTextBoxColumn5.Items.AddRange(new object[] {
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
            "0.01Ib",
            "\t"});
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn5.Width = 78;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.DataPropertyName = "GLYS";
            this.dataGridViewTextBoxColumn8.HeaderText = "功率因素";
            this.dataGridViewTextBoxColumn8.Items.AddRange(new object[] {
            "1.0",
            "0.5L",
            "0.8C",
            "0.5C",
            "0.25L",
            "\t"});
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn8.Width = 78;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "UnU";
            this.Column12.HeaderText = "电压上限";
            this.Column12.Name = "Column12";
            this.Column12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column12.Width = 78;
            // 
            // Column13
            // 
            this.Column13.DataPropertyName = "UnD";
            this.Column13.HeaderText = "电压下限";
            this.Column13.Name = "Column13";
            this.Column13.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column13.Width = 78;
            // 
            // Column14
            // 
            this.Column14.DataPropertyName = "IU";
            this.Column14.HeaderText = "电流上限";
            this.Column14.Name = "Column14";
            this.Column14.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column14.Width = 78;
            // 
            // Column15
            // 
            this.Column15.DataPropertyName = "ID";
            this.Column15.HeaderText = "电流下限";
            this.Column15.Name = "Column15";
            this.Column15.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column15.Width = 78;
            // 
            // Column17
            // 
            this.Column17.DataPropertyName = "PU";
            this.Column17.HeaderText = "功率上限";
            this.Column17.Name = "Column17";
            this.Column17.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column17.Width = 78;
            // 
            // Column16
            // 
            this.Column16.DataPropertyName = "PD";
            this.Column16.HeaderText = "功率下限";
            this.Column16.Name = "Column16";
            this.Column16.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column16.Width = 78;
            // 
            // Plan_JcjdUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Tab_Info);
            this.Name = "Plan_JcjdUserControl";
            this.Size = new System.Drawing.Size(938, 527);
            this.Tab_Info.ResumeLayout(false);
            this.Pag_Result.ResumeLayout(false);
            this.Pag_Result.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllPoint)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Tab_Info;
        private System.Windows.Forms.TabPage Pag_Result;
        private System.Windows.Forms.DataGridView dgvAllPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.TextBox txtP;
        private System.Windows.Forms.TextBox txtI;
        private System.Windows.Forms.TextBox txtV;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;

    }
}

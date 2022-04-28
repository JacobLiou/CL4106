namespace CL4100.Plan3Phase
{
    partial class Plan_DgnSyUserControl
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
            this.txtTimePLFunction = new System.Windows.Forms.TextBox();
            this.label65 = new System.Windows.Forms.Label();
            this.txtTurnNOFunction = new System.Windows.Forms.TextBox();
            this.label66 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.txtParameterFuntion = new System.Windows.Forms.TextBox();
            this.Tab_Info.SuspendLayout();
            this.Pag_Result.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tab_Info
            // 
            this.Tab_Info.Controls.Add(this.Pag_Result);
            this.Tab_Info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tab_Info.Location = new System.Drawing.Point(0, 0);
            this.Tab_Info.Name = "Tab_Info";
            this.Tab_Info.SelectedIndex = 0;
            this.Tab_Info.Size = new System.Drawing.Size(585, 357);
            this.Tab_Info.TabIndex = 6;
            // 
            // Pag_Result
            // 
            this.Pag_Result.Controls.Add(this.txtTimePLFunction);
            this.Pag_Result.Controls.Add(this.label65);
            this.Pag_Result.Controls.Add(this.txtTurnNOFunction);
            this.Pag_Result.Controls.Add(this.label66);
            this.Pag_Result.Controls.Add(this.label67);
            this.Pag_Result.Controls.Add(this.txtParameterFuntion);
            this.Pag_Result.Location = new System.Drawing.Point(4, 22);
            this.Pag_Result.Name = "Pag_Result";
            this.Pag_Result.Padding = new System.Windows.Forms.Padding(3);
            this.Pag_Result.Size = new System.Drawing.Size(577, 331);
            this.Pag_Result.TabIndex = 0;
            this.Pag_Result.Text = "日计时误差检定";
            this.Pag_Result.UseVisualStyleBackColor = true;
            // 
            // txtTimePLFunction
            // 
            this.txtTimePLFunction.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTimePLFunction.Location = new System.Drawing.Point(150, 92);
            this.txtTimePLFunction.Name = "txtTimePLFunction";
            this.txtTimePLFunction.ReadOnly = true;
            this.txtTimePLFunction.Size = new System.Drawing.Size(193, 21);
            this.txtTimePLFunction.TabIndex = 83;
            this.txtTimePLFunction.Text = "1";
            // 
            // label65
            // 
            this.label65.AutoSize = true;
            this.label65.Location = new System.Drawing.Point(35, 92);
            this.label65.Name = "label65";
            this.label65.Size = new System.Drawing.Size(53, 12);
            this.label65.TabIndex = 82;
            this.label65.Text = "时钟频率";
            // 
            // txtTurnNOFunction
            // 
            this.txtTurnNOFunction.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTurnNOFunction.Location = new System.Drawing.Point(150, 129);
            this.txtTurnNOFunction.Name = "txtTurnNOFunction";
            this.txtTurnNOFunction.ReadOnly = true;
            this.txtTurnNOFunction.Size = new System.Drawing.Size(193, 21);
            this.txtTurnNOFunction.TabIndex = 81;
            this.txtTurnNOFunction.Text = "10";
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(35, 131);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(29, 12);
            this.label66.TabIndex = 80;
            this.label66.Text = "圈数";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(35, 52);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(101, 12);
            this.label67.TabIndex = 79;
            this.label67.Text = "标准时钟脉冲常数";
            // 
            // txtParameterFuntion
            // 
            this.txtParameterFuntion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtParameterFuntion.Location = new System.Drawing.Point(150, 48);
            this.txtParameterFuntion.Name = "txtParameterFuntion";
            this.txtParameterFuntion.ReadOnly = true;
            this.txtParameterFuntion.Size = new System.Drawing.Size(193, 21);
            this.txtParameterFuntion.TabIndex = 78;
            this.txtParameterFuntion.Text = "5000000";
            // 
            // Plan_DgnSyUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Tab_Info);
            this.Name = "Plan_DgnSyUserControl";
            this.Size = new System.Drawing.Size(585, 357);
            this.Tab_Info.ResumeLayout(false);
            this.Pag_Result.ResumeLayout(false);
            this.Pag_Result.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Tab_Info;
        private System.Windows.Forms.TabPage Pag_Result;
        private System.Windows.Forms.TextBox txtTimePLFunction;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.TextBox txtTurnNOFunction;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.TextBox txtParameterFuntion;

    }
}

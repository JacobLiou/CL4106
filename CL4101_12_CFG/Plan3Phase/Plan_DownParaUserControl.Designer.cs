namespace CL4100.Plan3Phase
{
    partial class Plan_DownParaUserControl
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
            this.txtNameParamDown = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.cmbProtcolParam = new System.Windows.Forms.TextBox();
            this.txtAddressParamSchema = new System.Windows.Forms.TextBox();
            this.btnChoose = new System.Windows.Forms.Button();
            this.txtAddressParam = new System.Windows.Forms.TextBox();
            this.txtProjectCount = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
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
            this.Pag_Result.Controls.Add(this.txtNameParamDown);
            this.Pag_Result.Controls.Add(this.label53);
            this.Pag_Result.Controls.Add(this.cmbProtcolParam);
            this.Pag_Result.Controls.Add(this.txtAddressParamSchema);
            this.Pag_Result.Controls.Add(this.btnChoose);
            this.Pag_Result.Controls.Add(this.txtAddressParam);
            this.Pag_Result.Controls.Add(this.txtProjectCount);
            this.Pag_Result.Controls.Add(this.label36);
            this.Pag_Result.Controls.Add(this.label32);
            this.Pag_Result.Controls.Add(this.label33);
            this.Pag_Result.Location = new System.Drawing.Point(4, 21);
            this.Pag_Result.Name = "Pag_Result";
            this.Pag_Result.Padding = new System.Windows.Forms.Padding(3);
            this.Pag_Result.Size = new System.Drawing.Size(577, 332);
            this.Pag_Result.TabIndex = 0;
            this.Pag_Result.Text = "打包参数下载";
            this.Pag_Result.UseVisualStyleBackColor = true;
            // 
            // txtNameParamDown
            // 
            this.txtNameParamDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNameParamDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNameParamDown.Location = new System.Drawing.Point(111, 67);
            this.txtNameParamDown.Name = "txtNameParamDown";
            this.txtNameParamDown.ReadOnly = true;
            this.txtNameParamDown.Size = new System.Drawing.Size(433, 21);
            this.txtNameParamDown.TabIndex = 44;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(31, 70);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(41, 12);
            this.label53.TabIndex = 43;
            this.label53.Text = "①名称";
            // 
            // cmbProtcolParam
            // 
            this.cmbProtcolParam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cmbProtcolParam.Location = new System.Drawing.Point(111, 104);
            this.cmbProtcolParam.Name = "cmbProtcolParam";
            this.cmbProtcolParam.ReadOnly = true;
            this.cmbProtcolParam.Size = new System.Drawing.Size(194, 21);
            this.cmbProtcolParam.TabIndex = 42;
            // 
            // txtAddressParamSchema
            // 
            this.txtAddressParamSchema.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAddressParamSchema.Location = new System.Drawing.Point(111, 144);
            this.txtAddressParamSchema.Name = "txtAddressParamSchema";
            this.txtAddressParamSchema.ReadOnly = true;
            this.txtAddressParamSchema.Size = new System.Drawing.Size(194, 21);
            this.txtAddressParamSchema.TabIndex = 40;
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(111, 29);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(194, 23);
            this.btnChoose.TabIndex = 38;
            this.btnChoose.Text = "选择文件";
            this.btnChoose.UseVisualStyleBackColor = true;
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // txtAddressParam
            // 
            this.txtAddressParam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAddressParam.Location = new System.Drawing.Point(330, 31);
            this.txtAddressParam.Name = "txtAddressParam";
            this.txtAddressParam.Size = new System.Drawing.Size(214, 21);
            this.txtAddressParam.TabIndex = 37;
            this.txtAddressParam.Text = "filePath";
            this.txtAddressParam.Visible = false;
            // 
            // txtProjectCount
            // 
            this.txtProjectCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProjectCount.Location = new System.Drawing.Point(111, 184);
            this.txtProjectCount.Name = "txtProjectCount";
            this.txtProjectCount.ReadOnly = true;
            this.txtProjectCount.Size = new System.Drawing.Size(194, 21);
            this.txtProjectCount.TabIndex = 36;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(31, 189);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(65, 12);
            this.label36.TabIndex = 35;
            this.label36.Text = "④项目总数";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(31, 148);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(65, 12);
            this.label32.TabIndex = 34;
            this.label32.Text = "③方案地址";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(32, 108);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(41, 12);
            this.label33.TabIndex = 33;
            this.label33.Text = "②协议";
            // 
            // Plan_DownParaUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Tab_Info);
            this.Name = "Plan_DownParaUserControl";
            this.Size = new System.Drawing.Size(585, 357);
            this.Tab_Info.ResumeLayout(false);
            this.Pag_Result.ResumeLayout(false);
            this.Pag_Result.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Tab_Info;
        private System.Windows.Forms.TabPage Pag_Result;
        private System.Windows.Forms.TextBox txtNameParamDown;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.TextBox cmbProtcolParam;
        private System.Windows.Forms.TextBox txtAddressParamSchema;
        private System.Windows.Forms.Button btnChoose;
        private System.Windows.Forms.TextBox txtAddressParam;
        private System.Windows.Forms.TextBox txtProjectCount;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;

    }
}

namespace CL4100.Plan3Phase
{
    partial class Plan_ReadPowerUserControl
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
            this.txt_MaxP2 = new System.Windows.Forms.TextBox();
            this.txt_MinP2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_MaxP = new System.Windows.Forms.TextBox();
            this.txt_MinP = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
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
            this.Pag_Result.Controls.Add(this.txt_MaxP2);
            this.Pag_Result.Controls.Add(this.txt_MinP2);
            this.Pag_Result.Controls.Add(this.label1);
            this.Pag_Result.Controls.Add(this.label2);
            this.Pag_Result.Controls.Add(this.txt_MaxP);
            this.Pag_Result.Controls.Add(this.txt_MinP);
            this.Pag_Result.Controls.Add(this.label30);
            this.Pag_Result.Controls.Add(this.label31);
            this.Pag_Result.Location = new System.Drawing.Point(4, 21);
            this.Pag_Result.Name = "Pag_Result";
            this.Pag_Result.Padding = new System.Windows.Forms.Padding(3);
            this.Pag_Result.Size = new System.Drawing.Size(577, 332);
            this.Pag_Result.TabIndex = 0;
            this.Pag_Result.Text = "读整机功耗";
            this.Pag_Result.UseVisualStyleBackColor = true;
            // 
            // txt_MaxP2
            // 
            this.txt_MaxP2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_MaxP2.Location = new System.Drawing.Point(232, 157);
            this.txt_MaxP2.Name = "txt_MaxP2";
            this.txt_MaxP2.Size = new System.Drawing.Size(164, 21);
            this.txt_MaxP2.TabIndex = 57;
            this.txt_MaxP2.Text = "1";
            // 
            // txt_MinP2
            // 
            this.txt_MinP2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_MinP2.Location = new System.Drawing.Point(232, 119);
            this.txt_MinP2.Name = "txt_MinP2";
            this.txt_MinP2.Size = new System.Drawing.Size(164, 21);
            this.txt_MinP2.TabIndex = 56;
            this.txt_MinP2.Text = "0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 12);
            this.label1.TabIndex = 54;
            this.label1.Text = "③电流回路功耗下限值(W)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 12);
            this.label2.TabIndex = 55;
            this.label2.Text = "④电流回路功耗上限值(W)";
            // 
            // txt_MaxP
            // 
            this.txt_MaxP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_MaxP.Location = new System.Drawing.Point(232, 79);
            this.txt_MaxP.Name = "txt_MaxP";
            this.txt_MaxP.Size = new System.Drawing.Size(164, 21);
            this.txt_MaxP.TabIndex = 53;
            this.txt_MaxP.Text = "2";
            // 
            // txt_MinP
            // 
            this.txt_MinP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_MinP.Location = new System.Drawing.Point(232, 41);
            this.txt_MinP.Name = "txt_MinP";
            this.txt_MinP.Size = new System.Drawing.Size(164, 21);
            this.txt_MinP.TabIndex = 52;
            this.txt_MinP.Text = "0.2";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(58, 43);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(143, 12);
            this.label30.TabIndex = 50;
            this.label30.Text = "①电压回路功耗下限值(W)";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(58, 81);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(143, 12);
            this.label31.TabIndex = 51;
            this.label31.Text = "②电压回路功耗上限值(W)";
            // 
            // Plan_ReadPowerUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Tab_Info);
            this.Name = "Plan_ReadPowerUserControl";
            this.Size = new System.Drawing.Size(585, 357);
            this.Tab_Info.ResumeLayout(false);
            this.Pag_Result.ResumeLayout(false);
            this.Pag_Result.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Tab_Info;
        private System.Windows.Forms.TabPage Pag_Result;
        private System.Windows.Forms.TextBox txt_MaxP;
        private System.Windows.Forms.TextBox txt_MinP;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox txt_MaxP2;
        private System.Windows.Forms.TextBox txt_MinP2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

    }
}

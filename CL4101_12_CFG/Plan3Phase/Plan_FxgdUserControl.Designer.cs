namespace CL4100.Plan3Phase
{
    partial class Plan_FxgdUserControl
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
            this.txtV = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtA = new System.Windows.Forms.TextBox();
            this.txtC = new System.Windows.Forms.TextBox();
            this.label100 = new System.Windows.Forms.Label();
            this.txtB = new System.Windows.Forms.TextBox();
            this.label103 = new System.Windows.Forms.Label();
            this.label104 = new System.Windows.Forms.Label();
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
            this.Pag_Result.Controls.Add(this.txtV);
            this.Pag_Result.Controls.Add(this.label1);
            this.Pag_Result.Controls.Add(this.txtA);
            this.Pag_Result.Controls.Add(this.txtC);
            this.Pag_Result.Controls.Add(this.label100);
            this.Pag_Result.Controls.Add(this.txtB);
            this.Pag_Result.Controls.Add(this.label103);
            this.Pag_Result.Controls.Add(this.label104);
            this.Pag_Result.Location = new System.Drawing.Point(4, 22);
            this.Pag_Result.Name = "Pag_Result";
            this.Pag_Result.Padding = new System.Windows.Forms.Padding(3);
            this.Pag_Result.Size = new System.Drawing.Size(577, 331);
            this.Pag_Result.TabIndex = 0;
            this.Pag_Result.Text = "分相供电测试";
            this.Pag_Result.UseVisualStyleBackColor = true;
            // 
            // txtV
            // 
            this.txtV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtV.Location = new System.Drawing.Point(157, 47);
            this.txtV.Name = "txtV";
            this.txtV.Size = new System.Drawing.Size(193, 21);
            this.txtV.TabIndex = 54;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 53;
            this.label1.Text = "他相上限值";
            // 
            // txtA
            // 
            this.txtA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtA.Location = new System.Drawing.Point(167, 160);
            this.txtA.Name = "txtA";
            this.txtA.Size = new System.Drawing.Size(193, 21);
            this.txtA.TabIndex = 52;
            this.txtA.Visible = false;
            // 
            // txtC
            // 
            this.txtC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtC.Location = new System.Drawing.Point(167, 241);
            this.txtC.Name = "txtC";
            this.txtC.Size = new System.Drawing.Size(193, 21);
            this.txtC.TabIndex = 51;
            this.txtC.Visible = false;
            // 
            // label100
            // 
            this.label100.AutoSize = true;
            this.label100.Location = new System.Drawing.Point(70, 244);
            this.label100.Name = "label100";
            this.label100.Size = new System.Drawing.Size(35, 12);
            this.label100.TabIndex = 50;
            this.label100.Text = "③C相";
            this.label100.Visible = false;
            // 
            // txtB
            // 
            this.txtB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtB.Location = new System.Drawing.Point(167, 200);
            this.txtB.Name = "txtB";
            this.txtB.Size = new System.Drawing.Size(193, 21);
            this.txtB.TabIndex = 49;
            this.txtB.Visible = false;
            // 
            // label103
            // 
            this.label103.AutoSize = true;
            this.label103.Location = new System.Drawing.Point(70, 162);
            this.label103.Name = "label103";
            this.label103.Size = new System.Drawing.Size(35, 12);
            this.label103.TabIndex = 47;
            this.label103.Text = "①A相";
            this.label103.Visible = false;
            // 
            // label104
            // 
            this.label104.AutoSize = true;
            this.label104.Location = new System.Drawing.Point(70, 202);
            this.label104.Name = "label104";
            this.label104.Size = new System.Drawing.Size(35, 12);
            this.label104.TabIndex = 48;
            this.label104.Text = "②B相";
            this.label104.Visible = false;
            // 
            // Plan_FxgdUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Tab_Info);
            this.Name = "Plan_FxgdUserControl";
            this.Size = new System.Drawing.Size(585, 357);
            this.Tab_Info.ResumeLayout(false);
            this.Pag_Result.ResumeLayout(false);
            this.Pag_Result.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Tab_Info;
        private System.Windows.Forms.TabPage Pag_Result;
        private System.Windows.Forms.TextBox txtA;
        private System.Windows.Forms.TextBox txtC;
        private System.Windows.Forms.Label label100;
        private System.Windows.Forms.TextBox txtB;
        private System.Windows.Forms.Label label103;
        private System.Windows.Forms.Label label104;
        private System.Windows.Forms.TextBox txtV;
        private System.Windows.Forms.Label label1;

    }
}

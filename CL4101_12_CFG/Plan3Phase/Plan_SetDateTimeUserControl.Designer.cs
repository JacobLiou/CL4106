namespace CL4100.Plan3Phase
{
    partial class Plan_SetDateTimeUserControl
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
            this.Pag_Result.Location = new System.Drawing.Point(4, 21);
            this.Pag_Result.Name = "Pag_Result";
            this.Pag_Result.Padding = new System.Windows.Forms.Padding(3);
            this.Pag_Result.Size = new System.Drawing.Size(577, 332);
            this.Pag_Result.TabIndex = 0;
            this.Pag_Result.Text = "设置时钟";
            this.Pag_Result.UseVisualStyleBackColor = true;
            // 
            // txt_Len
            // 
            this.txt_Len.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Len.Location = new System.Drawing.Point(155, 116);
            this.txt_Len.Name = "txt_Len";
            this.txt_Len.Size = new System.Drawing.Size(193, 21);
            this.txt_Len.TabIndex = 44;
            this.txt_Len.Text = "12";
            // 
            // txt_Data
            // 
            this.txt_Data.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Data.Location = new System.Drawing.Point(155, 197);
            this.txt_Data.Name = "txt_Data";
            this.txt_Data.Size = new System.Drawing.Size(193, 21);
            this.txt_Data.TabIndex = 46;
            // 
            // txt_Dot
            // 
            this.txt_Dot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Dot.Location = new System.Drawing.Point(155, 157);
            this.txt_Dot.Name = "txt_Dot";
            this.txt_Dot.Size = new System.Drawing.Size(193, 21);
            this.txt_Dot.TabIndex = 45;
            this.txt_Dot.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(58, 200);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 43;
            this.label5.Text = "⑤下发参数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 41;
            this.label3.Text = "③数据长度";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 42;
            this.label4.Text = "④小数点";
            // 
            // txt_Code
            // 
            this.txt_Code.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Code.Location = new System.Drawing.Point(155, 76);
            this.txt_Code.Name = "txt_Code";
            this.txt_Code.Size = new System.Drawing.Size(193, 21);
            this.txt_Code.TabIndex = 40;
            this.txt_Code.Text = "FFF9";
            // 
            // cmb_Xieyi
            // 
            this.cmb_Xieyi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Xieyi.FormattingEnabled = true;
            this.cmb_Xieyi.Items.AddRange(new object[] {
            "DLT645_1997",
            "DLT645_2007"});
            this.cmb_Xieyi.Location = new System.Drawing.Point(155, 35);
            this.cmb_Xieyi.Name = "cmb_Xieyi";
            this.cmb_Xieyi.Size = new System.Drawing.Size(193, 20);
            this.cmb_Xieyi.TabIndex = 39;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 37;
            this.label1.Text = "①协议";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 38;
            this.label2.Text = "②标识编码";
            // 
            // Plan_SetDateTimeUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Tab_Info);
            this.Name = "Plan_SetDateTimeUserControl";
            this.Size = new System.Drawing.Size(585, 357);
            this.Tab_Info.ResumeLayout(false);
            this.Pag_Result.ResumeLayout(false);
            this.Pag_Result.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl Tab_Info;
        private System.Windows.Forms.TabPage Pag_Result;
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

    }
}

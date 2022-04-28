namespace CL4100
{
    partial class MOShemaForm
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
            this.txtMOName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.but_Confirm = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtMOName
            // 
            this.txtMOName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMOName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtMOName.Location = new System.Drawing.Point(80, 40);
            this.txtMOName.Multiline = true;
            this.txtMOName.Name = "txtMOName";
            this.txtMOName.Size = new System.Drawing.Size(223, 26);
            this.txtMOName.TabIndex = 0;
            this.txtMOName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMOName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "工单号:";
            // 
            // but_Confirm
            // 
            this.but_Confirm.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.but_Confirm.Location = new System.Drawing.Point(309, 40);
            this.but_Confirm.Name = "but_Confirm";
            this.but_Confirm.Size = new System.Drawing.Size(64, 26);
            this.but_Confirm.TabIndex = 12;
            this.but_Confirm.Text = "确 定";
            this.but_Confirm.UseVisualStyleBackColor = true;
            this.but_Confirm.Click += new System.EventHandler(this.but_Confirm_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(298, 93);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "测试获取服务器时间";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MOShemaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 151);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.but_Confirm);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMOName);
            this.Name = "MOShemaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "请输入工单号";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMOName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button but_Confirm;
        private System.Windows.Forms.Button button1;
    }
}
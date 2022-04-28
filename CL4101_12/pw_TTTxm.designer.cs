namespace CL4100
{
    partial class pw_TTTxm
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
            this.SuspendLayout();
            // 
            // txtMOName
            // 
            this.txtMOName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMOName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtMOName.Location = new System.Drawing.Point(106, 45);
            this.txtMOName.Multiline = true;
            this.txtMOName.Name = "txtMOName";
            this.txtMOName.Size = new System.Drawing.Size(293, 26);
            this.txtMOName.TabIndex = 0;
            this.txtMOName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMOName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "台体条码：";
            // 
            // pw_TTTxm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 133);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMOName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "pw_TTTxm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "请扫描台体条形码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMOName;
        private System.Windows.Forms.Label label1;
    }
}
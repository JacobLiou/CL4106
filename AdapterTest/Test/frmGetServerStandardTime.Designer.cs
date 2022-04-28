namespace ClAmMeterController.Test
{
    partial class frmGetServerStandardTime
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbxlocalip = new System.Windows.Forms.TextBox();
            this.tbxlocalPort = new System.Windows.Forms.TextBox();
            this.tbxSendtoport = new System.Windows.Forms.TextBox();
            this.tbxSendtoIp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkbxAnonymous = new System.Windows.Forms.CheckBox();
            this.tbxMessageSend = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.lstbxMessageView = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "本地IP";
            // 
            // tbxlocalip
            // 
            this.tbxlocalip.Location = new System.Drawing.Point(101, 35);
            this.tbxlocalip.Name = "tbxlocalip";
            this.tbxlocalip.Size = new System.Drawing.Size(100, 21);
            this.tbxlocalip.TabIndex = 1;
            // 
            // tbxlocalPort
            // 
            this.tbxlocalPort.Location = new System.Drawing.Point(226, 35);
            this.tbxlocalPort.Name = "tbxlocalPort";
            this.tbxlocalPort.Size = new System.Drawing.Size(64, 21);
            this.tbxlocalPort.TabIndex = 2;
            // 
            // tbxSendtoport
            // 
            this.tbxSendtoport.Location = new System.Drawing.Point(226, 83);
            this.tbxSendtoport.Name = "tbxSendtoport";
            this.tbxSendtoport.Size = new System.Drawing.Size(64, 21);
            this.tbxSendtoport.TabIndex = 5;
            // 
            // tbxSendtoIp
            // 
            this.tbxSendtoIp.Location = new System.Drawing.Point(101, 83);
            this.tbxSendtoIp.Name = "tbxSendtoIp";
            this.tbxSendtoIp.Size = new System.Drawing.Size(100, 21);
            this.tbxSendtoIp.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "发送到";
            // 
            // chkbxAnonymous
            // 
            this.chkbxAnonymous.AutoSize = true;
            this.chkbxAnonymous.Location = new System.Drawing.Point(39, 149);
            this.chkbxAnonymous.Name = "chkbxAnonymous";
            this.chkbxAnonymous.Size = new System.Drawing.Size(72, 16);
            this.chkbxAnonymous.TabIndex = 6;
            this.chkbxAnonymous.Text = "匿名发送";
            this.chkbxAnonymous.UseVisualStyleBackColor = true;
            // 
            // tbxMessageSend
            // 
            this.tbxMessageSend.Location = new System.Drawing.Point(132, 143);
            this.tbxMessageSend.Name = "tbxMessageSend";
            this.tbxMessageSend.Size = new System.Drawing.Size(194, 21);
            this.tbxMessageSend.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(338, 143);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "发送";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(39, 211);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "清空";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(126, 211);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "停止";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(338, 211);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "接收";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnReceive_Click);
            // 
            // lstbxMessageView
            // 
            this.lstbxMessageView.FormattingEnabled = true;
            this.lstbxMessageView.ItemHeight = 12;
            this.lstbxMessageView.Location = new System.Drawing.Point(39, 265);
            this.lstbxMessageView.Name = "lstbxMessageView";
            this.lstbxMessageView.Size = new System.Drawing.Size(374, 88);
            this.lstbxMessageView.TabIndex = 12;
            // 
            // frmGetServerStandardTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 367);
            this.Controls.Add(this.lstbxMessageView);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbxMessageSend);
            this.Controls.Add(this.chkbxAnonymous);
            this.Controls.Add(this.tbxSendtoport);
            this.Controls.Add(this.tbxSendtoIp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbxlocalPort);
            this.Controls.Add(this.tbxlocalip);
            this.Controls.Add(this.label1);
            this.Name = "frmGetServerStandardTime";
            this.Text = "获取GPS服务器时间";
            this.Load += new System.EventHandler(this.frmGetServerStandardTime_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxlocalip;
        private System.Windows.Forms.TextBox tbxlocalPort;
        private System.Windows.Forms.TextBox tbxSendtoport;
        private System.Windows.Forms.TextBox tbxSendtoIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkbxAnonymous;
        private System.Windows.Forms.TextBox tbxMessageSend;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListBox lstbxMessageView;

    }
}
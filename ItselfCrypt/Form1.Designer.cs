namespace ItselfCrypt
{
    partial class Form1
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
            this.BuildLog = new System.Windows.Forms.RichTextBox();
            this.FilePathTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.IcoTextbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BuildLog
            // 
            this.BuildLog.Location = new System.Drawing.Point(-1, 105);
            this.BuildLog.Name = "BuildLog";
            this.BuildLog.ReadOnly = true;
            this.BuildLog.Size = new System.Drawing.Size(339, 143);
            this.BuildLog.TabIndex = 0;
            this.BuildLog.Text = "Made by BahNahNah\nhttp://www.hackforums.net/member.php?action=profile&uid=2388291" +
    "\n";
            // 
            // FilePathTextbox
            // 
            this.FilePathTextbox.Location = new System.Drawing.Point(50, 12);
            this.FilePathTextbox.Name = "FilePathTextbox";
            this.FilePathTextbox.Size = new System.Drawing.Size(250, 20);
            this.FilePathTextbox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-4, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Payload:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(306, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(32, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(-1, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(339, 35);
            this.button2.TabIndex = 4;
            this.button2.Text = "Crypt";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-4, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Icon:";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(306, 38);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(32, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // IcoTextbox
            // 
            this.IcoTextbox.Location = new System.Drawing.Point(50, 38);
            this.IcoTextbox.Name = "IcoTextbox";
            this.IcoTextbox.Size = new System.Drawing.Size(250, 20);
            this.IcoTextbox.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 251);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.IcoTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FilePathTextbox);
            this.Controls.Add(this.BuildLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "ItselfCrypter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox BuildLog;
        private System.Windows.Forms.TextBox FilePathTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox IcoTextbox;
    }
}


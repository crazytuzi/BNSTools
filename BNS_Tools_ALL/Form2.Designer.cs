namespace BNS_Tools_ALL
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SQL_BTN = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PASSWORD_TB = new System.Windows.Forms.TextBox();
            this.ACCTION_TB = new System.Windows.Forms.TextBox();
            this.IP_TB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SQL_BTN);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.PASSWORD_TB);
            this.groupBox1.Controls.Add(this.ACCTION_TB);
            this.groupBox1.Controls.Add(this.IP_TB);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 147);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据库配置";
            // 
            // SQL_BTN
            // 
            this.SQL_BTN.Location = new System.Drawing.Point(89, 104);
            this.SQL_BTN.Name = "SQL_BTN";
            this.SQL_BTN.Size = new System.Drawing.Size(131, 23);
            this.SQL_BTN.TabIndex = 13;
            this.SQL_BTN.Text = "连接并保存";
            this.SQL_BTN.UseVisualStyleBackColor = true;
            this.SQL_BTN.Click += new System.EventHandler(this.SQL_BTN_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "密码:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "账号:";
            // 
            // PASSWORD_TB
            // 
            this.PASSWORD_TB.Location = new System.Drawing.Point(89, 77);
            this.PASSWORD_TB.Name = "PASSWORD_TB";
            this.PASSWORD_TB.Size = new System.Drawing.Size(131, 21);
            this.PASSWORD_TB.TabIndex = 3;
            this.PASSWORD_TB.Text = "Bs123456789";
            // 
            // ACCTION_TB
            // 
            this.ACCTION_TB.Location = new System.Drawing.Point(89, 50);
            this.ACCTION_TB.Name = "ACCTION_TB";
            this.ACCTION_TB.Size = new System.Drawing.Size(131, 21);
            this.ACCTION_TB.TabIndex = 2;
            this.ACCTION_TB.Text = "bs";
            // 
            // IP_TB
            // 
            this.IP_TB.Location = new System.Drawing.Point(89, 23);
            this.IP_TB.Name = "IP_TB";
            this.IP_TB.Size = new System.Drawing.Size(131, 21);
            this.IP_TB.TabIndex = 1;
            this.IP_TB.Text = "127.0.0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库IP:";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 171);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据库配置";
            this.Shown += new System.EventHandler(this.Form2_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button SQL_BTN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PASSWORD_TB;
        private System.Windows.Forms.TextBox ACCTION_TB;
        private System.Windows.Forms.TextBox IP_TB;
        private System.Windows.Forms.Label label1;
    }
}
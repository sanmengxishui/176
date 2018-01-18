namespace A162Client
{
    partial class A162Client_Win
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
            this.button3 = new System.Windows.Forms.Button();
            this.msgLB = new System.Windows.Forms.ListBox();
            this.setstr = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.cmdTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ip1TB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ModLB = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(437, 66);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 25);
            this.button3.TabIndex = 82;
            this.button3.Text = "Disconnect";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // msgLB
            // 
            this.msgLB.FormattingEnabled = true;
            this.msgLB.ItemHeight = 12;
            this.msgLB.Location = new System.Drawing.Point(168, 109);
            this.msgLB.Name = "msgLB";
            this.msgLB.Size = new System.Drawing.Size(481, 112);
            this.msgLB.TabIndex = 81;
            this.msgLB.SelectedIndexChanged += new System.EventHandler(this.msgLB_SelectedIndexChanged);
            // 
            // setstr
            // 
            this.setstr.Location = new System.Drawing.Point(545, 66);
            this.setstr.Name = "setstr";
            this.setstr.Size = new System.Drawing.Size(88, 25);
            this.setstr.TabIndex = 80;
            this.setstr.Text = "Read";
            this.setstr.UseVisualStyleBackColor = true;
            this.setstr.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(437, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 25);
            this.button1.TabIndex = 79;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmdTB
            // 
            this.cmdTB.Location = new System.Drawing.Point(275, 74);
            this.cmdTB.Name = "cmdTB";
            this.cmdTB.Size = new System.Drawing.Size(100, 21);
            this.cmdTB.TabIndex = 78;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(235, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 77;
            this.label3.Text = "Port";
            // 
            // ip1TB
            // 
            this.ip1TB.Location = new System.Drawing.Point(275, 38);
            this.ip1TB.Name = "ip1TB";
            this.ip1TB.Size = new System.Drawing.Size(125, 21);
            this.ip1TB.TabIndex = 76;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(193, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 75;
            this.label2.Text = "IP Address";
            // 
            // ModLB
            // 
            this.ModLB.FormattingEnabled = true;
            this.ModLB.ItemHeight = 12;
            this.ModLB.Location = new System.Drawing.Point(18, 40);
            this.ModLB.Name = "ModLB";
            this.ModLB.Size = new System.Drawing.Size(144, 232);
            this.ModLB.TabIndex = 74;
            this.ModLB.SelectedIndexChanged += new System.EventHandler(this.ModLB_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 73;
            this.label1.Text = "Module List";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(361, 247);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 25);
            this.button2.TabIndex = 83;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // A162Client_Win
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 287);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.msgLB);
            this.Controls.Add(this.setstr);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdTB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ip1TB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ModLB);
            this.Controls.Add(this.label1);
            this.Name = "A162Client_Win";
            this.Text = "A162Client_Win";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.A162ClientCom_FormClosing);
            this.Load += new System.EventHandler(this.A162ClientCom_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox msgLB;
        private System.Windows.Forms.Button setstr;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox cmdTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ip1TB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox ModLB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
    }
}


namespace Serial_Port
{
    partial class SP_Win
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
            this.SerialLB = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.Clear = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Set = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RSNUD = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.BtnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ParityCB = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.StopBitCB = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BitCB = new System.Windows.Forms.ComboBox();
            this.PortCB = new System.Windows.Forms.ComboBox();
            this.RateCB = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Light2 = new System.Windows.Forms.TextBox();
            this.Light1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.Light1trackbar = new System.Windows.Forms.TrackBar();
            this.Light2trackbar = new System.Windows.Forms.TrackBar();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RSNUD)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Light1trackbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Light2trackbar)).BeginInit();
            this.SuspendLayout();
            // 
            // SerialLB
            // 
            this.SerialLB.FormattingEnabled = true;
            this.SerialLB.Location = new System.Drawing.Point(16, 28);
            this.SerialLB.Name = "SerialLB";
            this.SerialLB.Size = new System.Drawing.Size(156, 264);
            this.SerialLB.TabIndex = 18;
            this.SerialLB.SelectedIndexChanged += new System.EventHandler(this.SerialLB_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.Clear);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.Set);
            this.groupBox2.Location = new System.Drawing.Point(388, 184);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(361, 190);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "通讯测试：";
            this.groupBox2.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(263, 152);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(216, 159);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "接收：";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(189, 22);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(166, 123);
            this.textBox2.TabIndex = 3;
            // 
            // Clear
            // 
            this.Clear.Location = new System.Drawing.Point(104, 152);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(67, 28);
            this.Clear.TabIndex = 2;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 22);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(174, 123);
            this.textBox1.TabIndex = 1;
            // 
            // Set
            // 
            this.Set.Location = new System.Drawing.Point(19, 152);
            this.Set.Name = "Set";
            this.Set.Size = new System.Drawing.Size(67, 28);
            this.Set.TabIndex = 0;
            this.Set.Text = "Set";
            this.Set.UseVisualStyleBackColor = true;
            this.Set.Click += new System.EventHandler(this.Set_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RSNUD);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.BtnSave);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ParityCB);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.StopBitCB);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.BitCB);
            this.groupBox1.Controls.Add(this.PortCB);
            this.groupBox1.Controls.Add(this.RateCB);
            this.groupBox1.Location = new System.Drawing.Point(198, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(173, 277);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前串口信息";
            // 
            // RSNUD
            // 
            this.RSNUD.Location = new System.Drawing.Point(62, 208);
            this.RSNUD.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.RSNUD.Name = "RSNUD";
            this.RSNUD.Size = new System.Drawing.Size(81, 20);
            this.RSNUD.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(147, 211);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "ms";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 210);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "收发超时";
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(37, 237);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(93, 30);
            this.BtnSave.TabIndex = 10;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "端口号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "波特率";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "数据位";
            // 
            // ParityCB
            // 
            this.ParityCB.FormattingEnabled = true;
            this.ParityCB.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even"});
            this.ParityCB.Location = new System.Drawing.Point(62, 169);
            this.ParityCB.Name = "ParityCB";
            this.ParityCB.Size = new System.Drawing.Size(81, 21);
            this.ParityCB.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "停止位";
            // 
            // StopBitCB
            // 
            this.StopBitCB.FormattingEnabled = true;
            this.StopBitCB.Items.AddRange(new object[] {
            "One",
            "OnePointFive",
            "Two"});
            this.StopBitCB.Location = new System.Drawing.Point(62, 134);
            this.StopBitCB.Name = "StopBitCB";
            this.StopBitCB.Size = new System.Drawing.Size(81, 21);
            this.StopBitCB.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "校验位";
            // 
            // BitCB
            // 
            this.BitCB.FormattingEnabled = true;
            this.BitCB.Items.AddRange(new object[] {
            "6",
            "7",
            "8"});
            this.BitCB.Location = new System.Drawing.Point(62, 100);
            this.BitCB.Name = "BitCB";
            this.BitCB.Size = new System.Drawing.Size(81, 21);
            this.BitCB.TabIndex = 7;
            // 
            // PortCB
            // 
            this.PortCB.FormattingEnabled = true;
            this.PortCB.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
            this.PortCB.Location = new System.Drawing.Point(62, 30);
            this.PortCB.Name = "PortCB";
            this.PortCB.Size = new System.Drawing.Size(81, 21);
            this.PortCB.TabIndex = 5;
            // 
            // RateCB
            // 
            this.RateCB.FormattingEnabled = true;
            this.RateCB.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "115200"});
            this.RateCB.Location = new System.Drawing.Point(62, 65);
            this.RateCB.Name = "RateCB";
            this.RateCB.Size = new System.Drawing.Size(81, 21);
            this.RateCB.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Serial Interface List";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(388, 101);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(105, 69);
            this.button2.TabIndex = 6;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(388, 24);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(195, 40);
            this.textBox3.TabIndex = 6;
            this.textBox3.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Light2trackbar);
            this.groupBox3.Controls.Add(this.Light1trackbar);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.Light2);
            this.groupBox3.Controls.Add(this.Light1);
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Location = new System.Drawing.Point(17, 298);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(330, 188);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "光源控制(最大为255)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 86);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "2光源当前值：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 35);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "1光源当前值：";
            // 
            // Light2
            // 
            this.Light2.Enabled = false;
            this.Light2.Location = new System.Drawing.Point(99, 83);
            this.Light2.Name = "Light2";
            this.Light2.Size = new System.Drawing.Size(36, 20);
            this.Light2.TabIndex = 2;
            // 
            // Light1
            // 
            this.Light1.Enabled = false;
            this.Light1.Location = new System.Drawing.Point(99, 32);
            this.Light1.Name = "Light1";
            this.Light1.Size = new System.Drawing.Size(36, 20);
            this.Light1.TabIndex = 1;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(107, 133);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 38);
            this.button3.TabIndex = 0;
            this.button3.Text = "设置";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Light1trackbar
            // 
            this.Light1trackbar.Location = new System.Drawing.Point(145, 31);
            this.Light1trackbar.Maximum = 255;
            this.Light1trackbar.Name = "Light1trackbar";
            this.Light1trackbar.Size = new System.Drawing.Size(179, 45);
            this.Light1trackbar.TabIndex = 20;
            this.Light1trackbar.Scroll += new System.EventHandler(this.Light1trackbar_Scroll);
            // 
            // Light2trackbar
            // 
            this.Light2trackbar.Location = new System.Drawing.Point(145, 82);
            this.Light2trackbar.Maximum = 255;
            this.Light2trackbar.Name = "Light2trackbar";
            this.Light2trackbar.Size = new System.Drawing.Size(179, 45);
            this.Light2trackbar.TabIndex = 21;
            this.Light2trackbar.Scroll += new System.EventHandler(this.Light2trackbar_Scroll);
            // 
            // SP_Win
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 497);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.SerialLB);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Name = "SP_Win";
            this.Text = "SP_Win";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinSerailComMgr_FormClosing);
            this.Load += new System.EventHandler(this.WinSerailComMgr_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RSNUD)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Light1trackbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Light2trackbar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox SerialLB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button Clear;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Set;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown RSNUD;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ParityCB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox StopBitCB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox BitCB;
        private System.Windows.Forms.ComboBox PortCB;
        private System.Windows.Forms.ComboBox RateCB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox Light2;
        private System.Windows.Forms.TextBox Light1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TrackBar Light1trackbar;
        private System.Windows.Forms.TrackBar Light2trackbar;
    }
}
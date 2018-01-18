namespace Panasonic_Communication
{
    partial class WinPanasonic_PLC
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.PortCB = new System.Windows.Forms.ComboBox();
            this.RateCB = new System.Windows.Forms.ComboBox();
            this.BitCB = new System.Windows.Forms.ComboBox();
            this.StopBitCB = new System.Windows.Forms.ComboBox();
            this.ParityCB = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RSNUD = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.BtnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.SerialLB = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.button10 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RSNUD)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "端口号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "波特率";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "数据位";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "停止位";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 159);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "校验位";
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
            this.PortCB.Location = new System.Drawing.Point(62, 28);
            this.PortCB.Name = "PortCB";
            this.PortCB.Size = new System.Drawing.Size(81, 20);
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
            this.RateCB.Location = new System.Drawing.Point(62, 60);
            this.RateCB.Name = "RateCB";
            this.RateCB.Size = new System.Drawing.Size(81, 20);
            this.RateCB.TabIndex = 6;
            // 
            // BitCB
            // 
            this.BitCB.FormattingEnabled = true;
            this.BitCB.Items.AddRange(new object[] {
            "6",
            "7",
            "8"});
            this.BitCB.Location = new System.Drawing.Point(62, 92);
            this.BitCB.Name = "BitCB";
            this.BitCB.Size = new System.Drawing.Size(81, 20);
            this.BitCB.TabIndex = 7;
            // 
            // StopBitCB
            // 
            this.StopBitCB.FormattingEnabled = true;
            this.StopBitCB.Items.AddRange(new object[] {
            "One",
            "OnePointFive",
            "Two"});
            this.StopBitCB.Location = new System.Drawing.Point(62, 124);
            this.StopBitCB.Name = "StopBitCB";
            this.StopBitCB.Size = new System.Drawing.Size(81, 20);
            this.StopBitCB.TabIndex = 8;
            // 
            // ParityCB
            // 
            this.ParityCB.FormattingEnabled = true;
            this.ParityCB.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even"});
            this.ParityCB.Location = new System.Drawing.Point(62, 156);
            this.ParityCB.Name = "ParityCB";
            this.ParityCB.Size = new System.Drawing.Size(81, 20);
            this.ParityCB.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "Serial Interface List";
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
            this.groupBox1.Location = new System.Drawing.Point(196, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(173, 256);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前串口信息";
            // 
            // RSNUD
            // 
            this.RSNUD.Location = new System.Drawing.Point(62, 192);
            this.RSNUD.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.RSNUD.Name = "RSNUD";
            this.RSNUD.Size = new System.Drawing.Size(81, 21);
            this.RSNUD.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(147, 195);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 15;
            this.label9.Text = "ms";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 194);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 11;
            this.label8.Text = "收发超时";
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(37, 219);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(93, 28);
            this.BtnSave.TabIndex = 10;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.textBox4);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Location = new System.Drawing.Point(8, 280);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(361, 172);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "测试寄存器DT：";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(205, 107);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(129, 21);
            this.textBox3.TabIndex = 18;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(205, 35);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(129, 21);
            this.textBox4.TabIndex = 17;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(25, 107);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(129, 21);
            this.textBox2.TabIndex = 16;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(25, 35);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(129, 21);
            this.textBox1.TabIndex = 15;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(217, 134);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(95, 28);
            this.button5.TabIndex = 14;
            this.button5.Text = "双寄存器读取";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(217, 62);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(95, 28);
            this.button4.TabIndex = 13;
            this.button4.Text = "双寄存器写入";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(42, 134);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(87, 28);
            this.button3.TabIndex = 12;
            this.button3.Text = "单寄存器读取";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(42, 62);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 28);
            this.button2.TabIndex = 11;
            this.button2.Text = "单寄存器写入";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(94, 277);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(76, 21);
            this.textBox5.TabIndex = 19;
            // 
            // SerialLB
            // 
            this.SerialLB.FormattingEnabled = true;
            this.SerialLB.ItemHeight = 12;
            this.SerialLB.Location = new System.Drawing.Point(14, 27);
            this.SerialLB.Name = "SerialLB";
            this.SerialLB.Size = new System.Drawing.Size(156, 244);
            this.SerialLB.TabIndex = 14;
            this.SerialLB.SelectedIndexChanged += new System.EventHandler(this.SerialLB_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox6);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.comboBox2);
            this.groupBox3.Controls.Add(this.comboBox1);
            this.groupBox3.Controls.Add(this.textBox10);
            this.groupBox3.Controls.Add(this.button10);
            this.groupBox3.Location = new System.Drawing.Point(412, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(188, 152);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "测试单点XYR：";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(25, 91);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(49, 21);
            this.textBox6.TabIndex = 23;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 117);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 28);
            this.button1.TabIndex = 22;
            this.button1.Text = "单点读取";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "1",
            "0"});
            this.comboBox2.Location = new System.Drawing.Point(25, 27);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(41, 20);
            this.comboBox2.TabIndex = 21;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "X",
            "Y",
            "R"});
            this.comboBox1.Location = new System.Drawing.Point(82, -3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(41, 20);
            this.comboBox1.TabIndex = 17;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(129, -4);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(49, 21);
            this.textBox10.TabIndex = 20;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(15, 53);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(71, 28);
            this.button10.TabIndex = 11;
            this.button10.Text = "单点写入";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(415, 190);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(71, 28);
            this.button6.TabIndex = 24;
            this.button6.Text = "连续写入";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(415, 224);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(71, 28);
            this.button7.TabIndex = 25;
            this.button7.Text = "连续读取";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // WinPanasonic_PLC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 464);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.SerialLB);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Name = "WinPanasonic_PLC";
            this.Text = "WinPanasonic_PLC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinPanasonic_PLC_FormClosing);
            this.Load += new System.EventHandler(this.WinPanasonic_PLC_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RSNUD)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox PortCB;
        private System.Windows.Forms.ComboBox RateCB;
        private System.Windows.Forms.ComboBox BitCB;
        private System.Windows.Forms.ComboBox StopBitCB;
        private System.Windows.Forms.ComboBox ParityCB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown RSNUD;
        private System.Windows.Forms.ListBox SerialLB;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
    }
}
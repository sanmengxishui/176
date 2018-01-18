namespace TcpSerive
{
    partial class Tcpserive
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
            this.SeriveList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.start = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.setmes = new System.Windows.Forms.TextBox();
            this.set = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.portsetbutton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.speedSavebotton = new System.Windows.Forms.Button();
            this.Accelbox = new System.Windows.Forms.TextBox();
            this.Speedbox = new System.Windows.Forms.TextBox();
            this.AccelBar = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.speedBar = new System.Windows.Forms.TrackBar();
            this.combPower = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AccelBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).BeginInit();
            this.SuspendLayout();
            // 
            // SeriveList
            // 
            this.SeriveList.FormattingEnabled = true;
            this.SeriveList.Location = new System.Drawing.Point(12, 13);
            this.SeriveList.Name = "SeriveList";
            this.SeriveList.Size = new System.Drawing.Size(181, 420);
            this.SeriveList.TabIndex = 0;
            this.SeriveList.SelectedIndexChanged += new System.EventHandler(this.SeriveList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(229, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 20);
            this.textBox1.TabIndex = 2;
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(453, 11);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(98, 44);
            this.start.TabIndex = 3;
            this.start.Text = "start";
            this.start.UseVisualStyleBackColor = true;
            // 
            // stop
            // 
            this.stop.Location = new System.Drawing.Point(453, 61);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(98, 44);
            this.stop.TabIndex = 4;
            this.stop.Text = "stop";
            this.stop.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(229, 205);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(295, 33);
            this.textBox2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "ReceiveMessage";
            // 
            // setmes
            // 
            this.setmes.Location = new System.Drawing.Point(229, 111);
            this.setmes.Multiline = true;
            this.setmes.Name = "setmes";
            this.setmes.Size = new System.Drawing.Size(295, 32);
            this.setmes.TabIndex = 7;
            // 
            // set
            // 
            this.set.Location = new System.Drawing.Point(229, 149);
            this.set.Name = "set";
            this.set.Size = new System.Drawing.Size(97, 36);
            this.set.TabIndex = 8;
            this.set.Text = "set";
            this.set.UseVisualStyleBackColor = true;
            this.set.Click += new System.EventHandler(this.set_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(310, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 44);
            this.button1.TabIndex = 9;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // portsetbutton
            // 
            this.portsetbutton.Location = new System.Drawing.Point(310, 20);
            this.portsetbutton.Name = "portsetbutton";
            this.portsetbutton.Size = new System.Drawing.Size(66, 35);
            this.portsetbutton.TabIndex = 10;
            this.portsetbutton.Text = "确认";
            this.portsetbutton.UseVisualStyleBackColor = true;
            this.portsetbutton.Click += new System.EventHandler(this.portsetbutton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.speedSavebotton);
            this.groupBox1.Controls.Add(this.Accelbox);
            this.groupBox1.Controls.Add(this.Speedbox);
            this.groupBox1.Controls.Add(this.AccelBar);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.speedBar);
            this.groupBox1.Controls.Add(this.combPower);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(229, 244);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 189);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "RobotSpeed";
            // 
            // speedSavebotton
            // 
            this.speedSavebotton.Location = new System.Drawing.Point(109, 149);
            this.speedSavebotton.Name = "speedSavebotton";
            this.speedSavebotton.Size = new System.Drawing.Size(87, 34);
            this.speedSavebotton.TabIndex = 12;
            this.speedSavebotton.Text = "Save";
            this.speedSavebotton.UseVisualStyleBackColor = true;
            this.speedSavebotton.Click += new System.EventHandler(this.speedSavebotton_Click);
            // 
            // Accelbox
            // 
            this.Accelbox.Enabled = false;
            this.Accelbox.Location = new System.Drawing.Point(247, 103);
            this.Accelbox.Name = "Accelbox";
            this.Accelbox.Size = new System.Drawing.Size(36, 20);
            this.Accelbox.TabIndex = 7;
            // 
            // Speedbox
            // 
            this.Speedbox.Enabled = false;
            this.Speedbox.Location = new System.Drawing.Point(247, 52);
            this.Speedbox.Name = "Speedbox";
            this.Speedbox.Size = new System.Drawing.Size(36, 20);
            this.Speedbox.TabIndex = 6;
            // 
            // AccelBar
            // 
            this.AccelBar.Location = new System.Drawing.Point(51, 103);
            this.AccelBar.Maximum = 120;
            this.AccelBar.Name = "AccelBar";
            this.AccelBar.Size = new System.Drawing.Size(181, 45);
            this.AccelBar.TabIndex = 5;
            this.AccelBar.Value = 1;
            this.AccelBar.Scroll += new System.EventHandler(this.AccelBar_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Accel:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Speed:";
            // 
            // speedBar
            // 
            this.speedBar.Location = new System.Drawing.Point(51, 52);
            this.speedBar.Maximum = 100;
            this.speedBar.Name = "speedBar";
            this.speedBar.Size = new System.Drawing.Size(181, 45);
            this.speedBar.TabIndex = 2;
            this.speedBar.Value = 1;
            this.speedBar.Scroll += new System.EventHandler(this.speedBar_Scroll);
            // 
            // combPower
            // 
            this.combPower.FormattingEnabled = true;
            this.combPower.Items.AddRange(new object[] {
            "Low",
            "High"});
            this.combPower.Location = new System.Drawing.Point(62, 25);
            this.combPower.Name = "combPower";
            this.combPower.Size = new System.Drawing.Size(53, 21);
            this.combPower.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Power:";
            // 
            // Tcpserive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 446);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.portsetbutton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.set);
            this.Controls.Add(this.setmes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.start);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SeriveList);
            this.Name = "Tcpserive";
            this.Text = "TcpseriveWin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Tcpserive_FormClosing);
            this.Load += new System.EventHandler(this.Tcpserive_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AccelBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox SeriveList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox setmes;
        private System.Windows.Forms.Button set;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button portsetbutton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox combPower;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar AccelBar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar speedBar;
        private System.Windows.Forms.TextBox Accelbox;
        private System.Windows.Forms.TextBox Speedbox;
        private System.Windows.Forms.Button speedSavebotton;
    }
}


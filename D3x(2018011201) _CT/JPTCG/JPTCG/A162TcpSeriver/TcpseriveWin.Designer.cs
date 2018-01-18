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
            this.SuspendLayout();
            // 
            // SeriveList
            // 
            this.SeriveList.FormattingEnabled = true;
            this.SeriveList.ItemHeight = 12;
            this.SeriveList.Location = new System.Drawing.Point(12, 12);
            this.SeriveList.Name = "SeriveList";
            this.SeriveList.Size = new System.Drawing.Size(181, 388);
            this.SeriveList.TabIndex = 0;
            this.SeriveList.SelectedIndexChanged += new System.EventHandler(this.SeriveList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(229, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 21);
            this.textBox1.TabIndex = 2;
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(440, 39);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(98, 41);
            this.start.TabIndex = 3;
            this.start.Text = "start";
            this.start.UseVisualStyleBackColor = true;
            // 
            // stop
            // 
            this.stop.Location = new System.Drawing.Point(440, 103);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(98, 41);
            this.stop.TabIndex = 4;
            this.stop.Text = "stop";
            this.stop.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(229, 272);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(295, 128);
            this.textBox2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 257);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "ReceiveMessage";
            // 
            // setmes
            // 
            this.setmes.Location = new System.Drawing.Point(229, 165);
            this.setmes.Multiline = true;
            this.setmes.Name = "setmes";
            this.setmes.Size = new System.Drawing.Size(295, 29);
            this.setmes.TabIndex = 7;
            // 
            // set
            // 
            this.set.Location = new System.Drawing.Point(229, 200);
            this.set.Name = "set";
            this.set.Size = new System.Drawing.Size(97, 33);
            this.set.TabIndex = 8;
            this.set.Text = "set";
            this.set.UseVisualStyleBackColor = true;
            this.set.Click += new System.EventHandler(this.set_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(291, 103);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 41);
            this.button1.TabIndex = 9;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // portsetbutton
            // 
            this.portsetbutton.Location = new System.Drawing.Point(310, 18);
            this.portsetbutton.Name = "portsetbutton";
            this.portsetbutton.Size = new System.Drawing.Size(66, 32);
            this.portsetbutton.TabIndex = 10;
            this.portsetbutton.Text = "确认";
            this.portsetbutton.UseVisualStyleBackColor = true;
            this.portsetbutton.Click += new System.EventHandler(this.portsetbutton_Click);
            // 
            // Tcpserive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 412);
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
    }
}


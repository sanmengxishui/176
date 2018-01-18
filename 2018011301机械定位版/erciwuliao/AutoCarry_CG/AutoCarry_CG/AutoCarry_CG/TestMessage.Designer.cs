namespace AutoCarry_CG
{
    partial class TestMessage
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
            this.OKbutton = new System.Windows.Forms.Button();
            this.ONbutton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(79, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // OKbutton
            // 
            this.OKbutton.Location = new System.Drawing.Point(115, 99);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(86, 38);
            this.OKbutton.TabIndex = 1;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // ONbutton
            // 
            this.ONbutton.Location = new System.Drawing.Point(312, 99);
            this.ONbutton.Name = "ONbutton";
            this.ONbutton.Size = new System.Drawing.Size(86, 38);
            this.ONbutton.TabIndex = 2;
            this.ONbutton.Text = "ON";
            this.ONbutton.UseVisualStyleBackColor = true;
            this.ONbutton.Click += new System.EventHandler(this.ONbutton_Click);
            // 
            // TestMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 150);
            this.Controls.Add(this.ONbutton);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TestMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestMessage";
            this.Activated += new System.EventHandler(this.TestMessage_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestMessage_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Button ONbutton;
    }
}
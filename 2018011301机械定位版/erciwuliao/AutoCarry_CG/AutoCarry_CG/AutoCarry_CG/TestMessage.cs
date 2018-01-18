using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JPTCG.Common;

namespace AutoCarry_CG
{
    public partial class TestMessage : Form
    {
        public TestMessage()
        {
            InitializeComponent();
        }

        private void TestMessage_FormClosing(object sender, FormClosingEventArgs e)
        {

            Para.RobotTestMessage = "";
            Para.RobotTestNG = true;
            Para.RobotTestOK = false;

        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            Para.RobotTestMessage = "";
            Para.RobotTestNG = false;
            Para.RobotTestOK = true;
            this.Hide();
        }

        private void ONbutton_Click(object sender, EventArgs e)
        {
            Para.RobotTestMessage = "";
            Para.RobotTestNG = true;
            Para.RobotTestOK = false;
            this.Hide();
        }

        private void TestMessage_Activated(object sender, EventArgs e)
        {
            label1.Text = Para.RobotTestMessage;
        }



    }
}

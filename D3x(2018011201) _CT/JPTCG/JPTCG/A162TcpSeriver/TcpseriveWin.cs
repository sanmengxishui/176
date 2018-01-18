using JPTCG.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpSerive
{
    public partial class Tcpserive : Form
    {
        TcpseriveComMgr TcpMgr;
        public Tcpserive(TcpseriveComMgr myMgr)
        {
            InitializeComponent();
            TcpMgr = myMgr;
        }

        private void Tcpserive_Load(object sender, EventArgs e)
        {
            SeriveList.Items.Clear();
            foreach (TcpseriveCom tcpcom in TcpMgr.TcpserList)
            {
                SeriveList.Items.Add(tcpcom.Name);
                tcpcom.EventCommu += dispMes;
            }
            if (SeriveList.Items.Count > 0)
            {
                SeriveList.SelectedIndex = 0;
                textBox1.Text = TcpMgr.TcpserList[SeriveList.SelectedIndex].Port.ToString();
            }
           
        }

        private void Tcpserive_FormClosing(object sender, FormClosingEventArgs e)
        {
            TcpMgr.TcpseriveComMgr_Save(Para.MchConfigFileName);

            foreach (TcpseriveCom tcpcom in TcpMgr.TcpserList)
            {
                tcpcom.EventCommu -= dispMes;
            }
        }

        private void set_Click(object sender, EventArgs e)
        {
            if (SeriveList.Items.Count == 0)
                return;
            int select = SeriveList.SelectedIndex;

            TcpMgr.TcpserList[select].SendMessage(setmes.Text);
        }

        public void dispMes(string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>((strrr)=>
                {
                    textBox2.Text = strrr;
                }),str);       
            }
            else
            {
                textBox2.Text =str;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (SeriveList.Items.Count == 0)
                return;
            int select = SeriveList.SelectedIndex;

            TcpMgr.TcpserList[select].Disconnect();

        }

        private void SeriveList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SeriveList.Items.Count == 0)
                return;
            int select = SeriveList.SelectedIndex;

            textBox1.Text = TcpMgr.TcpserList[select].Port.ToString();
        }

        private void portsetbutton_Click(object sender, EventArgs e)
        {
            if (SeriveList.Items.Count == 0)
                return;
            int select = SeriveList.SelectedIndex;

            TcpMgr.TcpserList[select].Port = int.Parse(textBox1.Text);

        }







    }
}

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

        public int dispMes(string str)
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
            return 0;
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
            disp_Speed(TcpMgr.TcpserList[select]);

        }

        private void portsetbutton_Click(object sender, EventArgs e)
        {
            if (SeriveList.Items.Count == 0)
                return;
            int select = SeriveList.SelectedIndex;

            TcpMgr.TcpserList[select].Port = int.Parse(textBox1.Text);
        }

        public void disp_Speed(TcpseriveCom tc)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    combPower.Text = tc.Power;
                    speedBar.Value = tc.Speed;
                    AccelBar.Value = tc.Accel;
                    Speedbox.Text = tc.Speed.ToString();
                    Accelbox.Text = tc.Accel.ToString();
                }));
            }
            else
            {
                combPower.Text = tc.Power;
                speedBar.Value = tc.Speed;
                AccelBar.Value = tc.Accel;
                Speedbox.Text = tc.Speed.ToString();
                Accelbox.Text = tc.Accel.ToString();
            }
        }

        private void speedBar_Scroll(object sender, EventArgs e)
        {
            Speedbox.Text = speedBar.Value.ToString();
        }

        private void AccelBar_Scroll(object sender, EventArgs e)
        {
            Accelbox.Text = AccelBar.Value.ToString();
        }

        private void speedSavebotton_Click(object sender, EventArgs e)
        {
            if (SeriveList.Items.Count == 0)
                return;
            int select = SeriveList.SelectedIndex;
            TcpMgr.TcpserList[select].Power = combPower.Text;
            TcpMgr.TcpserList[select].Speed = speedBar.Value;
            TcpMgr.TcpserList[select].Accel = AccelBar.Value;
        }







    }
}

using JPTCG.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpSerive
{
    public partial class Form1 : Form
    {

        string basepath = AppDomain.CurrentDomain.BaseDirectory;
        string filepath;

        TcpseriveComMgr tcpMgr = new TcpseriveComMgr();

        public Form1()
        {
            InitializeComponent();
            tcpMgr.TcpseriveComMgr_Add("tp1");
            tcpMgr.TcpserList[0].EventConState += disp_comstate;
            //tcpMgr.TcpserList[0].EventCommu += SetA176_Mes;
            filepath = basepath + "\\setting.xml";
            Para.MchConfigFileName = filepath;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tcpMgr.TcpseriveComMgr_Load(filepath);
            timer1.Enabled = true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            tcpMgr.TcpseriveComMgr_Win();
        }


        public void disp_comstate(string str,bool col)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string, bool>((strr, coll) =>
                {
                    textBox1.Text = strr;
                    if (coll)
                        state.BackColor = Color.Green;
                    else
                        state.BackColor = Color.Red;
                }), str, col);
            }
            else
            {
                textBox1.Text = str;
                if (col)
                    state.BackColor = Color.Green;
                else
                    state.BackColor = Color.Red;
            }
        }

        private void begin_Click(object sender, EventArgs e)
        {
            Para.setA176 = "";
            begin.BackColor = Color.Green;
        }

        private void stop_Click(object sender, EventArgs e)
        {
            Para.setA176 = "stopping";
            begin.BackColor = Color.White;
        }


        string settrr = "";//"compeleted;0;0;end";"wait;end"
        //"started;end";

        public void SetA176_Mes(string mess)
        {
            int res = 0;
            string setstrRo;
            if (mess == "")
            {
                return;
            }
            else
            {
                try
                {
                    string[] strrlist = mess.Split(';');
                    setstrRo = "";
                    switch (strrlist[0] +";"+ strrlist[strrlist.Length-1])
                    {
                        case "compelet;end"://取料NG
                            Para.A176CurMod1 = strrlist[1];
                            Para.A176CurMod2 = strrlist[2];
                            if (Para.A176PutCG)
                            {
                                setstrRo = "compeleted" + ";" + Para.A162CurMod1 + ";" + Para.A162CurMod2 + ";" + "end";
                            }
                            else
                            {
                                setstrRo = "wait;end";
                            }
                            break;
                        case "start;end"://启动机台
                            Para.A162start = true;
                            Thread.Sleep(800);
                            if (Para.A162start)
                            {
                                setstrRo = "started;end";
                            }
                            else
                            {
                                setstrRo = "startNG;end";
                            }
                            break;
                        default:
                            setstrRo = "NG";
                            break;
                    }
                    Para.A176PutCG = false;//清除上料标志
                    Para.A162start = false;//清除启动标志
                    tcpMgr.TcpserList[0].SendMessage(setstrRo);
                }
                catch
                {
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Para.A176PutCG = radioButton2.Checked;
            Para.A162CurMod1= comboBox1.Text;
            Para.A162CurMod2 = comboBox2.Text;

            M1to.Text = Para.A176CurMod1;
            M2to.Text = Para.A176CurMod2;

            if(Para.A162start)
            {
                button3.BackColor = Color.Green;
                timer2.Enabled = true;
            }
            else
            {
                button3.BackColor = Color.White;
            }

            if (comboBox1.Text == "3")
            {
                button4.Text = "NG2";
                button4.BackColor = Color.Red;
                
            }
            else if (comboBox1.Text == "2")
            {
                button4.Text = "NG1";
                button4.BackColor = Color.Yellow;
            }
            else if (comboBox1.Text == "1")
            {
                button4.Text = "OK";
                button4.BackColor = Color.Green;
            }
            else
            {
                button4.Text = "NON";
                button4.BackColor = Color.White;
            }




            if (comboBox2.Text == "3")
            {
                button5.Text = "NG2";
                button5.BackColor = Color.Red;

            }
            else if (comboBox2.Text == "2")
            {
                button5.Text = "NG1";
                button5.BackColor = Color.Yellow;
            }
            else if (comboBox2.Text == "1")
            {
                button5.Text = "OK";
                button5.BackColor = Color.Green;
            }
            else
            {
                button5.Text = "NON";
                button5.BackColor = Color.White;
            }









        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int k = 0;

            for (int i = 0; i < 80;i++)
            {
                Thread.Sleep(30);
                textBox3.Text = (k + 1).ToString();
                Application.DoEvents();
            }
            timer2.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false; 
        }
       




    }
}

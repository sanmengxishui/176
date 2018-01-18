using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Serial_Port
{
    public partial class SP_Win : Form
    {
        SerailComMgr SerailMgr;

        public SP_Win(SerailComMgr SMgr)
        {
            InitializeComponent();
            SerailMgr = SMgr;
        }

        private void WinSerailComMgr_Load(object sender, EventArgs e)
        {
            //SerialPort.GetPortNames();//获得当前计算机的所有串口名字
            SerialLB.Items.Clear();
            for (int i = 0; i < SerailMgr.SerailComList.Count; i++)
            {
                SerialLB.Items.Add(SerailMgr.SerailComList[i].MyName);
                SerailMgr.SerailComList[i].ReceiveEventhandle += Disp;//将事件加载到显示
            }
            if (SerialLB.Items.Count != 0)
            {
                SerialLB.SelectedIndex = 0;
                UpdateUI(0);
            }

            Light1.Text = Light1trackbar.Value.ToString();
            Light2.Text = Light2trackbar.Value.ToString();
        }

        private void WinSerailComMgr_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < SerailMgr.SerailComList.Count; i++)
            {
                SerailMgr.SerailComList[i].ReceiveEventhandle -= Disp;//将事件去除
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (SerialLB.SelectedItem != null)
                {
                    SerailCom SelectPort = SerailMgr.SerailComList[SerialLB.SelectedIndex];//SerialLB.SelectedItem as Panasonic_PLC;

                    if (SelectPort.DisConnect())//关闭串口
                    {
                        SelectPort.Port.PortName = PortCB.Text.Trim();
                        SelectPort.Port.BaudRate = int.Parse(RateCB.Text.Trim());
                        SelectPort.Port.DataBits = int.Parse(BitCB.Text.Trim());
                        //停止位赋值
                        if (StopBitCB.Text.Trim() == "One")
                            SelectPort.Port.StopBits = StopBits.One;
                        else if (StopBitCB.Text.Trim() == "OnePointFive")
                            SelectPort.Port.StopBits = StopBits.OnePointFive;
                        else
                            SelectPort.Port.StopBits = StopBits.Two;
                        //奇偶校验位赋值
                        if (ParityCB.Text.Trim() == "None")
                            SelectPort.Port.Parity = Parity.None;
                        else if (ParityCB.Text.Trim() == "Odd")
                            SelectPort.Port.Parity = Parity.Odd;
                        else
                            SelectPort.Port.Parity = Parity.Even;

                        SelectPort.Port.ReadTimeout = Convert.ToInt32(RSNUD.Value);

                        if (SelectPort.Connect())//打开串口，如果能够打开则保存数据
                        {
                            SerailMgr.SerailComList_Save();
                            MessageBox.Show(SelectPort.Port.PortName + "保存成功");
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("串口数据保存失败，请检查参数及线路是否正确");
            }
        }


        private void UpdateUI(int selectItem)
        {
            PortCB.Text = SerailMgr.SerailComList[selectItem].Port.PortName;
            RateCB.Text = SerailMgr.SerailComList[selectItem].Port.BaudRate.ToString();
            BitCB.Text = SerailMgr.SerailComList[selectItem].Port.DataBits.ToString();
            StopBitCB.Text = SerailMgr.SerailComList[selectItem].Port.StopBits.ToString();
            ParityCB.Text = SerailMgr.SerailComList[selectItem].Port.Parity.ToString();
            RSNUD.Value = Convert.ToInt32(SerailMgr.SerailComList[selectItem].Port.ReadTimeout);
        }

        private void SerialLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SerialLB.SelectedItem != null)
            {
                UpdateUI(SerialLB.SelectedIndex);
            }
        }

        private void Set_Click(object sender, EventArgs e)
        {
            SerailMgr.SerailComList[SerialLB.SelectedIndex].set_mes(textBox1.Text);
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        public void Disp(string mes)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    textBox2.Text = mes;
                }));
            }
            else
            {
                textBox2.Text = mes;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (SerailMgr.SerailComList[SerialLB.SelectedIndex].Receive_Data!="")
            {
                //if (!SerailMgr.SerailComList[SerialLB.SelectedIndex].ReceiveOK(textBox3.Text.Trim()))
                //    MessageBox.Show("数据失败");
                //else
                //    MessageBox.Show("成功");
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SerailMgr.SerailComList[SerialLB.SelectedIndex].setLightSource_mes("1,"+Int32.Parse((Light1.Text)).ToString());
                SerailMgr.SerailComList[SerialLB.SelectedIndex].setLightSource_mes("2,"+Int32.Parse((Light2.Text)).ToString());
            }
            catch
            {
                MessageBox.Show("光源参数设置错误");
            }
        }

        private void Light1trackbar_Scroll(object sender, EventArgs e)
        {
            Light1.Text = Light1trackbar.Value.ToString();
        }

        private void Light2trackbar_Scroll(object sender, EventArgs e)
        {
            Light2.Text = Light2trackbar.Value.ToString();
        }

    }
}

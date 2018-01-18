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

namespace Panasonic_Communication
{
    public partial class WinPanasonic_PLC : Form
    {
        Panasonic_PLCMgr PanaMgr;
        public WinPanasonic_PLC(Panasonic_PLCMgr Pana)
        {
            InitializeComponent();
            PanaMgr = Pana;
        }

        private void WinPanasonic_PLC_Load(object sender, EventArgs e)
        {
            //SerialPort.GetPortNames();//获得当前计算机的所有串口名字
            SerialLB.Items.Clear();
            for (int i = 0; i < PanaMgr.PanaList.Count; i++)
            {
                SerialLB.Items.Add(PanaMgr.PanaList[i].MyName);
            }
            if (SerialLB.Items.Count != 0)
            {
                SerialLB.SelectedIndex = 0;
                UpdateUI(0);
            }
        }

        private void WinPanasonic_PLC_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (SerialLB.SelectedItem != null)
                {
                    Panasonic_PLC SelectPort = PanaMgr.PanaList[SerialLB.SelectedIndex];//SerialLB.SelectedItem as Panasonic_PLC;

                    if(SelectPort.DisConnect())//关闭串口
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
                            PanaMgr.PanaList_Save();
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
             PortCB.Text = PanaMgr.PanaList[selectItem].Port.PortName;
             RateCB.Text = PanaMgr.PanaList[selectItem].Port.BaudRate.ToString();
             BitCB.Text = PanaMgr.PanaList[selectItem].Port.DataBits.ToString();
             StopBitCB.Text = PanaMgr.PanaList[selectItem].Port.StopBits.ToString();
             ParityCB.Text = PanaMgr.PanaList[selectItem].Port.Parity.ToString();
             RSNUD.Value = Convert.ToInt32(PanaMgr.PanaList[selectItem].Port.ReadTimeout);
        }

        private void SerialLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SerialLB.SelectedItem != null)
            {
                UpdateUI(SerialLB.SelectedIndex);
            }
        }

        //单寄存器写入
        private void button2_Click(object sender, EventArgs e)
        {
            if ((SerialLB.SelectedItem != null) && (textBox1.Text.Trim() != "") && (textBox5.Text.Trim() != ""))
            {
                PanaMgr.PanaList[SerialLB.SelectedIndex].Write_Single_DT(int.Parse(textBox5.Text.Trim()), Convert.ToInt16(textBox1.Text.Trim()));
            }
        }
        //单寄存器读取
        private void button3_Click(object sender, EventArgs e)
        {
            if ((SerialLB.SelectedItem != null) && (textBox5.Text.Trim() != ""))
            {
                textBox2.Text = PanaMgr.PanaList[SerialLB.SelectedIndex].Read_Single_DT(int.Parse(textBox5.Text.Trim())).ToString();
            }

        }
        //双寄存器写入
        private void button4_Click(object sender, EventArgs e)
        {
            if ((SerialLB.SelectedItem != null) && (textBox4.Text.Trim() != "") && (textBox5.Text.Trim() != ""))
            {
                PanaMgr.PanaList[SerialLB.SelectedIndex].Write_Double_DT(int.Parse(textBox5.Text.Trim()), Convert.ToInt32(textBox4.Text.Trim()));
            }

        }
        //双寄存器读取
        private void button5_Click(object sender, EventArgs e)
        {
            if ((SerialLB.SelectedItem != null) && (textBox5.Text.Trim() != ""))
            {
                textBox3.Text = PanaMgr.PanaList[SerialLB.SelectedIndex].Read_Double_DT(int.Parse(textBox5.Text.Trim())).ToString();
            }
        }

        //单点写入
        private void button10_Click(object sender, EventArgs e)
        {
            if ((comboBox1.SelectedItem != null) && (textBox10.Text.Trim() != "") && (comboBox2.SelectedItem != null))
            {
                switch (comboBox1.Text)
                {
                    case "X":
                        PanaMgr.PanaList[SerialLB.SelectedIndex].Write_Single_X(int.Parse(textBox10.Text.Trim()), Convert.ToInt16(comboBox2.Text));
                        break;
                    case "Y":
                        PanaMgr.PanaList[SerialLB.SelectedIndex].Write_Single_Y(int.Parse(textBox10.Text.Trim()), Convert.ToInt16(comboBox2.Text));
                        break;
                    case "R":
                        PanaMgr.PanaList[SerialLB.SelectedIndex].Write_Single_R(int.Parse(textBox10.Text.Trim()), Convert.ToInt16(comboBox2.Text));
                        break;
                    default:
                        MessageBox.Show("请选择正确单点寄存器");
                        break;
                }
            }
        }

        //单点读取
        private void button1_Click(object sender, EventArgs e)
        {
            if ((comboBox1.SelectedItem != null) && (textBox10.Text.Trim() != ""))
            {
                switch (comboBox1.Text)
                {
                    case "X":
                        textBox6.Text = PanaMgr.PanaList[SerialLB.SelectedIndex].Read_Single_X(int.Parse(textBox10.Text.Trim())).ToString();
                        break;
                    case "Y":
                        textBox6.Text = PanaMgr.PanaList[SerialLB.SelectedIndex].Read_Single_Y(int.Parse(textBox10.Text.Trim())).ToString();
                        break;
                    case "R":
                        textBox6.Text = PanaMgr.PanaList[SerialLB.SelectedIndex].Read_Single_R(int.Parse(textBox10.Text.Trim())).ToString();
                        break;
                    default:
                        MessageBox.Show("请选择正确单点寄存器");
                        break;
                } 
            }  
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //PanaMgr.PanaList[SerialLB.SelectedIndex].Write_Serial();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            //PanaMgr.PanaList[SerialLB.SelectedIndex].Read_Serial();

        }

        
       



      

        

        
    
    }
}

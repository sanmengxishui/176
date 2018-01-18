using Common;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Serial_Port
{
    public class SerailCom
    {
        public SerialPort Port = new SerialPort();
        public string MyName = "";

        public string Receive_Data = "";//接收返回的数据

        public delegate void ReceiveHandle(string remes);
        public event ReceiveHandle ReceiveEventhandle;

        public SerailCom(string name)
        {
            MyName = name;

            Port.PortName = "COM1";
            Port.BaudRate = 9600;
            Port.DataBits = 8;
            Port.StopBits = StopBits.One;
            Port.Parity = Parity.None;
            Port.ReadTimeout = 200;//读取超时
            Port.WriteTimeout = 200;//写入超时
        }

        public void SerailCom_Load(string fileName)
        {
            string Header = "";
            string strread = "";

            Header = "SerailCom_" + MyName;
            FileOperation.ReadData(fileName, Header, "PortName", ref strread);
            if (strread != "0")
                Port.PortName = strread.Trim();
            FileOperation.ReadData(fileName, Header, "BaudRate", ref strread);
            if (strread != "0")
                Port.BaudRate = int.Parse(strread);
            FileOperation.ReadData(fileName, Header, "DataBits", ref strread);
            if (strread != "0")
                Port.DataBits = int.Parse(strread);

            //停止位//One,OnePointFive,Two
            FileOperation.ReadData(fileName, Header, "StopBits", ref strread);
            if (strread != "0")
            {
                if (strread == "One")
                    Port.StopBits = StopBits.One;
                else if (strread == "OnePointFive")
                    Port.StopBits = StopBits.OnePointFive;
                else
                    Port.StopBits = StopBits.Two;
            }

            //奇偶校验位//None,Odd,Even   
            FileOperation.ReadData(fileName, Header, "Parity", ref strread);
            if (strread != "0")
            {
                if (strread == "None")
                    Port.Parity = Parity.None;
                else if (strread == "Odd")
                    Port.Parity = Parity.Odd;
                else
                    Port.Parity = Parity.Even;
            }
            FileOperation.ReadData(fileName, Header, "ReadTimeout", ref strread);
            if (strread != "0")
                Port.ReadTimeout = int.Parse(strread);
            FileOperation.ReadData(fileName, Header, "ReadTimeout", ref strread);
            if (strread != "0")
                Port.WriteTimeout = int.Parse(strread);
        }

        public void SerailCom_Save(string fileName)
        {
            string Header = "";

            Header = "SerailCom_" + MyName;

            FileOperation.SaveData(fileName, Header, "PortName", Port.PortName);
            FileOperation.SaveData(fileName, Header, "BaudRate", Port.BaudRate.ToString());
            FileOperation.SaveData(fileName, Header, "DataBits", Port.DataBits.ToString());
            FileOperation.SaveData(fileName, Header, "StopBits", Port.StopBits.ToString());
            FileOperation.SaveData(fileName, Header, "Parity", Port.Parity.ToString());
            FileOperation.SaveData(fileName, Header, "ReadTimeout", Port.ReadTimeout.ToString());
        }

        /// <summary>
        /// 连接串口
        /// </summary>
        /// <param name="PortName"></param>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                //Port.DataReceived += ReseiveData;
                if (Port.IsOpen)
                    Port.Close();
                Port.Open();
            }
            catch
            {
                MessageBox.Show(MyName + "连接失败");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns></returns>
        public bool DisConnect()
        {
            try
            {
                if ((Port != null) && (Port.IsOpen))
                {
                    Port.Close();
                    //Port = null;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        object ob = new object();
        /// <summary>
        /// 订阅串口事件的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReseiveData(object sender, SerialDataReceivedEventArgs e)
        {
            lock (ob)
            {
                try
                {
                    string str = Port.ReadTo("\r");
                    Receive_Data += str.Trim();//去除字符串前后的空格
                    Port.DiscardInBuffer();//清空输入缓存区
                    if (ReceiveEventhandle != null)
                    {
                        ReceiveEventhandle(str);
                    }
                }
                catch
                {
                }
            }
        }


        public bool IsConnected()
        {
            if ((Port != null) && (Port.IsOpen))
                return true;
            return false;
        }

        public bool set_mes(string mes)
        {
            try
            {
                Port.DiscardOutBuffer();//清空输出缓存区

                Port.Write(mes+"\r");
                //byte[] sgt = Encoding.Default.GetBytes(mes);
                //Port.Write(sgt, 0, sgt.Length);  
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool setLightSource_mes(string mes)
        {
            try
            {
                for (int i = 0; i < 3; i++)//1016
                {
                    byte[] k = new byte[80];//Port.ReadBufferSize];
                    Port.DiscardOutBuffer();//清空输出缓存区
                    Port.DiscardInBuffer();//清空输入缓存区
                    Port.Write(mes + "\r\n");
                    string hu = Port.ReadTo("\r");
                    if (mes == hu)
                    {
                        return true;
                    }
                    Thread.Sleep(50);
                }
            }
            catch
            {
                MessageBox.Show("光源串口出错");
                return false;
            }
            return false;
        }



        public string ReceiveOK()
        {
            if (Receive_Data != "")
            {
                Thread.Sleep(120);
                string ReStr = Receive_Data;
                Receive_Data = "";
                return ReStr;
            }
            return "";
        }



    }
}

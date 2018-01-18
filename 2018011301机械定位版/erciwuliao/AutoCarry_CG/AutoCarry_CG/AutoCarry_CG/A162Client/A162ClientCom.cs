using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace A162Client
{
    public class A162ClientCom
    {
        public delegate void OnDataSendRecevie(Color statu);
        public event OnDataSendRecevie OnSendAndRec;

        public delegate int SWconnect(string msg, NetworkStream net);
        public event SWconnect EventSWconnect;


        private TcpClient tcpClient;
        byte[] Data = new byte[500];

        private string name = "";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Port = 5566; 
        public string IP = "192.168.0.100 ";

        public bool IsConnected
        {
            get { return tcpClient.Connected; }
        }
        public A162ClientCom(string myName)
        {
            name = myName;
            tcpClient = new TcpClient();
        }
        ~A162ClientCom()
        {
            if (tcpClient.Connected)
                tcpClient.Close();
        }
        public void SaveSettings(string fileName)
        {
            string Header = "";

            Header = "A162ClientCom" + Name;
            FileOperation.SaveData(fileName, Header, "IP", IP);
            FileOperation.SaveData(fileName, Header, "Port", Port.ToString());
        }
        public void LoadSettings(string fileName)
        {
            string Header = "";
            string strread = "";

            Header = "A162ClientCom" + Name;

            FileOperation.ReadData(fileName, Header, "IP", ref strread);
            IP = strread;

            FileOperation.ReadData(fileName, Header, "Port", ref strread);
            Port = int.Parse(strread);

            if (!Connect(IP, Port))
                MessageBox.Show(Name + " can't be connected.");
        }

        public bool Connect(string ip, int PortNo)
        {
            bool res = false;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Disconnect();
                    Thread.Sleep(100);
                    //IP = ip;
                    //Port = PortNo;
                    tcpClient = new TcpClient();
                    tcpClient.Connect(ip, PortNo);
                    WaitMessageFromSever();
                    //if ("client;hiAuto456;set" != Read("server;helloCG123;end"))
                    //{
                    //    break;
                    //}
                    //if ("accept;can" != Read("connect;may"))
                    //{
                    //    break;
                    //}
                    //if ("success;allow" != Read("communicate;OK"))
                    //{
                    //    break;
                    //}
                    if (OnSendAndRec != null)
                        OnSendAndRec(Color.Green);//("Connected with RobotCom at " + ip + ":" + PortNo);
                   
                    res = true;
                }
                catch
                {
                    if (OnSendAndRec != null)
                        OnSendAndRec(Color.Red);//("Fail to Connect with RobotCom at " + ip + ":" + PortNo);
                    break;
                }
            }
            return res;
        }
        public void Disconnect()
        {
            if (tcpClient.Connected)
            {
                if (OnSendAndRec != null)
                    OnSendAndRec(Color.Red);//("Disconnected with RobotCom at" + IP);
                tcpClient.Close();
            }
        }

        void WaitMessageFromSever()
        {
            try
            {
                tcpClient.GetStream().BeginRead(Data, 0, Data.Length, ReceiveMessage, null);
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }

        string StrA162 = "";
        object obj = new object();
        public void ReceiveMessage(IAsyncResult ar)
        {
            int bufferLength;
            try
            {
                lock (obj)
                {
                    bufferLength = tcpClient.GetStream().EndRead(ar);
                    StrA162 = (System.Text.Encoding.ASCII.GetString(Data, 0, bufferLength)).ToString();
                    NetworkStream netStr = tcpClient.GetStream();
                    //if (EventSWconnect != null)
                    //    EventSWconnect(StrA162, netStr);
                    WaitMessageFromSever();
                }
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }
        public bool SendMessage(Byte[] msg)
        {
            try
            {
                NetworkStream netStr = tcpClient.GetStream();
                netStr.Write(msg, 0, msg.Length);
                netStr.Flush();
                if (OnSendAndRec != null)
                {
                    var str = System.Text.Encoding.Default.GetString(msg);
                    //OnSendAndRec(IP + " Send::" + str);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string Read(string strr)
        {
            StrA162 = "";
            Byte[] command = ASCIIEncoding.ASCII.GetBytes(strr);
            SendMessage(command);
            DateTime st_time = DateTime.Now;

            TimeSpan time_span;
            while (StrA162 == "")
            {
                Thread.Sleep(10);
                Application.DoEvents();
                time_span = DateTime.Now - st_time;
                if (time_span.TotalMilliseconds > 2000)
                {
                    break;
                }
            }
            if (StrA162 != "")
                return StrA162.Replace("\r", "");//.Substring(0, barcode.IndexOf(':'));
            else
                return "ConnectNG";
        }


    }
}

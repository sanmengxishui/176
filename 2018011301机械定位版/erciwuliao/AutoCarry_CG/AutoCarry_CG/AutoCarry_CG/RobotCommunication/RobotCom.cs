using Common;
using JPTCG.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotCommunication
{
    public class RobotCom
    {
        public delegate void OnDataSendRecevie(string msg);
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
        public RobotCom(string myName)
        {
            name = myName;
            tcpClient = new TcpClient();
        }
        ~RobotCom()
        {
            if (tcpClient.Connected)
                tcpClient.Close();
        }
        public void SaveSettings(string fileName)
        {
            string Header = "";            
                
            Header = "RobotCom" + Name;
            FileOperation.SaveData(fileName, Header, "IP", IP);
            FileOperation.SaveData(fileName, Header, "Port", Port.ToString());
        }
        public void LoadSettings(string fileName)
        {
            string Header = "";
            string strread = "";

            Header = "RobotCom" + Name;

            FileOperation.ReadData(fileName, Header, "IP", ref strread);
            IP = strread;

            FileOperation.ReadData(fileName, Header, "Port", ref strread);
            Port = int.Parse(strread);

            if (Para.MachineOnline)
            {
                if (!Connect(IP, Port))
                MessageBox.Show(Name + " can't be connected.");
            }
        }

        public bool Connect(string ip, int PortNo)
        {
            bool res = false;
            try
            {
                Disconnect();
                IP = ip;
                Port = PortNo;
                tcpClient = new TcpClient();
                tcpClient.Connect(ip, PortNo);
                if (OnSendAndRec != null)
                    OnSendAndRec("Connected with RobotCom at " + ip + ":" + PortNo);
                WaitMessageFromSever();
                res = true;        
            }
            catch
            {
                if (OnSendAndRec != null)
                    OnSendAndRec("Fail to Connect with RobotCom at " + ip + ":" + PortNo);
            }
            return res;
        }
        public void Disconnect()
        {
            if (tcpClient.Connected)
            {
                if (OnSendAndRec != null)
                    OnSendAndRec("Disconnected with RobotCom at" + IP);
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

        string StrRobot = "";
        object obj = new object();
        public void ReceiveMessage(IAsyncResult ar)
        {
            int bufferLength;
            try
            {
                lock (obj)
                {
                    bufferLength = tcpClient.GetStream().EndRead(ar);
                    StrRobot = (System.Text.Encoding.ASCII.GetString(Data, 0, bufferLength)).ToString();
                    NetworkStream netStr = tcpClient.GetStream();
                    if (EventSWconnect != null)
                        EventSWconnect(StrRobot, netStr);
                    else
                    {
                        string end = "stopping\r";   // CR is terminator
                        Byte[] command = ASCIIEncoding.ASCII.GetBytes(end);
                        netStr.Write(command, 0, command.Length);
                        netStr.Flush();
                    }  
                    WaitMessageFromSever();
                }
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }
        //public bool SendMessage(Byte[] msg)
        //{
        //    try
        //    {
        //        NetworkStream netStr = tcpClient.GetStream();
        //        netStr.Write(msg, 0, msg.Length);
        //        netStr.Flush();
        //        if (OnSendAndRec != null)
        //        {
        //            var str = System.Text.Encoding.Default.GetString(msg);
        //            OnSendAndRec(IP + " Send::" + str);
        //        }
        //        return true;
        //        //AddToLog(message, true, DateTime.Now);
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //public void LON()
        //{
        //    string lon = "LON\r";   // CR is terminator
        //    Byte[] command = ASCIIEncoding.ASCII.GetBytes(lon);
        //    SendMessage(command);

        //}
        //public void LOFF()
        //{
        //    string loff = "LOFF\r"; // CR is terminator
        //    Byte[] command = ASCIIEncoding.ASCII.GetBytes(loff);
        //    SendMessage(command);      
        //}

        //public string Read()
        //{            
        //    barcode = "";
        //    //LON();
        //    DateTime st_time = DateTime.Now;
        //    TimeSpan time_span;
        //    while (barcode == "")
        //    {
        //        Thread.Sleep(10);
        //        Application.DoEvents();
        //        time_span = DateTime.Now - st_time;
        //        if (time_span.TotalMilliseconds > 2000)
        //        {
        //            break;
        //        }
        //    }
        //    //LOFF();
        //    if (barcode != "")
        //        return barcode.Replace("\r","");//.Substring(0, barcode.IndexOf(':'));
        //    else
        //        return "";
        //}
    }
}

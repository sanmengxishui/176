using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpSerive
{
    public class TcpseriveCom
    {
        TcpListener listener;
        TcpClient client;

        public delegate void ConnectState(string str,bool bol);
        public event ConnectState EventConState;

        public delegate void Communication(string str);
        public event Communication EventCommu;

        private bool lockTcp1 = false;//1015
        private bool lockTcp2 = false;//1015
        private bool lockTcp3 = false;//1015

        public string Name;
        public int Port = 5566;
        byte[] Data = new byte[500];

        public TcpseriveCom(string name)
        {
            Name = name;
        }

        ~TcpseriveCom()
        {
            if (listener != null)
                listener.Stop();
            Disconnect();
        }

        public void Disconnect()
        {
            if (client != null)
            {
                if (client.Connected)
                {
                    if (EventConState != null)
                        EventConState(client.Client.RemoteEndPoint.ToString() + ":" + "DisConnect",false);
                    client.Close();
                    
                }
                client = null;
            }
        }
        public void SaveSettings(string fileName)
        {
            string Header = "";

            Header = "A162" + Name;
            FileOperation.SaveData(fileName, Header, "Port", Port.ToString());
        }
        public void LoadSettings(string fileName)
        {
            string Header = "";
            string strread = "";

            Header = "A162" + Name;

            FileOperation.ReadData(fileName, Header, "Port", ref strread);
            Port = int.Parse(strread);

            listener = new TcpListener(IPAddress.Any, Port);
            TcplistenerStart();
        }

        //等待异步的客户端连接
        public void TcplistenerStart()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(Listener_IAsy, null);
        }

        private void Listener_IAsy(IAsyncResult Ias)
        {
            client = listener.EndAcceptTcpClient(Ias);
            
            if (EventConState != null)
                EventConState(client.Client.RemoteEndPoint.ToString() + ":" + "Connected Succcess",true);
            WaitMessage();
            TcplistenerStart();
        }

        void WaitMessage()
        {
            try
            {
                client.GetStream().BeginRead(Data, 0, Data.Length, ReceiveMessage, null);
            }
            catch (Exception ex)
            {
                lockTcp1 = false;//1015
                lockTcp2 = false;//1015
                lockTcp3 = false;//1015
                client.Close();
            }
        }

        string ReceStr = "";
        object obj = new object();
        public void ReceiveMessage(IAsyncResult ar)
        {
            int bufferLength;
            try
            {
                lock (obj)
                {
                    bufferLength = client.GetStream().EndRead(ar);

                    ReceStr = (System.Text.Encoding.ASCII.GetString(Data, 0, bufferLength)).ToString();
                    //if (lockTcp1 && lockTcp2 && lockTcp3)//先进行握手，否则就只发送"lock"
                    //{
                        if (EventCommu != null)
                        {
                            EventCommu(ReceStr);
                        }
                        else
                        {
                            SendMessage("stoping");
                        }
                    //}
                    //{
                        //if (ReceStr == "server;helloCG123;end")
                        //{
                        //    lockTcp1 = true;
                        //    SendMessage("client;hiAuto456;set");
                        //}
                        //else if (ReceStr == "connect;may")
                        //{
                        //    lockTcp2 = true;
                        //    SendMessage("accept;can");
                        //}
                        //else if (ReceStr == "communicate;OK")
                        //{
                        //    lockTcp3 = true;
                        //    SendMessage("success;allow");
                        //}
                        //else
                        //    SendMessage("lock");
                    //}
                    WaitMessage();
                }
            }
            catch (Exception ex)
            {
                return;
                lockTcp1 = false;//1015
                lockTcp2 = false;//1015
                lockTcp3 = false;//1015
                client.Close();
                //Disconnect();
            }
        }

        object obji = new object();
        public bool SendMessage(string setstr)
        {
            try
            {
                lock (obji)
                {
                    byte[] msg = Encoding.ASCII.GetBytes(setstr);
                    NetworkStream netStr = client.GetStream();
                    netStr.Write(msg, 0, msg.Length);
                    netStr.Flush();
                    return true;
                }   
            }
            catch (Exception ex)
            {
                lockTcp1 = false;//1015
                lockTcp2 = false;//1015
                lockTcp3 = false;//1015
                return false;
            }
        }











       







    }
}

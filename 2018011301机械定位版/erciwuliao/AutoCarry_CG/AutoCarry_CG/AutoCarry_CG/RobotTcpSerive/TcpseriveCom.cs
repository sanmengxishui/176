using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public delegate void ConnectState(Color statu,bool stu);
        public event ConnectState EventConState;

        public delegate int Communication(string str);
        public event Communication EventCommu;

        public string Name;
        public int Port = 5566;
        byte[] Data = new byte[500];

        public string Power;
        public int Speed;
        public int Accel;


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
                        EventConState(Color.Red,false);//(client.Client.RemoteEndPoint.ToString() + ":" + "DisConnect",false);
                    client.Close();
                }
            }
        }
        public void SaveSettings(string fileName)
        {
            string Header = "";

            Header = "A162" + Name;
            FileOperation.SaveData(fileName, Header, "Port", Port.ToString());
            FileOperation.SaveData(fileName, Header, "power", Power);
            FileOperation.SaveData(fileName, Header, "speed", Speed.ToString());
            FileOperation.SaveData(fileName, Header, "accel", Accel.ToString());

        }
        public void LoadSettings(string fileName)
        {
            string Header = "";
            string strread = "";

            Header = "A162" + Name;

            FileOperation.ReadData(fileName, Header, "Port", ref strread);
            Port = int.Parse(strread);

            FileOperation.ReadData(fileName, Header, "power", ref strread);
            if (strread == "High" || strread == "Low")
                Power = strread;
            else
                Power = "Low";

            FileOperation.ReadData(fileName, Header, "speed", ref strread);
            if ((0 <= int.Parse(strread)) && (int.Parse(strread) <= 100))
                Speed = int.Parse(strread);
            else
                Speed = 5;
            
            FileOperation.ReadData(fileName, Header, "accel", ref strread);
            if ((0 <= int.Parse(strread)) && (int.Parse(strread) <= 120))
                Accel = int.Parse(strread);
            else
                Accel = 10;

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
                EventConState(Color.Green,true);//(client.Client.RemoteEndPoint.ToString() + ":" + "Connected Succcess",true);
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
                client.Close();
            }
        }

        string ReceStr = "";
        string setstr = "";
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
                    if (ReceStr == "")
                    {
                        EventConState(Color.Red,false);
                        return;
                    }
                    if (EventCommu != null)
                        EventCommu(ReceStr);
                    else
                    {
                        Thread.Sleep(100);
                        SendMessage("A176stoping");
                    }   
                    WaitMessage();
                }
            }
            catch (Exception ex)
            {
                client.Close();
            }
        }

        object obji = new object();
        public bool SendMessage(string setstr)
        {
            try
            {
                lock (obji)
                {
                    byte[] msg = Encoding.ASCII.GetBytes(setstr+"\r");
                    NetworkStream netStr = client.GetStream();
                    netStr.Write(msg, 0, msg.Length);
                    netStr.Flush();
                    return true;
                }   
            }
            catch (Exception ex)
            {
                return false;
            }
        }











       







    }
}

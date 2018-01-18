using Common;
using JPTCG.Common;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Panasonic_Communication
{
  
    public class Panasonic_PLC
    {
        public SerialPort Port = new SerialPort();
        public string MyName = "";

        public string Receive_Data = "";//PLC返回的数据
        public List<Int32> Serial_Data = new List<Int32>();//用于连续读取的数据

        string station = "01";//PLC站点设置必须是两位"01"

        public string rsdata;
        
        public Panasonic_PLC(string name)
        {
            MyName = name;

            Port.PortName = "COM1";
            Port.BaudRate = 9600;
            Port.DataBits = 8;
            Port.StopBits = StopBits.One;
            Port.Parity = Parity.None;
            Port.ReadTimeout = 3000;//读取超时
            Port.WriteTimeout = 3000;//写入超时
        }

        public void Pana_Load(string fileName)
        {
            string Header = "";
            string strread = "";

            Header = "Panan_" + MyName;
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

        public void Pana_Save(string fileName)
        {
            string Header = "";

            Header = "Panan_" + MyName;

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
                if (Port.IsOpen)
                    Port.Close();
                Port.Open();
            }
            catch
            {
                MessageBox.Show(MyName+"连接失败");
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

        public bool IsConnected()
        {
            if ((Port != null) && (Port.IsOpen))
                return true;
            return false;
        }
      
       
        public bool Write_Serial_Data(int St_DT,int Ed_DT,List<int> DT_Data)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    if (DT_Data.Count != Ed_DT - St_DT + 1)//比较发送D寄存器的数量是否和数据是否在数量上相等
                        return false;
                    string setmessage = "%" + station + "#WDD" + St_DT.ToString("00000") + Ed_DT.ToString("00000");
                    for (int i = 0; i < DT_Data.Count; i++)
                    {
                        setmessage += HL_Exchange(DT_Data[i].ToString("X4")); //"X4"表示变成4位16进制数据
                    }
                    setmessage += "**" + "\r\n";

                    //Port.DiscardOutBuffer();//清空发送缓存
                    //Port.DiscardInBuffer();//清空接收缓存

                    //Port.Write(setmessage);//将数据发送

                    //string res = Port.ReadTo("\r");
                    string res = commPLc(setmessage);

                    //接收回来的字符串中如果含有‘$’这个表示指令运行正确
                    if (res.Contains('$') && res.Length == 8)
                        return true;
                }
                catch
                {
                    break;
                }
            }  
            return false;
        }
        /// <summary>
        /// 连续寄存器的的数据读取
        /// </summary>
        /// <param name="station">站号必须是（两位）</param>
        /// <param name="St_DT">起始DT地址（五位）</param>
        /// <param name="Ed_DT">结束DT地址（五位）</param>
        /// <returns></returns>
        public bool Read_Serial_Data(int St_DT,int Ed_DT)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    //Port.DiscardOutBuffer();//清空输出缓存区
                    //Port.DiscardInBuffer();//清空输入缓存区

                    //Port.Write("%" + station + "#RDD" + St_DT.ToString("00000") + Ed_DT.ToString("00000") + "**" + "\r");
                    //string res = Port.ReadTo("\r");
                    string setmessage = "%" + station + "#RDD" + St_DT.ToString("00000") + Ed_DT.ToString("00000") + "**" + "\r";
                    string res = commPLc(setmessage);

                    if (res.Contains('$') && res.Length == ((Ed_DT - St_DT) * 4 + 12))
                    {
                        res = res.Substring(6, res.Length - 6 - 2);//截取我们需要的字符串
                        Serial_Data.Clear();
                        for (int i = 0; i < res.Length / 4; i++)
                        {
                            Serial_Data.Add(HL_Exchange_16Dec(res.Substring(i * 4, 4)));//收到的数据进行高地位转换    
                        }
                        return true;
                    } 
                }
                catch
                {
                    break;
                }
            }
            
            return false;
        }

        Int16 HL_Exchange_16Dec(string str)//16位高地位转换，并16进制转10进制
        {
            string Hstr = str.Substring(0, 2);
            string Lstr = str.Substring(2, 2);
            return Convert.ToInt16(Lstr + Hstr,16); //16进制转10进制
        }

        Int32 HL_Exchange_32Dec(string str)//32位高地位转换，并16进制转10进制
        {
            string Hstr = HL_Exchange(str.Substring(0, 4));
            string Lstr = HL_Exchange(str.Substring(4, 4));
            return Convert.ToInt32(Lstr + Hstr, 16); //16进制转10进制
        }

        string HL_Exchange(string str)//高地位转换
        {
            string Hstr = str.Substring(0, 2);
            string Lstr = str.Substring(2, 2);
            return Lstr + Hstr; 
        }


        public bool Write_Single_DT(int DT,Int16 Data)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    string setmessage = "%" + station + "#WDD" + DT.ToString("00000") + DT.ToString("00000");
                    setmessage += HL_Exchange(Data.ToString("X4")); //"X4"表示变成4位16进制数据
                    setmessage += "**" + "\r\n";

                    //Port.DiscardOutBuffer();//清空发送缓存
                    //Port.DiscardInBuffer();//清空接收缓存

                    //Port.Write(setmessage);//将数据发送

                    //string res = Port.ReadTo("\r");

                    string res = commPLc(setmessage);

                    //接收回来的字符串中如果含有‘$’这个表示指令运行正确
                    if (res.Contains('$') && res.Length == 8)
                        return true;

                    //byte[] strbyte = new byte[Port.BytesToRead];
                    //Port.Read(strbyte, 0, strbyte.Length);
                    //对读回来的数据进行判断，是否正确
                    //string ju = Encoding.ASCII.GetString(strbyte);
                }
                catch
                {
                    //MessageBox.Show(e1.ToString());
                    break;
                }
            }
           
            return false;
        }


        public Int16 Read_Single_DT(int DT)//, out Int16 Data)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    Port.DiscardOutBuffer();//清空输出缓存区
                    Port.DiscardInBuffer();//清空输入缓存区

                    //Port.Write("%" + station + "#RDD" + DT.ToString("00000") + DT.ToString("00000") + "**" + "\r");
                    //string res = Port.ReadTo("\r");

                    string setmessage = "%" + station + "#RDD" + DT.ToString("00000") + DT.ToString("00000") + "**" + "\r";
                    string res = commPLc(setmessage);

                    if (res.Contains('$') && res.Length == 12)
                    {
                        //Data = 0;
                        res = res.Substring(6, 4);//截取4个16进制数据刚好是16位
                        return HL_Exchange_16Dec(res);  //Data = HL_Exchange_16Dec(res);//收到的数据进行高地位转换    
                    }
                }
                catch
                {
                    break;// Data = 0;
                }
            }
            return 555;
        }



        public bool Write_Double_DT(int DT, Int32 Data)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    Para.ki++;
                    string setmessage = "%" + station + "#WDD" + DT.ToString("00000") + (DT + 1).ToString("00000");
                    setmessage += HL_Exchange(Data.ToString("X8").Substring(4, 4)); //"X4"表示变成4位16进制数据
                    setmessage += HL_Exchange(Data.ToString("X8").Substring(0, 4));
                    setmessage += "**" + "\r";

                    //Port.DiscardOutBuffer();//清空发送缓存
                    //Port.DiscardInBuffer();//清空接收缓存

                    //Port.Write(setmessage);//将数据发送

                    //string res = Port.ReadTo("\r");

                    string res = commPLc(setmessage);
                    //接收回来的字符串中如果含有‘$’这个表示指令运行正确
                    if (res.Contains('$') && res.Length == 8)
                        return true;

                    //byte[] strbyte = new byte[Port.BytesToRead];
                    //Port.Read(strbyte, 0, strbyte.Length);
                    //对读回来的数据进行判断，是否正确
                    //string ju = Encoding.ASCII.GetString(strbyte);
                }
                catch
                {
                    //MessageBox.Show(e1.ToString());
                    break;
                }
            }
            
            return false;
        }

        public Int32 Read_Double_DT(int DT)//, Int32 Data)//, out int Rece_Data)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    Para.ki++;
                    Port.DiscardOutBuffer();//清空输出缓存区
                    Port.DiscardInBuffer();//清空输入缓存区

                    //Port.Write("%" + station + "#RDD" + DT.ToString("00000") + (DT + 1).ToString("00000") + "**" + "\r");
                    //string res = Port.ReadTo("\r");

                    string setmessage = "%" + station + "#RDD" + DT.ToString("00000") + (DT + 1).ToString("00000") + "**" + "\r";
                    string res = commPLc(setmessage);

                    if (res.Contains('$') && res.Length == 16)
                    {
                        res = res.Substring(6, 8);//截取我们需要的字符串
                        return HL_Exchange_32Dec(res);//Rece_Data = HL_Exchange_32Dec(res);
                    } 
                }
                catch
                {
                    break;
                }
            }
            return 555;
        }



        public bool Write_Single_R(int R, Int16 Data)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    string setmessage = "%" + station + "#WCP1R" + R.ToString("0000") + Data.ToString("0") + "**" + "\r\n";

                    //Port.DiscardOutBuffer();//清空发送缓存
                    //Port.DiscardInBuffer();//清空接收缓存

                    //Port.Write(setmessage);//将数据发送

                    //string res = Port.ReadTo("\r");
                    
                    string res = commPLc(setmessage);

                    //接收回来的字符串中如果含有‘$’这个表示指令运行正确
                    if (res.Contains('$')&&res.Length==8)
                    {
                        return true;
                    
                    }//&&res.Length==)
                        //return false;

                    //byte[] strbyte = new byte[Port.BytesToRead];
                    //Port.Read(strbyte, 0, strbyte.Length);
                    //对读回来的数据进行判断，是否正确
                    //string ju = Encoding.ASCII.GetString(strbyte);
                }
                catch
                {
                    break;
                }
            }
            
            return false;
        }

        public Int16 Read_Single_R(int R)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    //Port.DiscardOutBuffer();//清空输出缓存区
                    //Port.DiscardInBuffer();//清空输入缓存区

                    //Port.Write("%" + station + "#RCP1R" + R.ToString("0000") + "**" + "\r");
                    //string res = Port.ReadTo("\r");

                    string setmessage = "%" + station + "#RCP1R" + R.ToString("0000") + "**" + "\r";
                    string res = commPLc(setmessage);

                    rsdata = res;
                    if (res.Contains('$')&&res.Length==9)
                    {
                        return Convert.ToInt16(res.Substring(6, 1));//截取4个16进制数据刚好是16位
                        //return HL_Exchange_16Dec(res);  //Data = HL_Exchange_16Dec(res);//收到的数据进行高地位转换    
                    }
                       
                }
                catch
                {
                    break;
                }
            }
            return 555;
        }

        public bool Write_Single_Y(int Y, Int16 Data)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    string setmessage = "%" + station + "#WCP1Y" + Y.ToString("0000") + Data.ToString("0") + "**" + "\r\n";

                    //Port.DiscardOutBuffer();//清空发送缓存
                    //Port.DiscardInBuffer();//清空接收缓存

                    //Port.Write(setmessage);//将数据发送

                    //string res = Port.ReadTo("\r");
                    string res = commPLc(setmessage);

                    //接收回来的字符串中如果含有‘$’这个表示指令运行正确
                    if (res.Contains('$')&&res.Length==8)
                        return true;
                    //byte[] strbyte = new byte[Port.BytesToRead];
                    //Port.Read(strbyte, 0, strbyte.Length);
                    //对读回来的数据进行判断，是否正确
                    //string ju = Encoding.ASCII.GetString(strbyte);
                }
                catch
                {
                    break;
                }
            }
           
            return false;
        }

        public Int16 Read_Single_Y(int Y)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    //Port.DiscardOutBuffer();//清空输出缓存区
                    //Port.DiscardInBuffer();//清空输入缓存区

                    //Port.Write("%" + station + "#RCP1Y" + Y.ToString("0000") + "**" + "\r");
                    //string res = Port.ReadTo("\r");

                    string setmessage = "%" + station + "#RCP1Y" + Y.ToString("0000") + "**" + "\r";
                    string res = commPLc(setmessage);
                    if (res.Contains('$') && res.Length == 9)
                    {
                        return Convert.ToInt16(res.Substring(6, 1));//截取4个16进制数据刚好是16位
                        //return HL_Exchange_16Dec(res);  //Data = HL_Exchange_16Dec(res);//收到的数据进行高地位转换       
                    }  
                }
                catch
                {
                    break;
                }
            }
            return 555;
            
        }

        public bool Write_Single_X(int X, Int16 Data)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    string setmessage = "%" + station + "#WCP1X" + X.ToString("0000") + Data.ToString("0") + "**" + "\r\n";

                    //Port.DiscardOutBuffer();//清空发送缓存
                    //Port.DiscardInBuffer();//清空接收缓存

                    //Port.Write(setmessage);//将数据发送

                    //string res = Port.ReadTo("\r");
                    string res = commPLc(setmessage);
                    //接收回来的字符串中如果含有‘$’这个表示指令运行正确
                    if (res.Contains('$') && res.Length == 8)
                        return true;

                    //byte[] strbyte = new byte[Port.BytesToRead];
                    //Port.Read(strbyte, 0, strbyte.Length);
                    //对读回来的数据进行判断，是否正确
                    //string ju = Encoding.ASCII.GetString(strbyte);
                }
                catch
                {
                    break;
                }
            }
            
            return false;
        }

        public Int16 Read_Single_X(int X)
        {
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    //Port.DiscardOutBuffer();//清空输出缓存区
                    //Port.DiscardInBuffer();//清空输入缓存区

                    //Port.Write("%" + station + "#RCP1X" + X.ToString("0000") + "**" + "\r");
                    //string res = Port.ReadTo("\r");

                    string setmessage = "%" + station + "#RCP1X" + X.ToString("0000") + "**" + "\r";
                    string res = commPLc(setmessage);
                    if (res.Contains('$')&&res.Length==9)
                    {
                        return Convert.ToInt16(res.Substring(6, 1));//截取4个16进制数据刚好是16位
                        //return HL_Exchange_16Dec(res);  //Data = HL_Exchange_16Dec(res);//收到的数据进行高地位转换       
                    }  
                }
                catch
                {
                    break;
                }
            }
            return 555;

        }

        object oblock = new object();

        private string commPLc(string strr)
        {
            lock (oblock)
            {
                Para.zongshu++;
                try
                {
                    Port.DiscardOutBuffer();//清空输出缓存区
                    Port.DiscardInBuffer();//清空输入缓存区

                    Port.Write(strr);
                    return Port.ReadTo("\r");
                }
                catch
                {
                    
                }
                return "error";  
            }
        }




        #region 
        ////连续读取单个触点信息结果不颠倒，读取寄存器颠倒
        //public bool Write_Serial()
        //{
        //    try
        //    {
        //        //string setmessage = "%" + station.ToString("00") + "#WCP1Y" + Y.ToString("0000") + Data.ToString("0") + "**" + "\r\n";
        //        string setmessage = "%01#WCP5R00001R00010R00020R00031R00041**" + "\r";

        //        Port.DiscardOutBuffer();//清空发送缓存
        //        Port.DiscardInBuffer();//清空接收缓存

        //        Port.Write(setmessage);//将数据发送

        //        string res = Port.ReadTo("\r");
        //        //接收回来的字符串中如果含有‘$’这个表示指令运行正确
        //        if (!res.Contains('$'))
        //            return false;

        //        //byte[] strbyte = new byte[Port.BytesToRead];
        //        //Port.Read(strbyte, 0, strbyte.Length);
        //        //对读回来的数据进行判断，是否正确
        //        //string ju = Encoding.ASCII.GetString(strbyte);
        //    }
        //    catch
        //    {
        //        //MessageBox.Show(e1.ToString());
        //        return false;
        //    }
        //    return true;
        //}

        //public bool Read_Serial()
        //{
        //    try
        //    {
        //        Port.DiscardOutBuffer();//清空输出缓存区
        //        Port.DiscardInBuffer();//清空输入缓存区

        //        Port.Write("%01#RCP5R0000R0001R0002R0003R0004**" + "\r");
        //        string res = Port.ReadTo("\r");
        //        if (!res.Contains('$'))
        //        {
        //            return false;
        //        }
        //        return true;
        //        //return Convert.ToInt16(res.Substring(6, 1));//截取4个16进制数据刚好是16位
        //        //return HL_Exchange_16Dec(res);  //Data = HL_Exchange_16Dec(res);//收到的数据进行高地位转换       
        //    }
        //    catch
        //    {
        //        return false;// Data = 0;
        //        //return false;
        //    }
        //}
        #endregion
























    }
}

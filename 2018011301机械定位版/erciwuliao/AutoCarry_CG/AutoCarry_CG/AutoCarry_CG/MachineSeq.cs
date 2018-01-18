using A162Client;
using HalconDotNet;
using JPTCG.Common;
using JPTCG.Sequencing;
using JPTCG.Vision;
using Panasonic_Communication;
using RobotCommunication;
using Serial_Port;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TcpSerive;


namespace AutoCarry_CG
{
    public class MachineSeq
    {
        public HalconVision camera3;
        public RobotVision hWndCtrl3;
        public MachineSeq SeqMgr;
        public SerailComMgr SerailRobot;
        public mainWindow MianWin;
        public Panasonic_PLCMgr PanaPLC;
        public TcpseriveComMgr Myrobotmgr;
        public A162ClientComMgr MyA162comMgr;

        private List<FunctionObjects> robotSeqList = new List<FunctionObjects>();
        WorkerThread robotSeq;

        public MachineSeq(HalconVision cam, RobotVision hWnd3, SerailComMgr SeRobot, Panasonic_PLCMgr pana_plc, TcpseriveComMgr robotmgr, A162ClientComMgr A162commgr, mainWindow MWin)
        {
            camera3 = cam;
            hWndCtrl3 = hWnd3;
            SerailRobot = SeRobot;
            MianWin = MWin;
            PanaPLC = pana_plc;
            Myrobotmgr = robotmgr;
            MyA162comMgr = A162commgr;

            robotSeq = new WorkerThread("RobotSeq ", MWin);

            Myrobotmgr.TcpserList[0].EventCommu += Operition;
        }


        public bool RobotCaBusy = false;//机器人相机是否忙碌

        HTuple AX = new HTuple();//通道1的机械手X坐标
        HTuple AC = new HTuple();//通道1的图像Cloumn坐标
        HTuple R1 = new HTuple();//通道1的角度

        HTuple AY = new HTuple();//通道1的机械手Y坐标
        HTuple AR = new HTuple();//通道1的图像Row坐标

        HTuple BX = new HTuple();//通道2的机械手X坐标
        HTuple BC = new HTuple();//通道2的图像Cloumn坐标
        HTuple R2 = new HTuple();//通道2的角度

        HTuple BY = new HTuple();//通道2的机械手Y坐标
        HTuple BR = new HTuple();//通道2的图像Row坐标


        private bool BiaoDingCamera(HTuple Colunm,HTuple Row,HTuple R,int xi, string strpath)
        {

            for (int c = 0; c < 3; c++)
            {
                if (c == 0)
                    camera3.SetExposure(Para.RobotExposeTime);
                if (c == 1)
                    camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 0.2)));
                if (c == 2)
                    camera3.SetExposure(Para.RobotExposeTime - Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 0.2)));

                bool GrabPass = false;
                for (int j = 0; j < 3; j++)
                {
                    if (camera3.Grab())
                    {
                        GrabPass = true;
                        break;
                    }
                    Application.DoEvents();
                }
                if (!GrabPass)
                {
                    return false;
                }
                RobotData RDate = hWndCtrl3.RobotInspect(camera3.myImage, strpath);
                if (RDate.Found)
                {
                    Colunm[xi] = RDate.CenterX;
                    Row[xi]= RDate.CenterY;
                    R[xi] = RDate.CenterAngle;
                    return true;
                }
            }
            Colunm = 0;
            Row = 0;
            return false;
        }

      
        public void begin()
        {

        }
        public void stop()
        {
            for (int i = 0; i < 3; i++)
            {
                PanaPLC.PanaList[0].Write_Single_R(71, 1);
                Thread.Sleep(100);
                if (PanaPLC.PanaList[0].Read_Single_R(71) == 0)
                {
                    PanaPLC.PanaList[0].Write_Single_R(71, 1);
                    break;
                }
                else
                    return;
            }
            MessageBox.Show("停止失败");
        }

        public void BiaoDingStop()
        {
            HTuple Ahom_mat = new HTuple();
            HOperatorSet.VectorToHomMat2d(AC, AR, AX, AY, out Ahom_mat);
            HOperatorSet.WriteTuple(Ahom_mat, Para.RobotCurrentClass + "\\Mod1" + "\\BiaoDing1.tup");

            HTuple Bhom_mat = new HTuple();
            HOperatorSet.VectorToHomMat2d(BC, BR, BX, BY, out Bhom_mat);
            HOperatorSet.WriteTuple(Bhom_mat, Para.RobotCurrentClass + "\\Mod2" + "\\BiaoDing2.tup");
        }

        public int Operition(string mess)
        {
            int res = 0;
            try
            {
                string strr = mess.Replace("\r", string.Empty);//SerailRobot.SerailComList[0].ReceiveOK();
                string[] strrlist = strr.Split(';');
                string setstrRo = "";
                switch (strrlist[0])
                {
                    #region 抓料报警
                    case "PutGetCGNG1"://取料NG
                        setstrRo = "PutGetCGNG1";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        PanaPLC.PanaList[0].Write_Single_R(503, 1);//有抓料失败
                        MianWin.WriteOperationinformation("放料位左爪掉料");
                        break;
                    case "PutGetCGNG2"://取料NG
                        setstrRo = "PutGetCGNG1";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        PanaPLC.PanaList[0].Write_Single_R(503, 1);//有抓料失败
                        MianWin.WriteOperationinformation("放料位右爪掉料");
                        break;
                    case "GetGetCGNG1"://取料NG
                        setstrRo = "PutGetCGNG1";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        PanaPLC.PanaList[0].Write_Single_R(503, 1);//有抓料失败
                        MianWin.WriteOperationinformation("取料位左爪掉料");
                        break;
                    case "GetGetCGNG2"://取料NG
                        setstrRo = "GetGetCGNG2";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        PanaPLC.PanaList[0].Write_Single_R(503, 1);//有抓料失败
                        MianWin.WriteOperationinformation("取料位右爪掉料");
                        break;
                    #endregion
                    case "ReNGCGstu"://通过PLC查询是否有需要复测试的料
                        short PLCres = 0;//PanaPLC.PanaList[0].Read_Single_DT(20);
                        if (PLCres != 555)
                        {
                            setstrRo = PLCres.ToString();
                            MianWin.disp_ReCG(PLCres.ToString());
                        }
                        else
                            setstrRo = "NG";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    #region 相机拍照
                    case "TakePhone1"://照相
                        MianWin.WriteOperationinformation("TakePhone1");
                        if (SerailRobot.SerailComList[0].setLightSource_mes("1,255,2,255"))//1016
                        {
                            for (int c = 0; c < 3; c++)
                            {
                                bool GrabPass = false;
                                for (int j = 0; j < 3; j++)
                                {
                                    if (j == 0)
                                        camera3.SetExposure(Para.RobotExposeTime);
                                    if (j == 1)
                                        camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 0.5)));
                                    if (j == 2)
                                        camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 1.2)));

                                    if (camera3.Grab())
                                    {
                                        GrabPass = true;
                                        break;
                                    }
                                    Application.DoEvents();
                                }
                                if (!GrabPass)
                                {

                                }
                                RobotData RDate = hWndCtrl3.Inspect(camera3.myImage, hWndCtrl3.RobotMod[0], Para.RobotCurrentClass + "\\Mod1");
                                if (RDate.Found)
                                {
                                    HTuple hv_ReTuple = new HTuple();
                                    HOperatorSet.ReadTuple(Para.RobotCurrentClass + "\\Mod1" + "\\BiaoDing1.tup", out hv_ReTuple);

                                    HTuple xx, yy;
                                    HOperatorSet.AffineTransPoint2d(hv_ReTuple, RDate.CenterX, RDate.CenterY, out xx, out yy);

                                    double subR = hWndCtrl3.RobotMod[0].CenterAngle - RDate.CenterAngle;
                                    double subX = hWndCtrl3.RobotMod[0].CenterX - ((xx + 68.640) * Math.Cos(subR * 3.1415926 / 180) - (yy - 476.826) * Math.Sin(subR * 3.1415926 / 180) - 68.640);
                                    double subY = hWndCtrl3.RobotMod[0].CenterY - ((xx + 68.640) * Math.Sin(subR * 3.1415926 / 180) + (yy - 476.826) * Math.Cos(subR * 3.1415926 / 180) + 476.826);
                                    
                                    if (Math.Abs(subX) < 30 && Math.Abs(subX) < 30 && Math.Abs(subR) < 10)
                                    {
                                        setstrRo = subX.ToString("F3") + ";" + subY.ToString("F3") + ";" + subR.ToString("F3");
                                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                                        return 0;
                                    }
                                }
                            }
                        }
                        setstrRo = "TakePhoneNG";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        MianWin.WriteOperationinformation("机械手工位1拍照失败");
                        break;
                    case "TakePhone2"://照相
                        MianWin.WriteOperationinformation("TakePhone2");
                        for (int c = 0; c < 3; c++)
                        {
                            if (c == 0)
                                camera3.SetExposure(Para.RobotExposeTime);
                            if (c == 1)
                                camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 0.5)));
                            if (c == 2)
                                camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 1.2)));

                            bool GrabPass = false;
                            for (int j = 0; j < 3; j++)
                            {
                                if (camera3.Grab())
                                {
                                    GrabPass = true;
                                    break;
                                }
                                Application.DoEvents();
                            }
                            if (!GrabPass)
                            {

                            }
                            RobotData RDate = hWndCtrl3.Inspect(camera3.myImage, hWndCtrl3.RobotMod[0], Para.RobotCurrentClass + "\\Mod2");
                            if (RDate.Found)
                            {
                                HTuple hv_ReTuple = new HTuple();
                                HOperatorSet.ReadTuple(Para.RobotCurrentClass + "\\Mod2" + "\\BiaoDing2.tup", out hv_ReTuple);

                                HTuple xx, yy;
                                HOperatorSet.AffineTransPoint2d(hv_ReTuple, RDate.CenterX, RDate.CenterY, out xx, out yy);

                                //double subX = hWndCtrl3.RobotMod[1].CenterX - xx;
                                //double subY = hWndCtrl3.RobotMod[1].CenterY - yy;
                                double subR = hWndCtrl3.RobotMod[1].CenterAngle - RDate.CenterAngle;
                                double subX = hWndCtrl3.RobotMod[1].CenterX - ((xx + 69.670) * Math.Cos(subR * 3.1415926 / 180) - (yy - 476.905) * Math.Sin(subR * 3.1415926 / 180) - 69.670);
                                double subY = hWndCtrl3.RobotMod[1].CenterY - ((xx + 69.670) * Math.Sin(subR * 3.1415926 / 180) + (yy - 476.905) * Math.Cos(subR * 3.1415926 / 180) + 476.905);
                                    

                                if (Math.Abs(subX) < 30 && Math.Abs(subY) < 30 && Math.Abs(subR) < 10)
                                {

                                    setstrRo = subX.ToString("F3") + ";" + subY.ToString("F3") + ";" + subR.ToString("F3");
                                    Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                                    if (strrlist[1] == "2")
                                    {
                                        if (SerailRobot.SerailComList[0].setLightSource_mes("1,0,2,0"))//1016
                                        {
                                            //break;//1016
                                        }
                                    }

                                    return 0; ;
                                }
                            }
                        }
                        SerailRobot.SerailComList[0].setLightSource_mes("1,0,2,0");
                        setstrRo = "TakePhoneNG";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        //res = -1;
                        MianWin.WriteOperationinformation("机械手工位2拍照失败");
                        break;
                    #endregion
                    #region 机械手与A162机台通讯部分
                    case "compelet"://上之前与A162交换产品信息
                        if (strrlist[3] == "end")
                        {
                            string restrr = MyA162comMgr.A162ClientComList[0].Read("compelet;" + strrlist[1] + ";" + strrlist[2] + ";end");
                            if (restrr == "ConnectNG")
                            {
                                int ci = 0;
                                while (!MyA162comMgr.A162ClientComList[0].Connect(MyA162comMgr.A162ClientComList[0].IP,MyA162comMgr.A162ClientComList[0].Port))
                                {
                                    if (ci > 2)
                                    {
                                        Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                                        MianWin.WriteOperationinformation("网线连接异常");
                                        PanaPLC.PanaList[0].Write_Single_R(502, 0);//PC未启动
                                        Thread.Sleep(300);
                                        PanaPLC.PanaList[0].Write_Single_R(502, 1);
                                        break;
                                    }  
                                    ci++;
                                }
                            }
                            if (restrr == "A162Stopping")
                            {
                                Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                                MianWin.WriteOperationinformation("D3X未启动");
                                PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                                Thread.Sleep(500);
                                PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                                break;
                            }
                            string[] strlist = restrr.Split(';');//将A162发回来的数据进行处理
                            if (strlist.Length == 5)
                            {
                                if (strlist[1] == "3" || strlist[2] == "3" || strlist[1] == "4" || strlist[2] == "4" ||
                                    strlist[1] == "5" || strlist[2] == "5" || strlist[1] == "6" || strlist[2] == "6")
                                {
                                    PanaPLC.PanaList[0].Write_Single_R(501, 1);//有二次NG料
                                    if (PanaPLC.PanaList[0].Read_Single_R(501) == 0)
                                    {
                                        PanaPLC.PanaList[0].Write_Single_R(501, 1);//有二次NG料
                                    }
                                }
                                MianWin.WriteOperationinformation("Mod1:" + strlist[1] + "；Mod2:" + strlist[2]);
                            }
                            setstrRo = restrr;
                            Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        }
                        res = 0;
                        break;
                    case "A162Begin"://当所有的料都放置OK以后，开始启动A162机台
                        string rrstr = MyA162comMgr.A162ClientComList[0].Read("start;end");
                        if (rrstr == "ConnectNG")
                        {
                            int ci = 0;
                            while (!MyA162comMgr.A162ClientComList[0].Connect(MyA162comMgr.A162ClientComList[0].IP, MyA162comMgr.A162ClientComList[0].Port))
                            {
                                if (ci > 2)
                                {
                                    Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                                    MianWin.WriteOperationinformation("网线连接异常");
                                    PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                                    Thread.Sleep(300);
                                    PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                                    break;
                                }  
                                ci++;
                            }
                        }
                        if (rrstr == "A162Stopping")
                        {
                            Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                            MianWin.WriteOperationinformation("D3X未启动");
                            PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                            Thread.Sleep(500);
                            PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                            break;
                        }
                        string[] rrstrlist = rrstr.Split(';');//将A162发回来的数据进行处理
                        if (rrstrlist[0] == "started" && rrstrlist[1] == "end")
                        {
                            setstrRo = "A162Begin";
                            MianWin.WriteOperationinformation("启动D3X机台");
                        }
                        else
                        {
                            setstrRo = "A162BeginNG";
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    case "A162CurrectMod"://查询当前A162工位是否有料工位是否
                        //1表示当前工位1有料，2表示当前工位2有料，3表示当前两个工位都有料
                        string rrrstr = MyA162comMgr.A162ClientComList[0].Read("A162CurrectMod;end");
                        if (rrrstr == "ConnectNG")
                        {
                            int ci = 0;
                            while (!MyA162comMgr.A162ClientComList[0].Connect(MyA162comMgr.A162ClientComList[0].IP, MyA162comMgr.A162ClientComList[0].Port))
                            {
                                if (ci > 2)
                                {
                                    Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                                    MianWin.WriteOperationinformation("网线连接异常");
                                    PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                                    Thread.Sleep(300);
                                    PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                                    break;
                                }  
                                ci++;
                            }
                        }
                        if (rrrstr == "A162Stopping")
                        {
                            Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                            MianWin.WriteOperationinformation("D3X未启动");
                            PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                            Thread.Sleep(500);
                            PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                            break;
                        }
                        string[] rrrstrlist = rrrstr.Split(';');//将A162发回来的数据进行处理
                        if (rrrstrlist[0] == "CurrectMod" && rrrstrlist[3] == "end")
                        {
                            setstrRo = "A162CurrectMod;" + rrrstrlist[1] + ";" + rrrstrlist[2] + ";end";
                        }
                        else
                        {
                            setstrRo = rrrstr;
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    case "A162EndLot"://读取A162机台是不是点击了EndLot
                        string rrsstr = MyA162comMgr.A162ClientComList[0].Read("EndLot;end");
                        if (rrsstr == "ConnectNG")
                        {
                            int ci = 0;
                            while (!MyA162comMgr.A162ClientComList[0].Connect(MyA162comMgr.A162ClientComList[0].IP, MyA162comMgr.A162ClientComList[0].Port))
                            {
                                if (ci > 2)
                                {
                                    Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                                    MianWin.WriteOperationinformation("D3X设备未启动或网线连接异常");
                                    PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                                    Thread.Sleep(300);
                                    PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                                    break;
                                }  
                                ci++;
                            }
                        }
                        if (rrsstr == "A162Stopping")
                        {
                            Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                            MianWin.WriteOperationinformation("D3X未启动");
                            PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                            Thread.Sleep(500);
                            PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                            break;
                        }
                        string[] rrsstrlist = rrsstr.Split(';');//将A162发回来的数据进行处理
                        if (rrsstrlist[0] == "EndLot" && rrsstrlist[2] == "end")
                        {
                            if (bool.Parse(rrsstrlist[1]))
                            {
                                Para.EndLot = true;
                                MianWin.Dis_EndLot();
                            }
                            else
                            {
                                Para.EndLot = false;
                                MianWin.Dis_EndLot();
                            }
                                
                            setstrRo = "A162EndLot;" + ";" + rrsstrlist[1] + ";" + rrsstrlist[2];
                        }
                        else
                        {
                            setstrRo = "A162EndLotNG";
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    case "AbleEndLot"://点击A162的EndLot
                        if (Para.EndLot)
                        {
                            string rrrsstr = MyA162comMgr.A162ClientComList[0].Read("AbleEndLot;" + Para.EndLot + ";end");
                            if (rrrsstr == "ConnectNG")
                            {
                                int ci = 0;
                                while (!MyA162comMgr.A162ClientComList[0].Connect(MyA162comMgr.A162ClientComList[0].IP, MyA162comMgr.A162ClientComList[0].Port))
                                {
                                    if (ci > 2)
                                    {
                                        Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                                        MianWin.WriteOperationinformation("网线连接异常");
                                        PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                                        Thread.Sleep(300);
                                        PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                                        break;
                                    }  
                                    ci++;
                                }
                            }
                            if (rrrsstr == "A162Stopping")
                            {
                                Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                                MianWin.WriteOperationinformation("D3X未启动");
                                PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                                Thread.Sleep(500);
                                PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                                break;
                            }
                            string[] rrrsstrlist = rrrsstr.Split(';');//将A162发回来的数据进行处理
                            if (rrrsstrlist[0] == "AbleEndLot" && rrrsstrlist[2] == "end")
                            {
                                setstrRo = rrrsstr;
                                Para.EndLot = bool.Parse(rrrsstrlist[1]);
                                MianWin.Dis_EndLot();//更新EndLot按钮状态
                            }
                            else
                            {
                                setstrRo = "EndLotNG";
                            }

                        }
                        else
                        {
                            setstrRo = "UnableEndLot";
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    case "Vacuum"://A162当前工位吸气
                        string rrrssstr = MyA162comMgr.A162ClientComList[0].Read(strr);
                        if (rrrssstr == "ConnectNG")
                        {
                            int ci = 0;
                            while (!MyA162comMgr.A162ClientComList[0].Connect(MyA162comMgr.A162ClientComList[0].IP, MyA162comMgr.A162ClientComList[0].Port))
                            {
                                if (ci > 2)
                                {
                                    Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                                    MianWin.WriteOperationinformation("网线连接异常");
                                    PanaPLC.PanaList[0].Write_Single_R(502, 0);
                                    Thread.Sleep(300);
                                    PanaPLC.PanaList[0].Write_Single_R(502, 1);
                                    break;
                                }
                                ci++;
                            }
                        }
                        if (rrrssstr == "A162Stopping")
                        {
                            Myrobotmgr.TcpserList[0].SendMessage("A162Stopping");
                            MianWin.WriteOperationinformation("D3X未启动");
                            PanaPLC.PanaList[0].Write_Single_R(502, 0);//气缸伸缩超时报警
                            Thread.Sleep(500);
                            PanaPLC.PanaList[0].Write_Single_R(502, 1);//气缸伸缩超时报警
                            break;
                        }
                        string[] rrrssstrlist = rrrssstr.Split(';');//将A162发回来的数据进行处理
                        if (rrrssstrlist[0] == "Vacuum" && rrrssstrlist[2] == "end")
                        {
                            setstrRo = rrrssstr;
                        }
                        else
                        {
                            setstrRo = "VacuumNG";
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    #endregion
                    case "GetGetCGNG":
                        setstrRo = "GetGetCGNG";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        res = 0;
                        break;
                    
                    #region 气缸超时报警内容
                    case "PutModAirNG":

                        setstrRo = "PutModAirNG";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        PanaPLC.PanaList[0].Write_Single_R(504, 1);//气缸伸缩超时报警
                        MianWin.WriteOperationinformation("放料气缸报警");
                        res = 0;
                        break;
                    case "GetModAirNG":

                        setstrRo = "GetModAirNG";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        PanaPLC.PanaList[0].Write_Single_R(504, 1);//气缸伸缩超时报警
                        MianWin.WriteOperationinformation("抓料气缸报警");
                        res = 0;
                        break;
                    #endregion
                    #region 机械手放料记录
                    case "ReadCount":
                        if (PanaPLC.PanaList[0].Read_Serial_Data(32606, 32612))//读取7个坑的数据
                        {
                            string setstr = "ReadCount";
                            foreach (int dt in PanaPLC.PanaList[0].Serial_Data)
                            {
                                setstr += ";" + dt.ToString();
                            }
                            setstrRo = setstr;
                            Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                            MianWin.disp_count(PanaPLC.PanaList[0].Serial_Data);//显示数量
                        }
                        else
                        {
                            setstrRo = "ReadCountNG";
                            Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        }
                        break;
                    case "WriteCount":
                        List<int> DT = new List<int>();
                        for (int i = 1; i < 8; i++)
                        {
                            DT.Add(int.Parse(strrlist[i]));
                        }
                        if (PanaPLC.PanaList[0].Write_Serial_Data(32606, 32612, DT))//写入7个坑的数据
                        {
                            setstrRo = "WriteCount";
                        }
                        else
                        {
                            setstrRo = "WriteCountNG";
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        MianWin.disp_count(DT);//显示数量
                        break;
                    #endregion

                    #region 所有料放置状态
                    case "OKTrayOK":                      
                        if (PanaPLC.PanaList[0].Write_Single_R(507, 1))//OK料盘放满，需要取料了
                        {
                            setstrRo = "OKTrayOK";
                            MianWin.WriteOperationinformation("料盘放满通知取料");
                        }
                        else
                        {
                            setstrRo = "NG";   
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    case "ALLOKTrayCount":
                        if (0 == PanaPLC.PanaList[0].Read_Single_DT(32610))//只读OK料盘数据是否放满
                        {
                            setstrRo = "ALLOKTrayCount";
                        }
                        else
                        {
                            setstrRo = "NG";   
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    #endregion
                    #region 回原状态
                    case "RobotOriginOK":
                        PanaPLC.PanaList[0].Write_Single_R(500, 1);//通知PLC机械手回原完成
                        if (PanaPLC.PanaList[0].Read_Single_R(500) == 0)
                        {
                            PanaPLC.PanaList[0].Write_Single_R(500, 1);//通知PLC机械手回原完成

                            setstrRo = "NG";
                            Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        }
                        else
                        {
                            MianWin.WriteOperationinformation("机械手回原完成");
                            setstrRo = "RobotOriginOK";
                            Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        }
                        break;
                    case "RobotOriginNG":
                        PanaPLC.PanaList[0].Write_Single_R(500, 0);//通知PLC机械手回原完成
                        if (PanaPLC.PanaList[0].Read_Single_R(500) == 1)
                        {
                            PanaPLC.PanaList[0].Write_Single_R(500, 0);//通知PLC机械手回原完成

                            setstrRo = "NG";
                            Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        }
                        else
                        {

                            setstrRo = "RobotOriginNG";
                            Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        }
                        break;
                    #endregion
                    #region 夹紧平台动作
                    case "jiajinOpen":
                        PanaPLC.PanaList[0].Write_Single_R(600, 0);
                        Thread.Sleep(50);
                        while (PanaPLC.PanaList[0].Read_Single_R(601) == 0)
                        {
                            Thread.Sleep(50);
                        }
                        setstrRo = "jiajinOpen";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        //switch (strrlist[1])
                        //{ 
                        //    case "1":
                        //        PanaPLC.PanaList[0].Write_Single_R(600, 0);
                        //        Thread.Sleep(50);
                        //        while (PanaPLC.PanaList[0].Read_Single_R(603)==1)
                        //        { }
                        //        setstrRo = "jiajinOpen;1";
                        //        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        //        break;
                        //    case"2":
                        //        PanaPLC.PanaList[0].Write_Single_R(601, 0);
                        //        Thread.Sleep(50);
                        //        while (PanaPLC.PanaList[0].Read_Single_R(604)==1)
                        //        { }
                        //        setstrRo = "jiajinOpen;2";
                        //        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        //        break;
                        //    case"3":
                        //        PanaPLC.PanaList[0].Write_Single_R(602, 0);
                        //        Thread.Sleep(50);
                        //        while (PanaPLC.PanaList[0].Read_Single_R(605)==1)
                        //        { }
                        //        setstrRo = "jiajinOpen;3";
                        //        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        //        break;
                        //}
                        break;
                    case"jiajinClose":
                        PanaPLC.PanaList[0].Write_Single_R(600, 1);
                        Thread.Sleep(50);
                        while (PanaPLC.PanaList[0].Read_Single_R(601) == 1)
                        {
                            Thread.Sleep(50);
                        }
                        setstrRo = "jiajinClose";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        //switch (strrlist[1])
                        //{
                        //    case "1":
                        //        PanaPLC.PanaList[0].Write_Single_R(600, 1);
                        //        Thread.Sleep(50);
                        //        while (PanaPLC.PanaList[0].Read_Single_R(603) == 0)
                        //        { }
                        //        setstrRo = "jiajinClose;1";
                        //        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        //        break;
                        //    case "2":
                        //        PanaPLC.PanaList[0].Write_Single_R(601, 1);
                        //        Thread.Sleep(50);
                        //        while (PanaPLC.PanaList[0].Read_Single_R(604) == 0)
                        //        { }
                        //        setstrRo = "jiajinClose;2";
                        //        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        //        break;
                        //    case "3":
                        //        PanaPLC.PanaList[0].Write_Single_R(602, 1);
                        //        Thread.Sleep(50);
                        //        while (PanaPLC.PanaList[0].Read_Single_R(605) == 0)
                        //        { }
                        //        setstrRo = "jiajinClose;3";
                        //        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        //        break;
                        //}
                        break;
                    #endregion
                    case"NGTrayHome"://放OK料时,NG料盘是否已经退出
                        if (PanaPLC.PanaList[0].Read_Single_R(607) == 1)
                            setstrRo = "NGTrayHome";
                        else
                        {
                            setstrRo = "NGTrayHomeNG";
                            Thread.Sleep(100);
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo); 
                        break;
                    case "NGTrayGo"://NG料盘走
                        PanaPLC.PanaList[0].Write_Single_R(501, 0);
                        if (PanaPLC.PanaList[0].Read_Single_R(501) == 1)
                        {
                            PanaPLC.PanaList[0].Write_Single_R(501, 0);
                            setstrRo = "NG";
                            Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        }
                        else
                        {
                            setstrRo = "NGTrayGo";
                            Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        }
                        break;
                    case"NGTrayCome"://NG料盘过来，需要用于开机运行的时候
                        if (PanaPLC.PanaList[0].Write_Single_R(501, 1))
                        {
                            setstrRo = "NGTrayCome";
                        }
                        else
                        {
                            setstrRo = "NG";
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    case "ReCGInitNG"://返回值是小于0的说明机械手爪的料的复测平台料有冲突，必须将平台料取走
                        MianWin.WriteOperationinformation("复测平台料冲突，请将复测平台料取走，机械手重新回原");
                        setstrRo = "ReCGInitNG";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    case"RobotSpeed"://读取机械手的速度
                        setstrRo = "RobotSpeed;" + Myrobotmgr.TcpserList[0].Power + ";" + Myrobotmgr.TcpserList[0].Speed + ";" + Myrobotmgr.TcpserList[0].Accel;
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    #region 九点标定
                    case "BiaoDing":
                        for (int ji = 0; ji < 10; ji++)
                        {
                            if (strrlist[1] == "A" + ji.ToString())
                            {
                                if (BiaoDingCamera(AC, AR, R1, ji - 1, Para.RobotCurrentClass + "\\Mod1"))
                                {
                                    AX[ji - 1] = double.Parse(strrlist[2]);
                                    AY[ji - 1] = double.Parse(strrlist[3]);

                                    Myrobotmgr.TcpserList[0].SendMessage("A" + ji.ToString());
                                    MianWin.WriteOperationinformation("标定1工位:" + ji + "点");
                                }
                                else
                                    Myrobotmgr.TcpserList[0].SendMessage("BiaodingNG");
                                break;
                            }
                            if (strrlist[1] == "B" + ji.ToString())
                            {
                                if(BiaoDingCamera(BC, BR, R2, ji - 1, Para.RobotCurrentClass + "\\Mod2"))
                                {
                                    BX[ji - 1] = double.Parse(strrlist[2]);
                                    BY[ji - 1] = double.Parse(strrlist[3]);

                                    Myrobotmgr.TcpserList[0].SendMessage("B" + ji.ToString());
                                    MianWin.WriteOperationinformation("标定2工位:" + ji + "点");
                                }
                                else
                                    Myrobotmgr.TcpserList[0].SendMessage("BiaodingNG");
                                break;
                            }
                        }
                        break;
                    case "BiaodingOK"://标定成功
                        BiaoDingStop();
                        Myrobotmgr.TcpserList[0].SendMessage("BiaodingOK");
                        PanaPLC.PanaList[0].Write_Single_R(505, 1);//标定成功通知PLC
                        MianWin.WriteOperationinformation("标定成功");
                        break;
                    case "BiaodingNG"://标定失败 
                        //删除标定数据
                        Myrobotmgr.TcpserList[0].SendMessage("BiaodingNG");
                        PanaPLC.PanaList[0].Write_Single_R(506, 1);//标定失败通知PLC
                        MianWin.WriteOperationinformation("标定失败");
                        break;
                    #endregion
                    case"GRR"://机械手跑GRR
                        if (Para.GRRBool)
                        {
                            MianWin.WriteOperationinformation("GRR测试开始");
                            setstrRo = "GRR;4;4;4;37;0";
                            //上料次数；交换次数；下料次数；连续次数；备用
                        }
                        else
                            setstrRo = "GRRNO";
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    case"TestMessage"://提示
                        Para.RobotTestMessage = strrlist[0];
                        switch (strrlist[1])
                        { 
                            case "0":
                                Para.RobotTestMessage = "请屏蔽光栅感应!!!\r\n"+"手动将测试片放置在缓存料盒处";
                                break;
                            case "1":
                                Para.RobotTestMessage = "请屏蔽光栅感应!!!\r\n" + "手动将机械手取来的测试片交换位置";
                                break;
                            case"2":
                                Para.RobotTestMessage = "GRR测试已完成!!!\r\n"+"请将测试片取下，并开启光栅感应";
                                MianWin.GRR_UnCheck();//去除GRR勾选
                                break;
                            case"3"://下料开始，点击EndLot
                                MianWin.EndLot_Check();//勾选EndLOt
                                Para.RobotTestOK = true;
                                Para.RobotTestNG = false;
                                break;
                            default:
                                break;
                        }
                        MianWin.DispTestShow();
                        if(Para.RobotTestOK)
                        {
                            setstrRo = "GRRRun";
                            Para.RobotTestOK = false;
                            Para.RobotTestNG = false;
                            MianWin.DispTestHide();
                        } 
                        else
                            setstrRo = "GRRWaitting";
                        if (Para.RobotTestNG)
                        {
                            setstrRo = "GRRPause";
                            Para.RobotTestOK = false;
                            Para.RobotTestNG = false;
                            MianWin.DispTestHide();
                        }
                        Myrobotmgr.TcpserList[0].SendMessage(setstrRo);
                        break;
                    default:
                        //SerailRobot.SerailComList[0].set_mes("NG");
                        if (strrlist[0] != "")
                        {
                            MessageBox.Show(strrlist[0]);
                        }
                        res = 0;
                        break;
                }
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
            return 0;
        }
    }
}

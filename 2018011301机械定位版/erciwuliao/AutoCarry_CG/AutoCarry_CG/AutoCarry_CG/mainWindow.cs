using A162Client;
using Common;
using HalconDotNet;
using JPTCG;
using JPTCG.Common;
using JPTCG.Vision;
using Panasonic_Communication;
using RobotCommunication;
using Serial_Port;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TcpSerive;


namespace AutoCarry_CG
{
    public partial class mainWindow : Form
    {
        public HalconVision camera3;
        public RobotVision hWndCtrl3;
        public MachineSeq SeqMgr;
        public SerailComMgr SerailMgr;
        public SerailCom SerailRobot;
        public SerailCom SerailLightSource;
        public Panasonic_PLCMgr PanaMgr;
        public TcpseriveComMgr RobotMgr;
        public A162ClientComMgr A162ComMgr;
        public UserLogin uLogin = new UserLogin();

        Thread stopThread;


        public mainWindow()
        {
            InitializeComponent();
            this.mainTC.Region = new Region(new RectangleF(this.HomeTP.Left, this.HomeTP.Top, this.HomeTP.Width, this.HomeTP.Height));//Hide Tabcontrol

            VersionLbl.Text = Para.SWVersion;

            mchSettingsFilePath = exePath + mchSetFileName;

            Para.MchConfigFileName = mchSettingsFilePath;
            camera3 = new HalconVision("robot", Mpanel1, HalconWin3);
            hWndCtrl3 = new RobotVision(HalconWin3, camera3);
            SerailMgr = new SerailComMgr();
            SerailMgr.SerailComListAdd("LightSourceCom");
            PanaMgr = new Panasonic_PLCMgr();
            PanaMgr.PanaAdd("Panan1");
            
            RobotMgr = new TcpseriveComMgr();
            RobotMgr.TcpseriveComMgr_Add("Robot1");
            RobotMgr.TcpserList[0].EventConState += UpdateRobotCountStatusLbl;

            A162ComMgr = new A162ClientComMgr();
            A162ComMgr.AddRobotCom("ToA162");
            A162ComMgr.A162ClientComList[0].OnSendAndRec += UpdateA162CountStatusLbl;

            SeqMgr = new MachineSeq(camera3, hWndCtrl3, SerailMgr, PanaMgr, RobotMgr,A162ComMgr,this);

            HalconWin3.Parent = panel1;

            camera3.OnImageReadyFunction += OnImgReadyCam3;

            
            //DateTime dtime = DateTime.Now;
            //TimeSpan tsp;
            //Thread.Sleep(1000);
            //tsp = DateTime.Now - dtime;
            //MessageBox.Show(tsp.TotalMilliseconds.ToString());
        }



        private void mainWindow_Load(object sender, EventArgs e)
        {
            PanaMgr.PanaList_Load();

            RobotClassPath = exePath + "\\RobotClass";

            Load_RobotClass(RobotClassPath);
            Para.RobotCurrentClass = RobotClassPath;


            Load_RobotClass();

            camera3.LoadSettings(mchSettingsFilePath);
            hWndCtrl3.Load(mchSettingsFilePath);
            Load_Exposerobot(mchSettingsFilePath);
            SerailMgr.SerailComList_Load();
            A162ComMgr.LoadSettings(mchSettingsFilePath);

            RobotMgr.TcpseriveComMgr_Load(mchSettingsFilePath);

            UpdateStatusLbl(Color.Orange);

            userCB.Items.Clear();
            for (int i = 0; i < uLogin.user.Count(); i++)
                userCB.Items.Add(uLogin.user[i]);

            userCB.SelectedIndex = 0;
            SetUIAccess(0);

            stopThread = new Thread(Errorstop);
            stopThread.IsBackground = true;
            //stopThread.Start();

            disp_speed();//机械手的速度的设定
            
            if(PanaMgr.PanaList[0].Write_Single_R(502,1))//软件是否启动)
            {
                if(PanaMgr.PanaList[0].Read_Single_R(502) == 0)
                    PanaMgr.PanaList[0].Write_Single_R(502,1);
            }
        }

        private void hWindowControl3_HMouseMove(object sender, HalconDotNet.HMouseEventArgs e)
        {
            HTuple Window = new HTuple();
            Window = HalconWin3.HalconWindow;
            HTuple Row1, Col1, Button;
            try
            {
                HOperatorSet.GetMposition(Window, out Row1, out Col1, out Button);
                MStatusLblX.Text = Col1.ToString();
                MStatusLblY.Text = Row1.ToString();
            }
            catch
            {
            }
        }

        String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
        string mchSetFileName = "JPT_CG_Settings.xml";
        string mchSettingsFilePath;
        string RobotClassPath;
        private void MAssignCamBtn_Click(object sender, EventArgs e)
        {
            CameraSelectionWin camSelWin = new CameraSelectionWin(camera3);
            camSelWin.ShowDialog();
            McamIDLbl.Text = camera3.cameraID;
            camera3.SaveSettings(mchSettingsFilePath);

        }

        private void MLoadImgBtn_Click(object sender, EventArgs e)
        {
            string strHeadImagePath;
            HImage image;

            OpenImageDialog.Title = "Open Image file";
            OpenImageDialog.ShowHelp = true;
            OpenImageDialog.Filter = "(*.gif)|*.gif|(*.jpg)|*.jpg|(*.JPEG)|*.JPEG|(*.bmp)|*.bmp|(*.png)|*.png|All files (*.*)|*.*";
            DialogResult result = OpenImageDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    strHeadImagePath = OpenImageDialog.FileName;
                    image = new HImage(strHeadImagePath);
                    camera3.myImage = image;
                }
                catch
                {
                    MessageBox.Show("format not correct");
                }
            }

        }

        private void Mbutton11_Click(object sender, EventArgs e)
        {
            if (camera3.IsCameraOpen())
            {
                camera3.StopCamera();
                //EnbToolBtn(true);
                McamStatusLbl.Text = "Idle...";
                McamStatusLbl.BackColor = Color.Orange;
            }
            camera3.Grab();
        }

        private void Continu_Click(object sender, EventArgs e)
        {
            if (camera3.IsCameraOpen())
            {

                camera3.LiveCamera();
                //EnbToolBtn(false);
                McamStatusLbl.Text = "Live...";
                McamStatusLbl.BackColor = Color.Lime;
            }
        }

        private void MCam1SaveImgCB_CheckedChanged(object sender, EventArgs e)
        {
            camera3.bSaveImage = MCam1SaveImgCB.Checked;
        }

        
        private void OnImgReadyCam3(HObject myImage)
        {
            Action ac = new Action(() =>
            {
                hWndCtrl3.disp_HImage(myImage);

                camera3.bUIRefreshDone = true;
                Application.DoEvents();
            });
            BeginInvoke(ac);
        }

        private void mainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PanaMgr.PanaList[0].Write_Single_R(502, 0))//软件是否启动)
            {
                if (PanaMgr.PanaList[0].Read_Single_R(502) == 1)
                    PanaMgr.PanaList[0].Write_Single_R(502, 0);
            }

            SerailMgr.SerailComList[0].setLightSource_mes("1,0,2,0");
            camera3.StopCamera();
            Application.DoEvents();
            Thread.Sleep(700);
            camera3.CloseCamera();

            if (stopThread != null && stopThread.IsAlive == true)
                stopThread.Abort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (camera3.IsCameraOpen())
            {
                camera3.StopCamera();
                //EnbToolBtn(true);
                McamStatusLbl.Text = "Idle...";
                McamStatusLbl.BackColor = Color.Orange;
            }


            for (int c = 0; c < 3; c++)
            {
                if (c == 0)
                    camera3.SetExposure(Para.RobotExposeTime);
                if (c == 1)
                    camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 0.5)));
                if (c == 2)
                    camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 1)));

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
                    return;
                }
                RobotData RDate = hWndCtrl3.RobotInspect(camera3.myImage, RobotClassPath + "\\Mod1");
                if (RDate.Found)
                {
                    HTuple hv_ReTuple = new HTuple();
                    HOperatorSet.ReadTuple(Para.RobotCurrentClass + "\\Mod1" + "\\BiaoDing1.tup", out hv_ReTuple);

                    HTuple xx, yy;
                    HOperatorSet.AffineTransPoint2d(hv_ReTuple, RDate.CenterX, RDate.CenterY, out xx, out yy);

                    MessageBox.Show(xx.ToString() + ";" + yy.ToString() + ";" + RDate.CenterAngle);
                    break;
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (camera3.IsCameraOpen())
            {
                camera3.StopCamera();
                //EnbToolBtn(true);
                McamStatusLbl.Text = "Idle...";
                McamStatusLbl.BackColor = Color.Orange;
            }

            for (int c = 0; c < 3; c++)
            {
                if (c == 0)
                    camera3.SetExposure(Para.RobotExposeTime);
                if (c == 1)
                    camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 0.5)));
                if (c == 2)
                    camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 1)));

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
                    return;
                }
                RobotData RDate = hWndCtrl3.RobotInspect(camera3.myImage, RobotClassPath + "\\Mod2");
                if (RDate.Found)
                {
                    HTuple hv_ReTuple = new HTuple();
                    HOperatorSet.ReadTuple(Para.RobotCurrentClass + "\\Mod2" + "\\BiaoDing2.tup", out hv_ReTuple);

                    HTuple xx, yy;
                    HOperatorSet.AffineTransPoint2d(hv_ReTuple, RDate.CenterX, RDate.CenterY, out xx, out yy);

                    MessageBox.Show(xx.ToString() + ";" + yy.ToString() + ";" + RDate.CenterAngle);
                    break;
                }
            }
        }

        private void CreateMod1_Click(object sender, EventArgs e)
        {
            if (camera3.IsCameraOpen())
            {
                camera3.StopCamera();
                //EnbToolBtn(true);
                McamStatusLbl.Text = "Idle...";
                McamStatusLbl.BackColor = Color.Orange;
            }
            Thread.Sleep(100);

            if (camera3.myImage != null)
            {
                MessageBox.Show("是否为工位1创建模板");
                if (hWndCtrl3.CreatMod(camera3.myImage, RobotClassPath + "\\Mod1"))
                {
                    MessageBox.Show("工位1模板创建成功");
                }
            }
            else
            {
                MessageBox.Show("工位1没有照片采集");
            }
        }

        private void CreateMod2_Click(object sender, EventArgs e)
        {
            if (camera3.IsCameraOpen())
            {
                camera3.StopCamera();
                //EnbToolBtn(true);
                McamStatusLbl.Text = "Idle...";
                McamStatusLbl.BackColor = Color.Orange;
            }
            Thread.Sleep(100);

            if (camera3.myImage != null)
            {
                MessageBox.Show("是否为工位2创建模板");
                if (hWndCtrl3.CreatMod(camera3.myImage, RobotClassPath + "\\Mod2"))
                {
                    MessageBox.Show("工位2模板创建成功");
                }
            }
            else
            {
                MessageBox.Show("工位2没有照片采集");
            }
        }


        public bool fangliaobool = false;

        public string mod1stu = "0";
        public string mod2stu = "0";

        public bool res1ok = false;
        public bool res2ok = false;

        public string toA162m1 = "0";
        public string toA162m2 = "0";

        public bool erNG = true;

        private void timer2_Tick(object sender, EventArgs e)
        {
           

        }

        //标定开始
        private void button9_Click(object sender, EventArgs e)
        {
            
        }

        public void BiaodingSave()
        {
            hWndCtrl3.Save(mchSettingsFilePath);//保存标定的结果
            MessageBox.Show("标定结果保存OK");
        }

        private void SaveExpose_Click(object sender, EventArgs e)
        {
            camera3.SetExposure(int.Parse(_RoExpose.Text));
            Para.RobotExposeTime = int.Parse(_RoExpose.Text);
            SaveSettings(mchSettingsFilePath);
        }

        public void Load_Exposerobot(string fileName)
        {
            string strread = "";
            string headerStr = "Cam_" + Name.Replace(" ", string.Empty);
            FileOperation.ReadData(fileName, headerStr, "RobotExpose", ref strread);
            if (strread != "0")
            {
                camera3.SetExposure(int.Parse(strread));
                _RoExpose.Text = strread;
                Para.RobotExposeTime = int.Parse(strread);
            }
        }

        public void SaveSettings(string fileName)
        {
            string headerStr = "Cam_" + Name.Replace(" ", string.Empty);
            FileOperation.SaveData(fileName, headerStr, "RobotExpose", _RoExpose.Text);  
        }


        public List<string> RobotClass = new List<string>();
        public void Load_RobotClass()//加载所有机器人的模板
        {
            RobotClass.Clear();
            if (!Directory.Exists(RobotClassPath))
            {
                Directory.CreateDirectory(RobotClassPath);
            }
            else
            {
                foreach (string cla in Directory.GetDirectories(RobotClassPath))
                {
                    RobotClass.Add(cla);
                }
            }
        }

        public void Load_RobotClass(string fileName)
        {
            string strread = "";
            string headerStr = "Cam_" + Name.Replace(" ", string.Empty);
            FileOperation.ReadData(fileName, headerStr, "RobotClass", ref strread);
            if (strread != "0")
            {
                Para.RobotCurrentClass = strread;
            }
        }

        public void Save_RobotClass(string fileName)
        {
            string headerStr = "Cam_" + Name.Replace(" ", string.Empty);
            FileOperation.SaveData(fileName, headerStr, "RobotClass", Para.RobotCurrentClass);  
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (SerailMgr.SerailComList[0].setLightSource_mes("1,0,2,0"))
            {
                return;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (SerailMgr.SerailComList[0].setLightSource_mes("1,255,2,255"))
            {
                return;
            }
        }

        private void labelCamera_Click(object sender, EventArgs e)
        {
            this.mainTC.SelectedTab = this.CamTP;
        }

        private void labelSet_Click(object sender, EventArgs e)
        {
            this.mainTC.SelectedTab = this.SetTP;
        }

        private void labelHome_Click(object sender, EventArgs e)
        {
            this.mainTC.SelectedTab = this.HomeTP;
            disp_speed();
        }
        private void labelLogin_Click(object sender, EventArgs e)
        {
            this.mainTC.SelectedTab = this.userTP;
        }

        private void HomeBtn_Click(object sender, EventArgs e)
        {
            ////MessageBox.Show("Please Clear All Samples.", "Homing", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //SeqMgr.begin();

            //HomeBtn.Enabled = false;
            //labelStart.Enabled = false;
            //labelStop.Enabled = false;
            //labelPause.Enabled = false;

            //if (PanaMgr.PanaList[0].Write_Single_R(73, 1))//启动设备
            //{
            //    DateTime st_time = DateTime.Now;

            //    TimeSpan time_span;//记录回原时间，超时回原失败
            //    while(PanaMgr.PanaList[0].Read_Single_R(74) != 1)
            //    {
            //        Thread.Sleep(500);
            //        Application.DoEvents();
            //        time_span = DateTime.Now - st_time;
            //        if (time_span.TotalMilliseconds > 25000)
            //        {
            //            break;
            //        }
            //    }
            //}

            //SeqMgr.StopAuto();
            //UIStopClicked();

            
            //Thread.Sleep(200);
            //SeqMgr.StartHoming();
            //Para.isRotaryError = false;
            //Para.EndLot = false;
            //SeqMgr.ResetAll();
            //HomeBtn.Enabled = true;
            //UIStopClicked();

            

        }


        private void labelStart_Click(object sender, EventArgs e)
        {
            //if (PanaMgr.PanaList[0].Read_Single_R(74) != 1)//回原完成标志
            //{
            //    MessageBox.Show("回原未完成，请先回原");
            //    return;
            //}
            //if (PanaMgr.PanaList[0].Write_Single_R(70, 1))//启动设备
            //{ 
            //    Thread.Sleep(50);
            //    if (PanaMgr.PanaList[0].Read_Single_R(75) == 1)//再次查询是否启动
            //    {
            //        RunningLight();
            //        labelHome_Click(sender, e);
            //        Thread.Sleep(100);
            //        HomeBtn.Enabled = false;
            //        //SeqMgr.begin();
            //       UIStartClicked();
            //    }
            //    else
            //    {
            //        MessageBox.Show("启动失败");
            //    }
            //}
        }

        private void labelPause_Click(object sender, EventArgs e)
        {

        }

        private void labelStop_Click(object sender, EventArgs e)
        {
            //SeqMgr.stop();
            //UIStopClicked();
        }
        //和A162通讯的网口
        private void label20_Click(object sender, EventArgs e)
        {
            A162ComMgr.ShowSettings();
        }
        //和机器人的网口通讯

        private void label16_Click(object sender, EventArgs e)
        {
            PanaMgr.WinPana_Show();
        }

        private void LinghtSource_Click(object sender, EventArgs e)
        {
            SerailMgr.SP_Win_Show();
        }
    
        private void Robot_comm_Click(object sender, EventArgs e)
        {
            RobotMgr.TcpseriveComMgr_Win();
        }

        private void HomeTP_Enter(object sender, EventArgs e)
        {
            HalconWin3.Parent = panel1;
        }

        private void CamTP_Enter(object sender, EventArgs e)
        {
            HalconWin3.Parent = Mpanel1;
        }

        public void UpdateStatusLbl(Color myColor)
        {
            if (StatusLbl.InvokeRequired)
            {
                StatusLbl.BeginInvoke(new Action(() =>
                {
                    StatusLbl.BackColor = myColor;
                }));
            }
            else
                StatusLbl.BackColor = myColor;
        }


       
        //显示当前的设备状态
        public void WriteOperationinformation(string LogInfor)
        {
            try
            {
                if (RunLogDG.InvokeRequired)
                {
                    RunLogDG.Invoke(
                        new Action(() =>
                        {
                            RunLogDG.RowCount = RunLogDG.RowCount + 1;
                            int rwCnt = RunLogDG.RowCount;
                            RunLogDG.ColumnCount = 2;
                            RunLogDG.Rows[rwCnt - 1].Cells[0].Value = DateTime.Now.ToString("dd MM HH:mm:ss");
                            RunLogDG.Rows[rwCnt - 1].Cells[1].Value = LogInfor;
                            RunLogDG.FirstDisplayedScrollingRowIndex = RunLogDG.Rows.Count - 1;
                        })

                        );
                }
                else
                {
                    RunLogDG.RowCount = RunLogDG.RowCount + 1;
                    int rwCnt = RunLogDG.RowCount;
                    RunLogDG.ColumnCount = 2;
                    RunLogDG.Rows[rwCnt - 1].Cells[0].Value = DateTime.Now.ToString("dd MM HH:mm:ss");
                    RunLogDG.Rows[rwCnt - 1].Cells[1].Value = LogInfor;
                    RunLogDG.FirstDisplayedScrollingRowIndex = RunLogDG.Rows.Count - 1;
                }

            }
            catch
            {
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int access = uLogin.GetAccess(userCB.SelectedItem.ToString(), textBoxPassword.Text);
            SetUIAccess(access);
        }

        private void textBoxPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                int access = uLogin.GetAccess(userCB.SelectedItem.ToString(), textBoxPassword.Text);
                SetUIAccess(access);
            }
        }

        public void SetUIAccess(int accesslvl)
        {
            configCB.Enabled = true;
          
            mchNameEB.Enabled = true;
            CreateNewBtn.Enabled = true;
            SaveBtn.Enabled = true;
           
            label20.Enabled = true;
           
            PLCSEtButton.Enabled = true;
           
            switch (accesslvl)
            {
                case 0: //Operator
                    configCB.Enabled = false;
                    mchNameEB.Enabled = false;
                    CreateNewBtn.Enabled = false;
                    SaveBtn.Enabled = false;
                   
                    label20.Enabled = false;
                   
                    PLCSEtButton.Enabled = false;

                    LinghtSource.Enabled = false;
                    PLCSEtButton.Enabled = false;
                    labelSet.Enabled = false;
                    labelCamera.Enabled = false;
                   
                    label21.Text = uLogin.user[0];
                    break;
                case 1: // Engineer

                   
                    label21.Text = uLogin.user[1];
                    break;
                case 2: //full Access

                    configCB.Enabled = true;
                    mchNameEB.Enabled = true;
                    CreateNewBtn.Enabled = true;
                    SaveBtn.Enabled = true;

                    label20.Enabled = true;

                    PLCSEtButton.Enabled = true;

                    LinghtSource.Enabled = true;
                    PLCSEtButton.Enabled = true;
                    labelSet.Enabled = true;
                    labelCamera.Enabled = true;


                    label21.Text = uLogin.user[2];
                    break;

            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            SetUIAccess(0);
        }

        public void Errorstop()
        {
            int Rbian = 0;
            int RR = 0;
            while (true)
            {
                RR = PanaMgr.PanaList[0].Read_Single_R(75);
                if (RR != Rbian)
                {
                    if (RR == 1)
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                object o = new object();
                                EventArgs g = new EventArgs();
                                UIStartClicked();
                                //labelStop_Click(o, g);
                            }));
                        }
                        else
                        {
                            object o = new object();
                            EventArgs g = new EventArgs();
                            //labelSet_Click(o, g);
                        }
                    }
                    else
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                object o = new object();
                                EventArgs g = new EventArgs();
                                UIStopClicked();
                                //labelStop_Click(o, g);
                            }));
                        }
                        else
                        {
                            object o = new object();
                            EventArgs g = new EventArgs();
                            //labelSet_Click(o, g);
                        }
                    }
                }
                Rbian = RR;

                Thread.Sleep(800);
            }  
        }

        #region statu

        //显示灯的状态
        private void RunningLight()
        {

            UpdateStatusLbl(Color.Lime);
        }

        private void StoppingLight()
        {

            UpdateStatusLbl(Color.Orange);
        }

        private void ErrorLight()
        {

            UpdateStatusLbl(Color.Red);
        }

        public void UIStartClicked()
        {
            HomeBtn.Enabled = false;
            labelStart.Enabled = false;
            labelStop.Enabled = true;
            labelPause.Enabled = true;

            RunningLight();

            labelSet.Enabled = false;
            labelCamera.Enabled = false;

            WriteOperationinformation("Auto Run Started");
        }

        public void UIStopClicked()
        {
            HomeBtn.Enabled = true;
            labelStart.Enabled = true;
            labelStop.Enabled = false;
            labelPause.Enabled = false;

            labelSet.Enabled = true;
            labelCamera.Enabled = true;

            WriteOperationinformation("Auto Run Stop");
            StoppingLight();
        }


        #endregion

        public void UpdateA162CountStatusLbl(Color myColor)
        {
            if (this.InvokeRequired)
            {
                A162countLight.BeginInvoke(new Action(() =>
            {
                A162countLight.BackColor = myColor;
            }));
            }
            else
            {
                A162countLight.BackColor = myColor;
            }
        }
        public void UpdateRobotCountStatusLbl(Color myColor,bool st)
        {
            if (this.InvokeRequired)
            {
                Robotcountlight.BeginInvoke(new Action(() =>
                {
                    Robotcountlight.BackColor = myColor;
                    if (st)
                        UIStartClicked();
                    else
                        UIStopClicked();
                }));
            }
            else
            {
                Robotcountlight.BackColor = myColor;
                if (st)
                    UIStartClicked();
                else
                    UIStopClicked();
            }
        }


        //baocun weizhi 

        private void SavePho1butt_Click(object sender, EventArgs e)
        {
            if (camera3.IsCameraOpen())
            {
                camera3.StopCamera();
                //EnbToolBtn(true);
                McamStatusLbl.Text = "Idle...";
                McamStatusLbl.BackColor = Color.Orange;
            }

            for (int c = 0; c < 3; c++)
            {
                if (c == 1)
                    camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 0.5)));
                if (c == 2)
                    camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 1)));

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
                    return;
                }
                RobotData RDate = hWndCtrl3.RobotInspect(camera3.myImage, RobotClassPath + "\\Mod1");
                if (RDate.Found)
                {
                    HTuple hv_ReTuple = new HTuple();
                    HOperatorSet.ReadTuple(Para.RobotCurrentClass + "\\Mod1" + "\\BiaoDing1.tup", out hv_ReTuple);

                    HTuple xx, yy;
                    HOperatorSet.AffineTransPoint2d(hv_ReTuple, RDate.CenterX, RDate.CenterY, out xx, out yy);

                    hWndCtrl3.RobotMod[0].CenterX = xx;
                    hWndCtrl3.RobotMod[0].CenterY = yy;
                    hWndCtrl3.RobotMod[0].CenterAngle = RDate.CenterAngle;

                    hWndCtrl3.Save(mchSettingsFilePath);
                    MessageBox.Show("Photo1 Pos Save OK");
                    return;
                }
            }
            MessageBox.Show("Photo1 Pos Save Error");
        }

        private void SavePho2butt_Click(object sender, EventArgs e)
        {
            if (camera3.IsCameraOpen())
            {
                camera3.StopCamera();
                //EnbToolBtn(true);
                McamStatusLbl.Text = "Idle...";
                McamStatusLbl.BackColor = Color.Orange;
            }

            for (int c = 0; c < 3; c++)
            {
                if (c == 1)
                    camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 0.5)));
                if (c == 2)
                    camera3.SetExposure(Para.RobotExposeTime + Convert.ToInt32((Convert.ToDouble(Para.RobotExposeTime) * 1)));

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
                    return;
                }
                RobotData RDate = hWndCtrl3.RobotInspect(camera3.myImage, RobotClassPath + "\\Mod2");
                if (RDate.Found)
                {
                    HTuple hv_ReTuple = new HTuple();
                    HOperatorSet.ReadTuple(Para.RobotCurrentClass + "\\Mod2" + "\\BiaoDing2.tup", out hv_ReTuple);

                    HTuple xx, yy;
                    HOperatorSet.AffineTransPoint2d(hv_ReTuple, RDate.CenterX, RDate.CenterY, out xx, out yy);

                    hWndCtrl3.RobotMod[1].CenterX = xx;
                    hWndCtrl3.RobotMod[1].CenterY = yy;
                    hWndCtrl3.RobotMod[1].CenterAngle = RDate.CenterAngle;

                    hWndCtrl3.Save(mchSettingsFilePath);

                    MessageBox.Show("Photo2 Pos Save OK");
                    return;
                }
            }
            MessageBox.Show("Photo2 Pos Save Error");
        }

        private void EndlotCheck_CheckedChanged(object sender, EventArgs e)
        {
            Para.EndLot = EndlotCheck.Checked;
        }
        public void Dis_EndLot()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => 
                {
                    if (Para.EndLot)
                        EndlotCheck.Checked = true;
                    else
                        EndlotCheck.Checked = false;
                }));
            }
            else
            {
                if (Para.EndLot)
                    EndlotCheck.Checked = true;
                else
                    EndlotCheck.Checked = false;
            }
        }

        public void disp_count(List<int> count)
        {
            if (count.Count == 7)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        count1.Text = count[0].ToString();
                        count2.Text = count[1].ToString();
                        count3.Text = count[2].ToString();
                        count4.Text = count[3].ToString();
                        count5.Text = count[4].ToString();
                        count6.Text = count[5].ToString();
                        count7.Text = count[6].ToString();
                    }));
                }
                else
                {
                    count1.Text = count[0].ToString();
                    count2.Text = count[1].ToString();
                    count3.Text = count[2].ToString();
                    count4.Text = count[3].ToString();
                    count5.Text = count[4].ToString();
                    count6.Text = count[5].ToString();
                    count7.Text = count[6].ToString();
                }
            }
        }
        public void disp_ReCG(string str)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => 
                {
                    switch (str)
                    {
                        case"0":
                            Re1.Text = "0";
                            Re2.Text = "0";
                            break;
                        case"1":
                            Re1.Text = "1";
                            Re2.Text = "0";
                            break;
                        case"2":
                            Re1.Text = "0";
                            Re2.Text = "1";
                            break;
                        case"3":
                            Re1.Text = "1";
                            Re2.Text = "1";
                            break;
                        default:
                            break;
                    }
                }));
            }
            else
            {
                switch (str)
                {
                    case "0":
                        Re1.Text = "0";
                        Re2.Text = "0";
                        break;
                    case "1":
                        Re1.Text = "1";
                        Re2.Text = "0";
                        break;
                    case "2":
                        Re1.Text = "0";
                        Re2.Text = "1";
                        break;
                    case "3":
                        Re1.Text = "1";
                        Re2.Text = "1";
                        break;
                    default:
                        break;
                }
            }
        }

        public void disp_speed()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => 
                {
                    powertext.Text = RobotMgr.TcpserList[0].Power;
                    speedtext.Text = RobotMgr.TcpserList[0].Speed.ToString();
                    acceltext.Text = RobotMgr.TcpserList[0].Accel.ToString();
                }));  
            }
            else
            {
                powertext.Text = RobotMgr.TcpserList[0].Power;
                speedtext.Text = RobotMgr.TcpserList[0].Speed.ToString();
                acceltext.Text = RobotMgr.TcpserList[0].Accel.ToString();
            }
        }

        private void GRR_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            Para.GRRBool = GRR_checkbox.Checked;
        }


        TestMessage Tform = new TestMessage();//用于弹出测试框
        public void DispTestShow()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => { Tform.Show(); }));
            }
        }
        public void DispTestHide()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => { Tform.Hide(); }));
            }
        }
        public void EndLot_Check()//勾选EndLot
        {
            if (this.InvokeRequired)
            {
                EndlotCheck.BeginInvoke(new Action(() => { EndlotCheck.Checked = true; }));
            }
            else
                EndlotCheck.Checked = true;
            
        }
         public void GRR_UnCheck()//取消GRR勾选
        {
            if (this.InvokeRequired)
            {
                GRR_checkbox.BeginInvoke(new Action(() => { GRR_checkbox.Checked = false; }));
            }
            else
                GRR_checkbox.Checked = false;
        }














    }
}

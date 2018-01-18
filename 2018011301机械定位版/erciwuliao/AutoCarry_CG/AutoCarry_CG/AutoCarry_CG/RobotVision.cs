using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JPTCG.Vision;
using Common;
using System.Threading;
using System.IO;

namespace AutoCarry_CG
{
    public class RobotData
    {
        public RobotData()
        { 
        }
        public bool Found = false;
        public double CenterX;
        public double CenterY;
        public double CenterAngle;
        public double Area;
        public double Wide;
        public double Lengh;
    }

    public class RobotCaliValue
    {
        public string Name;
        public RobotCaliValue(string name)
        {
            Name = name;
        }
        public double CenterX;
        public double CenterY;
        public double CenterAngle;

    }


    public class RobotVision
    {
        
        HTuple windowHandle;

        private int windowWidth;
        private int windowHeight;
        private int imageWidth;
        private int imageHeight;


        HalconVision Camera;

        public List<RobotCaliValue> RobotMod = new List<RobotCaliValue>();

        bool disCross;

        public RobotVision(HWindowControl view,HalconVision camera)
        {
            windowHandle = view.HalconWindow;
            windowWidth = view.Size.Width;
            windowHeight = view.Size.Height;

            Camera = camera;

            RobotMod.Add(new RobotCaliValue("Mod1"));
            RobotMod.Add(new RobotCaliValue("Mod2"));

        }


        ~RobotVision()
        { 

        
        }


        public void Save(string fileName)
        {
            string headerStr = "Cam_" + Camera.Name.Replace(" ", string.Empty);//加载相机的时候一定是将名字加载过来
            for (int i = 0; i < RobotMod.Count; i++)
            {
                FileOperation.SaveData(fileName, headerStr, RobotMod[i].Name + "CenterX", RobotMod[i].CenterX.ToString("F4"));
                FileOperation.SaveData(fileName, headerStr, RobotMod[i].Name + "CenterY", RobotMod[i].CenterY.ToString("F4"));
                FileOperation.SaveData(fileName, headerStr, RobotMod[i].Name + "CenterAngle", RobotMod[i].CenterAngle.ToString("F4"));
            }
        }
        public void Load(string fileName)
        {
            string strread = "";
            string headerStr = "Cam_" + Camera.Name.Replace(" ", string.Empty);//加载相机的时候一定是将名字加载过来
            for (int i = 0; i < RobotMod.Count; i++)
            {
                FileOperation.ReadData(fileName, headerStr, RobotMod[i].Name + "CenterX", ref strread);
                if (strread != "0")
                    RobotMod[i].CenterX = double.Parse(strread);
                FileOperation.ReadData(fileName, headerStr, RobotMod[i].Name + "CenterY", ref strread);
                if (strread != "0")
                    RobotMod[i].CenterY = double.Parse(strread);
                FileOperation.ReadData(fileName, headerStr, RobotMod[i].Name + "CenterAngle", ref strread);
                if (strread != "0")
                    RobotMod[i].CenterAngle = double.Parse(strread);
            }
        }

        public void disp_HImage(HObject myImage)
        {
            try
            {
                HTuple wid, Heigh;
                HOperatorSet.GetImageSize(myImage, out wid, out Heigh);
                HOperatorSet.SetPart(windowHandle, 0, 0, Heigh, wid);

                HOperatorSet.DispObj(myImage, windowHandle);

                HOperatorSet.SetColor(windowHandle, "red");
                HOperatorSet.DispCross(windowHandle, Heigh / 2 , wid / 2, Heigh + wid, 0);

                Application.DoEvents();

            }
            catch
            {
            } 
        }

        
        public RobotData RobotInspect(HObject ho_Image,string strpath)
        {

            RobotData InspRobotData = new RobotData();

            // Local iconic variables 

            HObject ho_TransContours = null, ho_ReRegion11;
            HObject ho_ReRegion12, ho_ReModelContours, ho_Image1 = null;
            HObject ho_ImageEmphasize1 = null, ho_TransCosntours = null;
            HObject ho_RegionAffineTrans1 = null, ho_RegionAffineTrans2 = null;
            HObject ho_ImageReduced1 = null, ho_ImageReduced2 = null, ho_Regions1 = null;
            HObject ho_Regions2 = null, ho_regionline = null, ho_Cross = null;

            // Local control variables 

            HTuple hv_MatchingObjIdx = new HTuple(), hv_WindowHandle = null;
            HTuple hv_ReModelID = null, hv_ReTuple = null, hv_i = null;
            HTuple hv_ReWidth = new HTuple(), hv_ReHeight = new HTuple();
            HTuple hv_ModelRow = new HTuple(), hv_ModelColumn = new HTuple();
            HTuple hv_ModelAngle = new HTuple(), hv_ModelScore = new HTuple();
            HTuple hv_HomMat1 = new HTuple(), hv_HomMa1t = new HTuple();
            HTuple hv_HomMat2D = new HTuple(), hv_Area = new HTuple();
            HTuple hv_centerRow1 = new HTuple(), hv_centerColumn1 = new HTuple();
            HTuple hv_centerRow2 = new HTuple(), hv_centerColumn2 = new HTuple();
            HTuple hv_Angle = new HTuple(), hv_DeAngle = new HTuple();
            HTuple hv_centerRow = new HTuple(), hv_centerColumn = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_TransContours);
            HOperatorSet.GenEmptyObj(out ho_ReRegion11);
            HOperatorSet.GenEmptyObj(out ho_ReRegion12);
            HOperatorSet.GenEmptyObj(out ho_ReModelContours);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_ImageEmphasize1);
            HOperatorSet.GenEmptyObj(out ho_TransCosntours);
            HOperatorSet.GenEmptyObj(out ho_RegionAffineTrans1);
            HOperatorSet.GenEmptyObj(out ho_RegionAffineTrans2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
            HOperatorSet.GenEmptyObj(out ho_Regions1);
            HOperatorSet.GenEmptyObj(out ho_Regions2);
            HOperatorSet.GenEmptyObj(out ho_regionline);
            HOperatorSet.GenEmptyObj(out ho_Cross);

            //读取测量模板
            HTuple hv_Index = null;
            HTuple hv_MetrologyHandle = null;


            try
            {
                //************************读取模板**************************************
                InspRobotData.Found = false;//判断是否找到模板

                dev_update_off();
                HOperatorSet.SetSystem("border_shape_models", "false");
                HOperatorSet.SetSystem("clip_region", "false");
                HOperatorSet.SetDraw(windowHandle, "margin");

                HOperatorSet.GetImageSize(ho_Image, out hv_ReWidth, out hv_ReHeight);
                disp_HImage(ho_Image);

                
                HOperatorSet.ReadShapeModel(strpath+"\\moban.shm",out hv_ReModelID);
                HOperatorSet.ReadTuple(strpath + "\\dtaa.tup", out hv_ReTuple);
                ho_ReRegion11.Dispose();
                HOperatorSet.ReadRegion(out ho_ReRegion11, strpath + "\\recg1.reg");
                ho_ReRegion12.Dispose();
                HOperatorSet.ReadRegion(out ho_ReRegion12, strpath + "\\recg2.reg");
                ho_ReModelContours.Dispose();

                //读取测量模板
                HOperatorSet.ReadTuple(strpath + "\\metrologyIndex.tup", out hv_Index);
                HOperatorSet.ReadMetrologyModel(strpath + "\\metrology.mtr", out hv_MetrologyHandle);

   
                HOperatorSet.GetShapeModelContours(out ho_ReModelContours, hv_ReModelID, 1);

                //************************进行检测**************************************

                    ho_ImageEmphasize1.Dispose();
                    //HOperatorSet.Emphasize(ho_Image, out ho_ImageEmphasize1, hv_ReWidth, hv_ReHeight,5);
                    HOperatorSet.FindShapeModel(ho_Image, hv_ReModelID, (new HTuple(0)).TupleRad()
                        , (new HTuple(360)).TupleRad(), 0.5, 1, 0.6, "least_squares", (new HTuple(4)).TupleConcat(
                        1), 0.75, out hv_ModelRow, out hv_ModelColumn, out hv_ModelAngle, out hv_ModelScore);
                    disp_message(windowHandle, hv_ModelScore, "window", 0, 0, "black", "true");
                    for (hv_MatchingObjIdx = 0; (int)hv_MatchingObjIdx <= (int)((new HTuple(hv_ModelScore.TupleLength()
                        )) - 1); hv_MatchingObjIdx = (int)hv_MatchingObjIdx + 1)
                    {
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat1);
                        HOperatorSet.HomMat2dRotate(hv_HomMat1, hv_ModelAngle.TupleSelect(hv_MatchingObjIdx),
                            0, 0, out hv_HomMat1);
                        HOperatorSet.HomMat2dTranslate(hv_HomMat1, hv_ModelRow.TupleSelect(hv_MatchingObjIdx),
                            hv_ModelColumn.TupleSelect(hv_MatchingObjIdx), out hv_HomMa1t);
                        ho_TransContours.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_ReModelContours, out ho_TransContours,
                            hv_HomMat1);

                        //HOperatorSet.DispObj(ho_TransContours, windowHandle);

                        HOperatorSet.VectorAngleToRigid(hv_ReTuple.TupleSelect(0), hv_ReTuple.TupleSelect(
                            1), hv_ReTuple.TupleSelect(2), hv_ModelRow.TupleSelect(0), hv_ModelColumn.TupleSelect(
                            0), hv_ModelAngle.TupleSelect(0), out hv_HomMat2D);
                        ho_TransCosntours.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_ReModelContours, out ho_TransCosntours,
                            hv_HomMat2D);

                        ho_RegionAffineTrans1.Dispose();
                        HOperatorSet.AffineTransRegion(ho_ReRegion11, out ho_RegionAffineTrans1,
                            hv_HomMat2D, "nearest_neighbor");

                        HOperatorSet.DispObj(ho_RegionAffineTrans1, windowHandle);

                        ho_RegionAffineTrans2.Dispose();
                        HOperatorSet.AffineTransRegion(ho_ReRegion12, out ho_RegionAffineTrans2,
                            hv_HomMat2D, "nearest_neighbor");

                        HOperatorSet.DispObj(ho_RegionAffineTrans2, windowHandle);

                        ho_ImageReduced1.Dispose();
                        HOperatorSet.ReduceDomain(ho_Image, ho_RegionAffineTrans1, out ho_ImageReduced1);
                        ho_ImageReduced2.Dispose();
                        HOperatorSet.ReduceDomain(ho_Image, ho_RegionAffineTrans2, out ho_ImageReduced2);
                        
                        ho_Regions1.Dispose();
                        HOperatorSet.Threshold(ho_ImageReduced1, out ho_Regions1, 130, 255);
                        ho_Regions2.Dispose();
                        HOperatorSet.Threshold(ho_ImageReduced2, out ho_Regions2, 130, 255);
                        HOperatorSet.AreaCenter(ho_Regions1, out hv_Area, out hv_centerRow1, out hv_centerColumn1);
                        HOperatorSet.AreaCenter(ho_Regions2, out hv_Area, out hv_centerRow2, out hv_centerColumn2);
                        HOperatorSet.AngleLl(0, 0, 0, 100, hv_centerRow1, hv_centerColumn1, hv_centerRow2,
                            hv_centerColumn2, out hv_Angle);
                        hv_DeAngle = hv_Angle.TupleDeg();
                        hv_centerRow = (hv_centerRow2 + hv_centerRow1) / 2;
                        hv_centerColumn = (hv_centerColumn2 + hv_centerColumn1) / 2;

                        HTuple hv_circleParameter = null;

                        HOperatorSet.AlignMetrologyModel(hv_MetrologyHandle, hv_centerRow1, hv_centerColumn1, 0);
                        HOperatorSet.ApplyMetrologyModel(ho_Image, hv_MetrologyHandle);
                        HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                            "all_param", out hv_circleParameter);
                        //ho_Circle1.Dispose();
                        //HOperatorSet.GenCircle(out ho_Circle1, hv_circleParameter.TupleSelect(0), hv_circleParameter.TupleSelect(
                        //    1), hv_circleParameter.TupleSelect(2));

                        //rd.CenterR = hv_circleParameter.TupleSelect(0).D;
                        //rd.CenterC = hv_circleParameter.TupleSelect(1).D;
                        HObject ho_Contour = null;
                        HOperatorSet.GenEmptyObj(out ho_Contour);
                        ho_Contour.Dispose();
                        HTuple hv_MRow = null;
                        HTuple hv_MColumn = null;

                        //HOperatorSet.DispObj(ho_Circle1, windowHandle);
                        ho_Contour.Dispose();

                        //HOperatorSet.GetMetrologyObjectMeasures(out ho_Contour, hv_MetrologyHandle, "all",
                        //    "all", out hv_MRow, out hv_MColumn);
                        //HOperatorSet.DispObj(ho_Contour, windowHandle);

                        InspRobotData.CenterX = hv_circleParameter.TupleSelect(1).D;//图像中的Column对应机械手的X值
                        InspRobotData.CenterY = hv_circleParameter.TupleSelect(0).D;//图像中的Row对应机械手的Y值
                        InspRobotData.CenterAngle = hv_DeAngle;//图像中的角度column水平线，对应机械手的角度加和减

                        ho_regionline.Dispose();
                        HOperatorSet.GenRegionLine(out ho_regionline, hv_centerRow1, hv_centerColumn1,
                            hv_centerRow2, hv_centerColumn2);
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, hv_centerRow, hv_centerColumn,
                            20, 1);

                        HOperatorSet.DispObj(ho_regionline, windowHandle);

                        HOperatorSet.DispObj(ho_Cross, windowHandle);

                        InspRobotData.Found = true;//判断是否找到模板
                        if (hv_ModelScore[0] < 0.8)
                        {
                            InspRobotData.Found = false;
                        }
                    }
                //}

            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_TransContours.Dispose();
                ho_ReRegion11.Dispose();
                ho_ReRegion12.Dispose();
                ho_ReModelContours.Dispose();
                ho_Image1.Dispose();
                //ho_ImageEmphasize1.Dispose();
                ho_TransCosntours.Dispose();
                ho_RegionAffineTrans1.Dispose();
                ho_RegionAffineTrans2.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ImageReduced2.Dispose();
                ho_Regions1.Dispose();
                ho_Regions2.Dispose();
                ho_regionline.Dispose();
                ho_Cross.Dispose();

                HOperatorSet.ClearShapeModel(hv_ReModelID);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                InspRobotData.Found = false;

                return InspRobotData;//返回我们需要的数据  

                //throw HDevExpDefaultException;
            }
            ho_TransContours.Dispose();
            ho_ReRegion11.Dispose();
            ho_ReRegion12.Dispose();
            ho_ReModelContours.Dispose();
            ho_Image1.Dispose();
            //ho_ImageEmphasize1.Dispose();
            ho_TransCosntours.Dispose();
            ho_RegionAffineTrans1.Dispose();
            ho_RegionAffineTrans2.Dispose();
            ho_ImageReduced1.Dispose();
            ho_ImageReduced2.Dispose();
            ho_Regions1.Dispose();
            ho_Regions2.Dispose();
            ho_regionline.Dispose();
            ho_Cross.Dispose();

            HOperatorSet.ClearShapeModel(hv_ReModelID);
            HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);

            return InspRobotData;//返回我们需要的数据  
        }


        public bool CreatMod(HObject ho_Image1,string strpath)
        {
            if (!Directory.Exists(strpath))
            {
                Directory.CreateDirectory(strpath);
            }

            // windowHandle Local iconic variables 

            HObject ho_ImageEmphasize, ho_ModelRegion1;
            HObject ho__TmpRegion, ho_ModelRegion, ho_TemplateImage, ho_TemplateImage1;
            HObject ho_ModelContours, ho_TransContours = null;
            HObject ho_Image,ho_Regions1 = null;;
            HObject shiyan;

            // Local control variables 
            HTuple hv_Width1 = null, hv_Height1 = null;
            HTuple hv_Row = null, hv_Column = null, hv_Radi = null;
            HTuple hv_Length1 = null, hv_Length2 = null, hv_Row1 = null;
            HTuple hv_Column1 = null, hv_Phi1 = null, hv_Length11 = null;
            HTuple hv_Length21 = null, hv_ModelId = null, hv_ModelRow1 = null;
            HTuple hv_ModelColumn1 = null, hv_ModelAngle1 = null, hv_ModelScore1 = null;
            HTuple hv_MatchingObjIdx = null, hv_HomMat = new HTuple();
            HTuple hv_data = null;
            HTuple hv_Area = null, hv_Ro = null, hv_cl = null;

            // Initialize local and output iconic variables 
            
            HOperatorSet.GenEmptyObj(out ho_ImageEmphasize);
            HOperatorSet.GenEmptyObj(out ho_ModelRegion1);
            HOperatorSet.GenEmptyObj(out ho__TmpRegion);
            HOperatorSet.GenEmptyObj(out ho_ModelRegion);
            HOperatorSet.GenEmptyObj(out ho_TemplateImage);
            HOperatorSet.GenEmptyObj(out ho_TemplateImage1);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_TransContours);
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out shiyan);
            HOperatorSet.GenEmptyObj(out ho_Regions1);

            try
            {
                HOperatorSet.CopyImage(ho_Image1, out ho_Image);
                dev_update_off();
                //************************************图片预处理************************************
   
                //HOperatorSet.ReadImage(out ho_Image, "C:/Users/Administrator/Desktop/ceshi/黑/2.bmp");
                HOperatorSet.SetLineWidth(windowHandle, 1);
                HOperatorSet.SetDraw(windowHandle, "margin");
                HOperatorSet.GetImageSize(ho_Image, out hv_Width1, out hv_Height1);
                ho_ImageEmphasize.Dispose();
                //HOperatorSet.Emphasize(ho_Image, out ho_ImageEmphasize, hv_Width1, hv_Height1, 5);
                //HOperatorSet.DispObj(ho_Image, windowHandle);
                HOperatorSet.SetSystem("border_shape_models", "false");
                disp_HImage(ho_Image);
                //************************************选择圆孔************************************
                disp_message(windowHandle, "Please Select circle", "window", 0, 0, "black", "true");
                HOperatorSet.DrawCircle(windowHandle, out hv_Row, out hv_Column, out hv_Radi);
                ho_ModelRegion1.Dispose();
                HOperatorSet.GenCircle(out ho_ModelRegion1,  hv_Row,  hv_Column,  hv_Radi);
                
                ho_TemplateImage1.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_ModelRegion1, out ho_TemplateImage1);

                //************************************选择腰型孔************************************
                HOperatorSet.ClearWindow(windowHandle);
                //HOperatorSet.DispObj(ho_Image, windowHandle);
                disp_HImage(ho_Image);
                disp_message(windowHandle, "Please Select rectangle", "window", 12, 12, "black", "false");
                HOperatorSet.DrawRectangle2(windowHandle, out hv_Row1, out hv_Column1, out hv_Phi1,
                    out hv_Length11, out hv_Length21);
                ho__TmpRegion.Dispose();
                HOperatorSet.GenRectangle2(out ho__TmpRegion, hv_Row1, hv_Column1, hv_Phi1,
                    hv_Length11, hv_Length21);
                ho_ModelRegion.Dispose();
                HOperatorSet.Union2(ho_ModelRegion1, ho__TmpRegion, out ho_ModelRegion);
                //************************************创建模板************************************
                ho_TemplateImage.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_ModelRegion, out ho_TemplateImage);
                HOperatorSet.CreateShapeModel(ho_TemplateImage, 4, (new HTuple(0)).TupleRad()
                    , (new HTuple(360)).TupleRad(), (new HTuple(0.183)).TupleRad(), (new HTuple("none")).TupleConcat(
                    "no_pregeneration"), "use_polarity", ((new HTuple(35)).TupleConcat(116)).TupleConcat(
                    7), 4, out hv_ModelId);
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelId, 1);
                HOperatorSet.FindShapeModel(ho_Image, hv_ModelId, (new HTuple(0)).TupleRad()
                    , (new HTuple(360)).TupleRad(), 0.8, 1, 0, "least_squares", (new HTuple(4)).TupleConcat(
                    1), 0.75, out hv_ModelRow1, out hv_ModelColumn1, out hv_ModelAngle1, out hv_ModelScore1);
                for (hv_MatchingObjIdx = 0; (int)hv_MatchingObjIdx <= (int)((new HTuple(hv_ModelScore1.TupleLength()
                    )) - 1); hv_MatchingObjIdx = (int)hv_MatchingObjIdx + 1)
                {
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat);
                    HOperatorSet.HomMat2dRotate(hv_HomMat, hv_ModelAngle1.TupleSelect(hv_MatchingObjIdx),
                        0, 0, out hv_HomMat);
                    HOperatorSet.HomMat2dTranslate(hv_HomMat, hv_ModelRow1.TupleSelect(hv_MatchingObjIdx),
                        hv_ModelColumn1.TupleSelect(hv_MatchingObjIdx), out hv_HomMat);
                    ho_TransContours.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_TransContours,
                        hv_HomMat);
                    //disp_HImage(ho_TransContours);
                    HOperatorSet.DispObj(ho_TransContours, windowHandle);
                }

                //创建拟合圆
                HTuple hv_MetrologyHandle = null;
                HTuple hv_Index = null;
                ho_Regions1.Dispose();
                HOperatorSet.Threshold(ho_TemplateImage1, out ho_Regions1, 130, 255);
                HOperatorSet.AreaCenter(ho_Regions1, out hv_Area, out hv_Ro, out hv_cl);

                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width1, hv_Height1);
                HOperatorSet.AddMetrologyObjectCircleMeasure(hv_MetrologyHandle, hv_Row, hv_Column,
                    hv_Radi, 20, 10, 1, 10, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyModelParam(hv_MetrologyHandle, "reference_system", ((hv_Ro.TupleConcat(
                    hv_cl))).TupleConcat(0));

                hv_data = new HTuple();
                hv_data = hv_data.TupleConcat(hv_ModelRow1.TupleSelect(0));
                hv_data = hv_data.TupleConcat(hv_ModelColumn1.TupleSelect(0));
                hv_data = hv_data.TupleConcat(hv_ModelAngle1.TupleSelect(0));
                //************************************保存模板************************************
                HOperatorSet.WriteMetrologyModel(hv_MetrologyHandle, strpath + "\\metrology.mtr");//保存测量模板
                HOperatorSet.WriteTuple(hv_Index, strpath + "\\metrologyIndex.tup");//保存当前测量模板的下标
                HOperatorSet.WriteShapeModel(hv_ModelId, strpath+"\\moban.shm");
                HOperatorSet.WriteTuple(hv_data, strpath+"\\dtaa.tup");
                HOperatorSet.WriteRegion(ho_ModelRegion1, strpath + "\\recg1.reg");
                HOperatorSet.WriteRegion(ho__TmpRegion, strpath + "\\recg2.reg");
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image1.Dispose();
                ho_Image.Dispose();
                //ho_ImageEmphasize.Dispose();
                ho_ModelRegion1.Dispose();
                ho__TmpRegion.Dispose();
                ho_ModelRegion.Dispose();
                ho_TemplateImage.Dispose();
                ho_ModelContours.Dispose();
                ho_TransContours.Dispose();

                //MessageBox.Show(HDevExpDefaultException.ToString());
                if (hv_ModelId != null)
                {
                    HOperatorSet.ClearShapeModel(hv_ModelId);
                }
                MessageBox.Show("模板创建失败");   
                return false;
                //throw HDevExpDefaultException;
            }
            ho_Image1.Dispose();
            ho_Image.Dispose();
            //ho_ImageEmphasize.Dispose();
            ho_ModelRegion1.Dispose();
            ho__TmpRegion.Dispose();
            ho_ModelRegion.Dispose();
            ho_TemplateImage.Dispose();
            ho_ModelContours.Dispose();
            ho_TransContours.Dispose();
            HOperatorSet.ClearShapeModel(hv_ModelId);
            return true;
        }

        public RobotData Inspect(HObject ho_Image, RobotCaliValue rcv, string strpath)
        {
            RobotData rd = new RobotData();
            rd = RobotInspect(ho_Image,strpath);
            return rd;   
        }


        #region Halcon显示


        public void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
    HTuple hv_Bold, HTuple hv_Slant)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = null, hv_BufferWindowHandle = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_SubFamily = new HTuple(), hv_Fonts = new HTuple();
            HTuple hv_SystemFonts = new HTuple(), hv_Guess = new HTuple();
            HTuple hv_I = new HTuple(), hv_Index = new HTuple(), hv_AllowedFontSizes = new HTuple();
            HTuple hv_Distances = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_FontSelRegexp = new HTuple(), hv_FontsCourier = new HTuple();
            HTuple hv_Bold_COPY_INP_TMP = hv_Bold.Clone();
            HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
            HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();
            HTuple hv_Slant_COPY_INP_TMP = hv_Slant.Clone();

            // Initialize local and output iconic variables 
            //This procedure sets the text font of the current window with
            //the specified attributes.
            //It is assumed that following fonts are installed on the system:
            //Windows: Courier New, Arial Times New Roman
            //Mac OS X: CourierNewPS, Arial, TimesNewRomanPS
            //Linux: courier, helvetica, times
            //Because fonts are displayed smaller on Linux than on Windows,
            //a scaling factor of 1.25 is used the get comparable results.
            //For Linux, only a limited number of font sizes is supported,
            //to get comparable results, it is recommended to use one of the
            //following sizes: 9, 11, 14, 16, 20, 27
            //(which will be mapped internally on Linux systems to 11, 14, 17, 20, 25, 34)
            //
            //Input parameters:
            //WindowHandle: The graphics window for which the font will be set
            //Size: The font size. If Size=-1, the default of 16 is used.
            //Bold: If set to 'true', a bold font is used
            //Slant: If set to 'true', a slanted font is used
            //
            HOperatorSet.GetSystem("operating_system", out hv_OS);
            // dev_get_preferences(...); only in hdevelop
            // dev_set_preferences(...); only in hdevelop
            if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
            {
                hv_Size_COPY_INP_TMP = 16;
            }
            if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
            {
                //Set font on Windows systems
                try
                {
                    //Check, if font scaling is switched on
                    HOperatorSet.OpenWindow(0, 0, 256, 256, 0, "buffer", "", out hv_BufferWindowHandle);
                    HOperatorSet.SetFont(hv_BufferWindowHandle, "-Consolas-16-*-0-*-*-1-");
                    HOperatorSet.GetStringExtents(hv_BufferWindowHandle, "test_string", out hv_Ascent,
                        out hv_Descent, out hv_Width, out hv_Height);
                    //Expected width is 110
                    hv_Scale = 110.0 / hv_Width;
                    hv_Size_COPY_INP_TMP = ((hv_Size_COPY_INP_TMP * hv_Scale)).TupleInt();
                    HOperatorSet.CloseWindow(hv_BufferWindowHandle);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    //throw (Exception)
                }
                if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                    new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "Courier New";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "Consolas";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "Arial";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "Times New Roman";
                }
                if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    hv_Bold_COPY_INP_TMP = 1;
                }
                else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
                {
                    hv_Bold_COPY_INP_TMP = 0;
                }
                else
                {
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    hv_Slant_COPY_INP_TMP = 1;
                }
                else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
                {
                    hv_Slant_COPY_INP_TMP = 0;
                }
                else
                {
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                try
                {
                    HOperatorSet.SetFont(hv_WindowHandle, ((((((("-" + hv_Font_COPY_INP_TMP) + "-") + hv_Size_COPY_INP_TMP) + "-*-") + hv_Slant_COPY_INP_TMP) + "-*-*-") + hv_Bold_COPY_INP_TMP) + "-");
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    //throw (Exception)
                }
            }
            else if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Dar"))) != 0)
            {
                //Set font on Mac OS X systems. Since OS X does not have a strict naming
                //scheme for font attributes, we use tables to determine the correct font
                //name.
                hv_SubFamily = 0;
                if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    hv_SubFamily = hv_SubFamily.TupleBor(1);
                }
                else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    hv_SubFamily = hv_SubFamily.TupleBor(2);
                }
                else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
                {
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Menlo-Regular";
                    hv_Fonts[1] = "Menlo-Italic";
                    hv_Fonts[2] = "Menlo-Bold";
                    hv_Fonts[3] = "Menlo-BoldItalic";
                }
                else if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))).TupleOr(
                    new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                {
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "CourierNewPSMT";
                    hv_Fonts[1] = "CourierNewPS-ItalicMT";
                    hv_Fonts[2] = "CourierNewPS-BoldMT";
                    hv_Fonts[3] = "CourierNewPS-BoldItalicMT";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "ArialMT";
                    hv_Fonts[1] = "Arial-ItalicMT";
                    hv_Fonts[2] = "Arial-BoldMT";
                    hv_Fonts[3] = "Arial-BoldItalicMT";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "TimesNewRomanPSMT";
                    hv_Fonts[1] = "TimesNewRomanPS-ItalicMT";
                    hv_Fonts[2] = "TimesNewRomanPS-BoldMT";
                    hv_Fonts[3] = "TimesNewRomanPS-BoldItalicMT";
                }
                else
                {
                    //Attempt to figure out which of the fonts installed on the system
                    //the user could have meant.
                    HOperatorSet.QueryFont(hv_WindowHandle, out hv_SystemFonts);
                    hv_Fonts = new HTuple();
                    hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                    hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                    hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                    hv_Fonts = hv_Fonts.TupleConcat(hv_Font_COPY_INP_TMP);
                    hv_Guess = new HTuple();
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP);
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Regular");
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "MT");
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                    {
                        HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                        if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_Fonts == null)
                                hv_Fonts = new HTuple();
                            hv_Fonts[0] = hv_Guess.TupleSelect(hv_I);
                            break;
                        }
                    }
                    //Guess name of slanted font
                    hv_Guess = new HTuple();
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Italic");
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-ItalicMT");
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Oblique");
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                    {
                        HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                        if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_Fonts == null)
                                hv_Fonts = new HTuple();
                            hv_Fonts[1] = hv_Guess.TupleSelect(hv_I);
                            break;
                        }
                    }
                    //Guess name of bold font
                    hv_Guess = new HTuple();
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-Bold");
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldMT");
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                    {
                        HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                        if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_Fonts == null)
                                hv_Fonts = new HTuple();
                            hv_Fonts[2] = hv_Guess.TupleSelect(hv_I);
                            break;
                        }
                    }
                    //Guess name of bold slanted font
                    hv_Guess = new HTuple();
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalic");
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldItalicMT");
                    hv_Guess = hv_Guess.TupleConcat(hv_Font_COPY_INP_TMP + "-BoldOblique");
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Guess.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                    {
                        HOperatorSet.TupleFind(hv_SystemFonts, hv_Guess.TupleSelect(hv_I), out hv_Index);
                        if ((int)(new HTuple(hv_Index.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_Fonts == null)
                                hv_Fonts = new HTuple();
                            hv_Fonts[3] = hv_Guess.TupleSelect(hv_I);
                            break;
                        }
                    }
                }
                hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(hv_SubFamily);
                try
                {
                    HOperatorSet.SetFont(hv_WindowHandle, (hv_Font_COPY_INP_TMP + "-") + hv_Size_COPY_INP_TMP);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    //throw (Exception)
                }
            }
            else
            {
                //Set font for UNIX systems
                hv_Size_COPY_INP_TMP = hv_Size_COPY_INP_TMP * 1.25;
                hv_AllowedFontSizes = new HTuple();
                hv_AllowedFontSizes[0] = 11;
                hv_AllowedFontSizes[1] = 14;
                hv_AllowedFontSizes[2] = 17;
                hv_AllowedFontSizes[3] = 20;
                hv_AllowedFontSizes[4] = 25;
                hv_AllowedFontSizes[5] = 34;
                if ((int)(new HTuple(((hv_AllowedFontSizes.TupleFind(hv_Size_COPY_INP_TMP))).TupleEqual(
                    -1))) != 0)
                {
                    hv_Distances = ((hv_AllowedFontSizes - hv_Size_COPY_INP_TMP)).TupleAbs();
                    HOperatorSet.TupleSortIndex(hv_Distances, out hv_Indices);
                    hv_Size_COPY_INP_TMP = hv_AllowedFontSizes.TupleSelect(hv_Indices.TupleSelect(
                        0));
                }
                if ((int)((new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))).TupleOr(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(
                    "Courier")))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "courier";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "helvetica";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Font_COPY_INP_TMP = "times";
                }
                if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    hv_Bold_COPY_INP_TMP = "bold";
                }
                else if ((int)(new HTuple(hv_Bold_COPY_INP_TMP.TupleEqual("false"))) != 0)
                {
                    hv_Bold_COPY_INP_TMP = "medium";
                }
                else
                {
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("true"))) != 0)
                {
                    if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("times"))) != 0)
                    {
                        hv_Slant_COPY_INP_TMP = "i";
                    }
                    else
                    {
                        hv_Slant_COPY_INP_TMP = "o";
                    }
                }
                else if ((int)(new HTuple(hv_Slant_COPY_INP_TMP.TupleEqual("false"))) != 0)
                {
                    hv_Slant_COPY_INP_TMP = "r";
                }
                else
                {
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                try
                {
                    HOperatorSet.SetFont(hv_WindowHandle, ((((((("-adobe-" + hv_Font_COPY_INP_TMP) + "-") + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    if ((int)((new HTuple(((hv_OS.TupleSubstr(0, 4))).TupleEqual("Linux"))).TupleAnd(
                        new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("courier")))) != 0)
                    {
                        HOperatorSet.QueryFont(hv_WindowHandle, out hv_Fonts);
                        hv_FontSelRegexp = (("^-[^-]*-[^-]*[Cc]ourier[^-]*-" + hv_Bold_COPY_INP_TMP) + "-") + hv_Slant_COPY_INP_TMP;
                        hv_FontsCourier = ((hv_Fonts.TupleRegexpSelect(hv_FontSelRegexp))).TupleRegexpMatch(
                            hv_FontSelRegexp);
                        if ((int)(new HTuple((new HTuple(hv_FontsCourier.TupleLength())).TupleEqual(
                            0))) != 0)
                        {
                            hv_Exception = "Wrong font name";
                            //throw (Exception)
                        }
                        else
                        {
                            try
                            {
                                HOperatorSet.SetFont(hv_WindowHandle, (((hv_FontsCourier.TupleSelect(
                                    0)) + "-normal-*-") + hv_Size_COPY_INP_TMP) + "-*-*-*-*-*-*-*");
                            }
                            // catch (Exception) 
                            catch (HalconException HDevExpDefaultException2)
                            {
                                HDevExpDefaultException2.ToHTuple(out hv_Exception);
                                //throw (Exception)
                            }
                        }
                    }
                    //throw (Exception)
                }
            }
            // dev_set_preferences(...); only in hdevelop

            return;
        }

        // Chapter: Develop
        // Short Description: Switch dev_update_pc, dev_update_var and dev_update_window to 'off'. 
        public void dev_update_off()
        {

            // Initialize local and output iconic variables 
            //This procedure sets different update settings to 'off'.
            //This is useful to get the best performance and reduce overhead.
            //
            // dev_update_pc(...); only in hdevelop
            // dev_update_var(...); only in hdevelop
            // dev_update_window(...); only in hdevelop

            return;
        }

        // Chapter: Graphics / Text
        // Short Description: This procedure writes a text message. 
        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Red = null, hv_Green = null, hv_Blue = null;
            HTuple hv_Row1Part = null, hv_Column1Part = null, hv_Row2Part = null;
            HTuple hv_Column2Part = null, hv_RowWin = null, hv_ColumnWin = null;
            HTuple hv_WidthWin = null, hv_HeightWin = null, hv_MaxAscent = null;
            HTuple hv_MaxDescent = null, hv_MaxWidth = null, hv_MaxHeight = null;
            HTuple hv_R1 = new HTuple(), hv_C1 = new HTuple(), hv_FactorRow = new HTuple();
            HTuple hv_FactorColumn = new HTuple(), hv_UseShadow = null;
            HTuple hv_ShadowColor = null, hv_Exception = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Ascent = new HTuple(), hv_Descent = new HTuple();
            HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_FrameHeight = new HTuple();
            HTuple hv_FrameWidth = new HTuple(), hv_R2 = new HTuple();
            HTuple hv_C2 = new HTuple(), hv_DrawMode = new HTuple();
            HTuple hv_CurrentColor = new HTuple();
            HTuple hv_Box_COPY_INP_TMP = hv_Box.Clone();
            HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
            HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
            HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
            HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Column: The column coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for each new textline.
            //Box: If Box[0] is set to 'true', the text is written within an orange box.
            //     If set to' false', no box is displayed.
            //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
            //       the text is written in a box of that color.
            //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
            //       'true' -> display a shadow in a default color
            //       'false' -> display no shadow (same as if no second value is given)
            //       otherwise -> use given string as color string for the shadow color
            //
            //Prepare window
            HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part, out hv_Row2Part,
                out hv_Column2Part);
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
                out hv_WidthWin, out hv_HeightWin);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
            //
            //default settings
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple()))) != 0)
            {
                hv_Color_COPY_INP_TMP = "";
            }
            //
            hv_String_COPY_INP_TMP = ((("" + hv_String_COPY_INP_TMP) + "")).TupleSplit("\n");
            //
            //Estimate extentions of text depending on font size.
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                out hv_MaxWidth, out hv_MaxHeight);
            if ((int)(new HTuple(hv_CoordSystem.TupleEqual("window"))) != 0)
            {
                hv_R1 = hv_Row_COPY_INP_TMP.Clone();
                hv_C1 = hv_Column_COPY_INP_TMP.Clone();
            }
            else
            {
                //Transform image to window coordinates
                hv_FactorRow = (1.0 * hv_HeightWin) / ((hv_Row2Part - hv_Row1Part) + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = ((hv_Row_COPY_INP_TMP - hv_Row1Part) + 0.5) * hv_FactorRow;
                hv_C1 = ((hv_Column_COPY_INP_TMP - hv_Column1Part) + 0.5) * hv_FactorColumn;
            }
            //
            //Display text box depending on text size
            hv_UseShadow = 1;
            hv_ShadowColor = "gray";
            if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleEqual("true"))) != 0)
            {
                if (hv_Box_COPY_INP_TMP == null)
                    hv_Box_COPY_INP_TMP = new HTuple();
                hv_Box_COPY_INP_TMP[0] = "#fce9d4";
                hv_ShadowColor = "#f28d26";
            }
            if ((int)(new HTuple((new HTuple(hv_Box_COPY_INP_TMP.TupleLength())).TupleGreater(
                1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual("true"))) != 0)
                {
                    //Use default ShadowColor set above
                }
                else if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(1))).TupleEqual(
                    "false"))) != 0)
                {
                    hv_UseShadow = 0;
                }
                else
                {
                    hv_ShadowColor = hv_Box_COPY_INP_TMP[1];
                    //Valid color?
                    try
                    {
                        HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(
                            1));
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        hv_Exception = "Wrong value of control parameter Box[1] (must be a 'true', 'false', or a valid color string)";
                        throw new HalconException(hv_Exception);
                    }
                }
            }
            if ((int)(new HTuple(((hv_Box_COPY_INP_TMP.TupleSelect(0))).TupleNotEqual("false"))) != 0)
            {
                //Valid color?
                try
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_Exception = "Wrong value of control parameter Box[0] (must be a 'true', 'false', or a valid color string)";
                    throw new HalconException(hv_Exception);
                }
                //Calculate box extents
                hv_String_COPY_INP_TMP = (" " + hv_String_COPY_INP_TMP) + " ";
                hv_Width = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                    hv_Width = hv_Width.TupleConcat(hv_W);
                }
                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ));
                hv_FrameWidth = (((new HTuple(0)).TupleConcat(hv_Width))).TupleMax();
                hv_R2 = hv_R1 + hv_FrameHeight;
                hv_C2 = hv_C1 + hv_FrameWidth;
                //Display rectangles
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                //Set shadow color
                HOperatorSet.SetColor(hv_WindowHandle, hv_ShadowColor);
                if ((int)(hv_UseShadow) != 0)
                {
                    HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 1, hv_C1 + 1, hv_R2 + 1, hv_C2 + 1);
                }
                //Set box color
                HOperatorSet.SetColor(hv_WindowHandle, hv_Box_COPY_INP_TMP.TupleSelect(0));
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            //Write text.
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % (new HTuple(hv_Color_COPY_INP_TMP.TupleLength()
                    )));
                if ((int)((new HTuple(hv_CurrentColor.TupleNotEqual(""))).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                    "auto")))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                }
                hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
                HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
                HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                    hv_Index));
            }
            //Reset changed window settings
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);

            return;
        }
        #endregion

    }
}

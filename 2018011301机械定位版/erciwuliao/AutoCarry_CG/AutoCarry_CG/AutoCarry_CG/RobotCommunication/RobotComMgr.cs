using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCommunication
{
    public class RobotComMgr
    {
        public List<RobotCom> RobotComList = new List<RobotCom>();

        public RobotComMgr()
        {
            
        }
        ~RobotComMgr()
        { }

        public void AddRobotCom(string name)
        {
            RobotCom myBarcode = new RobotCom(name);
            RobotComList.Add(myBarcode);
        }

        public void LoadSettings(string FileName)
        {
            for (int i = 0; i < RobotComList.Count; i++)
                RobotComList[i].LoadSettings(FileName);
        }

        public void SaveSettings(string FileName)
        {
            for (int i = 0; i < RobotComList.Count; i++)
                RobotComList[i].SaveSettings(FileName);
        }

        public void ShowSettings()
        {
            WinRobotCom myWin = new WinRobotCom(this);
            myWin.ShowDialog();
        }

        public void Disable()
        {   
        }

    }
}

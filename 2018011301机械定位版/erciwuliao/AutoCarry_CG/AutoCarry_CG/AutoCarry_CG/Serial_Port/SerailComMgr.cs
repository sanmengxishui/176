using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Port
{
    public class SerailComMgr
    {
        public List<SerailCom> SerailComList = new List<SerailCom>();

        string SerailComSettingsFilePath = "";
        string SerailComFileName = "SerailCom_Settings.xml";


        public SerailComMgr()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            SerailComSettingsFilePath = exePath + SerailComFileName;
        }
        ~SerailComMgr()
        {

        }
        public bool SerailComListAdd(string name)
        {
            SerailCom SCom = new SerailCom(name);
            SerailComList.Add(SCom);
            return true;
        }

        public bool SerailComList_Save()
        {
            string fileName = SerailComSettingsFilePath;

            for (int i = 0; i < SerailComList.Count; i++)
            {
                SerailComList[i].SerailCom_Save(fileName);
            }
            return true;
        }

        public bool SerailComList_Load()
        {
            string fileName = SerailComSettingsFilePath;

            for (int i = 0; i < SerailComList.Count; i++)
            {
                SerailComList[i].SerailCom_Load(fileName);
                SerailComList[i].Connect();
            }
            return true;
        }

        public void SP_Win_Show()
        {
            SP_Win Win = new SP_Win(this);
            Win.ShowDialog();
        }




    }
}

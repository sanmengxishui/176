using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A162Client
{
    public class A162ClientComMgr
    {
        public List<A162ClientCom> A162ClientComList = new List<A162ClientCom>();

        public A162ClientComMgr()
        {
            
        }
        ~A162ClientComMgr()
        { }

        public void AddRobotCom(string name)
        {
            A162ClientCom myBarcode = new A162ClientCom(name);
            A162ClientComList.Add(myBarcode);
        }

        public void LoadSettings(string FileName)
        {
            for (int i = 0; i < A162ClientComList.Count; i++)
                A162ClientComList[i].LoadSettings(FileName);
        }

        public void SaveSettings(string FileName)
        {
            for (int i = 0; i < A162ClientComList.Count; i++)
                A162ClientComList[i].SaveSettings(FileName);
        }

        public void ShowSettings()
        {
            A162Client_Win myWin = new A162Client_Win(this);
            myWin.ShowDialog();
        }

        public void Disable()
        {   
        }

    }
}

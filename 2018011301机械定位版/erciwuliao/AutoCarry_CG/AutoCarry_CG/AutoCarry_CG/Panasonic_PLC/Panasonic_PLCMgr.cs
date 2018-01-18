using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panasonic_Communication
{
    public class Panasonic_PLCMgr
    {
        public List<Panasonic_PLC> PanaList = new List<Panasonic_PLC>();

        string PanaPLCSettingsFilePath = "";
        string PanaPLCetFileName = "PanaPLC_Settings.xml";


        public Panasonic_PLCMgr()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            PanaPLCSettingsFilePath = exePath + PanaPLCetFileName;
        }
        ~Panasonic_PLCMgr()
        { 
        
        }
        public bool PanaAdd(string name)
        {
            Panasonic_PLC Pana = new Panasonic_PLC(name);
            PanaList.Add(Pana);
            return true;
        }

        public bool PanaList_Save()
        {
            string fileName = PanaPLCSettingsFilePath;

            for (int i = 0; i < PanaList.Count; i++)
            {
                PanaList[i].Pana_Save(fileName);
            }
            return true;
        }

        public bool PanaList_Load()
        {
            string fileName = PanaPLCSettingsFilePath;

            for (int i = 0; i < PanaList.Count; i++)
            {
                PanaList[i].Pana_Load(fileName);
                PanaList[i].Connect();
            }
            return true;
        }

        public void WinPana_Show()
        {
            WinPanasonic_PLC Win = new WinPanasonic_PLC(this);
            Win.ShowDialog();
        }









    }
}

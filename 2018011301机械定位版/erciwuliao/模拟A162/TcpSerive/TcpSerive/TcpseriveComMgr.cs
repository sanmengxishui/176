using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpSerive
{
    public class TcpseriveComMgr
    {
        public List<TcpseriveCom> TcpserList = new List<TcpseriveCom>();
        public TcpseriveComMgr()
        { 
            
        }
        ~TcpseriveComMgr()
        { 
        
        }

        public void TcpseriveComMgr_Add(string name)
        {
            TcpseriveCom tcpserive = new TcpseriveCom(name);
            TcpserList.Add(tcpserive);
        }

        public void TcpseriveComMgr_Load(string file)
        {
            foreach (TcpseriveCom tcp in TcpserList)
            {
                tcp.LoadSettings(file);
            }
        }

        public void TcpseriveComMgr_Save(string file)
        {
            foreach (TcpseriveCom tcp in TcpserList)
            {
                tcp.SaveSettings(file);
            }
        }

        public void TcpseriveComMgr_Win()
        {
            Tcpserive tcpWin = new Tcpserive(this);
            tcpWin.ShowDialog();
        }






    }
}

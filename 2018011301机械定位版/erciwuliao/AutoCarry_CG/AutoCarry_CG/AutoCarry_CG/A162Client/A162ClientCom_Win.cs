using JPTCG.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A162Client
{
    public partial class A162Client_Win : Form
    {
        A162ClientComMgr myMgr;

        public A162Client_Win(A162ClientComMgr mgr)
        {
            InitializeComponent();
            myMgr = mgr;
        }

        private void A162ClientCom_Load(object sender, EventArgs e)
        {
            ModLB.Items.Clear();
            for (int i = 0; i < myMgr.A162ClientComList.Count; i++)
                ModLB.Items.Add(myMgr.A162ClientComList[i].Name);
            if (ModLB.Items.Count != 0)
            {
                ModLB.SelectedIndex = 0;
                UpdateUI(0);
            }
        }

        private void UpdateUI(int BarIdx)
        {
            ip1TB.Text = myMgr.A162ClientComList[BarIdx].IP.ToString();

            cmdTB.Text = myMgr.A162ClientComList[BarIdx].Port.ToString();
        }

        private void AddMsg(string msg)
        {
            if (msgLB.InvokeRequired)
            {
                msgLB.Invoke(new Action(() =>
                    {
                        msgLB.Items.Add(msg);
                    }));
            }
            else
            {
                msgLB.Items.Add(msg);
            }
        }
        private void A162ClientCom_FormClosing(object sender, FormClosingEventArgs e)
        {
            myMgr.SaveSettings(Para.MchConfigFileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ModLB.Items.Count == 0)
                return;

            int Idx = ModLB.SelectedIndex;

            myMgr.A162ClientComList[Idx].IP = ip1TB.Text;
            myMgr.A162ClientComList[Idx].Port = int.Parse(cmdTB.Text);

            //myMgr.barcodeList[BarIdx].Connect(myMgr.barcodeList[BarIdx].IP, myMgr.barcodeList[BarIdx].Port);
            if (!myMgr.A162ClientComList[Idx].Connect(myMgr.A162ClientComList[Idx].IP, myMgr.A162ClientComList[Idx].Port))
                msgLB.Items.Add(myMgr.A162ClientComList[Idx].Name + " Failed to connect.");
            else
                msgLB.Items.Add(myMgr.A162ClientComList[Idx].Name + " Connected.");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ModLB.Items.Count == 0)
                return;

            int BarIdx = ModLB.SelectedIndex;

            myMgr.A162ClientComList[BarIdx].Disconnect();
            msgLB.Items.Add(myMgr.A162ClientComList[BarIdx].Name + " Disconected.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (ModLB.Items.Count == 0)
                return;

            int Idx = ModLB.SelectedIndex;

            if (!myMgr.A162ClientComList[Idx].IsConnected)
                msgLB.Items.Add(myMgr.A162ClientComList[Idx].Name + " Is Not Connected.");

            string restr = myMgr.A162ClientComList[Idx].Read(DateTime.Now.ToString("HH:mm:ss") + "From:A170");
            if (restr == "")
                msgLB.Items.Add(myMgr.A162ClientComList[Idx].Name + " No  Readed.");
            else
                msgLB.Items.Add(myMgr.A162ClientComList[Idx].Name + ">>" + restr);

        }

        private void ModLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI(ModLB.SelectedIndex);
        }

        private void msgLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(msgLB.Text);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            msgLB.Items.Clear();
        }

        



    }
}

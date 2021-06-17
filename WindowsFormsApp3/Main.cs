using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace Tiktok
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        public int doneFileCount = 0;
        public int totalFileCount = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            // 3 nghia la lay 3 video moi nhat
            Thread reupThread = new Thread(new ParameterizedThreadStart(ReupTiktokTQ));
            reupThread.Start(numericUpDown1.Value.ToString()+"|"+txtLink.Text.ToString());
        }

        private void ReupTiktokTQ (object param)
        {
            string[] settings =  param.ToString().Split('|');
            if (settings.Length != 2)
            {
                Console.WriteLine(param.ToString() + " bị sai định dạng số lượng|link");
            }
            try
            {
                ReupTiktokTQ reup = new ReupTiktokTQ(settings[1].ToString(), int.Parse(settings[0]));
                reup.Run();
                MessageBox.Show("Done, check you folder: "+ Directory.GetCurrentDirectory() + @"\Videos");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}

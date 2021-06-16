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
            Thread reupThread = new Thread(new ParameterizedThreadStart(ReupTiktokTQ));
            reupThread.Start(txtLink.Text.ToString());
        }

        private void ReupTiktokTQ (object channelLink)
        {
            try
            {
                ReupTiktokTQ reup = new ReupTiktokTQ(channelLink.ToString());
                reup.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}

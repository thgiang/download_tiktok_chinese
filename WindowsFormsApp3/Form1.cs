﻿using RestSharp;
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

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int doneFileCount = 0;
        public int totalFileCount = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            this.doneFileCount = 0;

            if (String.IsNullOrEmpty(txtLink.Text))
            {
                MessageBox.Show("Please input tiktok channel link. Ex: https://v.douyin.com/eHEnfxc/");
                return;
            }

            string realUrl = RedirectPath(txtLink.Text);
            if (String.IsNullOrEmpty(realUrl))
            {
                MessageBox.Show("Something wrong when get real URL of your link");
                return;
            }
            else
            {
                Uri myUri = new Uri(realUrl);
                string secUid = HttpUtility.ParseQueryString(myUri.Query).Get("sec_uid");
                if (String.IsNullOrEmpty(realUrl))
                {
                    MessageBox.Show("Cannot find sec_uid from real URL");
                    return;
                }
                else
                {
                    btnDownload.Text = "Getting list...";
                    btnDownload.Enabled = false;

                    VideoList result = new VideoList();
                    List<string> allVideos = new List<string>();
                    long maxCursor = 0;
                    do
                    {
                        result = getVideoUrls(secUid, maxCursor);
                        foreach (var video in result.AwemeList)
                        {
                            try
                            {
                                allVideos.Add(video.Video.PlayAddr.UrlList[0].ToString());
                            } catch 
                            {
                                continue;
                            }
                            
                        }
                        maxCursor = result.MaxCursor;
                    } while (result.HasMore == true);

                    // Tao folder moi
                    string folderPath = @"C:\Users\Admin\source\repos\WindowsFormsApp3\WindowsFormsApp3\bin\Debug\Videos\" + secUid;
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Echo link
                    this.totalFileCount = allVideos.Count;
                    foreach (string videoUrl in allVideos)
                    {
                        DownloadFile(videoUrl, folderPath);
                        //Thread downloadThread = new Thread(() => DownloadFile(videoUrl, folderPath));
                        //downloadThread.Start();
                    }

                    // Reset btn
                    btnDownload.Text = "Download";
                    btnDownload.Enabled = true;


                }
            }

        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Invoke(new Action(() => {
                progressBar1.Value = e.ProgressPercentage;
            }));
        }

        public void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.doneFileCount++;
            this.Invoke(new Action(() => {
                label2.Text = "Downloading " + doneFileCount.ToString() + "/" + totalFileCount.ToString() + " video(s)";
            }));
        }

        public bool DownloadFile(string url, string folderPath)
        {
            string filename = RandomString(10) + ".mp4";
            while (File.Exists(folderPath + @"\" + filename))
            {
                filename = RandomString(10) + ".mp4";
            }

            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadFileAsync(
                    new System.Uri(url),
                    folderPath + @"\" + filename
                );
            }
            return true;
        }


        public string RedirectPath(string url)
        {
            StringBuilder sb = new StringBuilder();
            string location = string.Copy(url);
            while (!string.IsNullOrWhiteSpace(location))
            {
                sb.AppendLine(location); // you can also use 'Append'
                HttpWebRequest request = HttpWebRequest.CreateHttp(location);
                request.AllowAutoRedirect = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    location = response.GetResponseHeader("Location");
                }
            }
            return sb.ToString();
        }

        public dynamic getVideoUrls(string secUid, long maxCursor)
        {
            try
            {
                var client = new RestClient("https://www.iesdouyin.com/web/api/v2/aweme/post/?sec_uid=" + secUid + "&count=36&max_cursor=" + maxCursor + "&aid=1128&_signature=xX5OIQAApdEdWiVTpkBHycV-Tj&dytk=");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("authority", "www.iesdouyin.com");
                request.AddHeader("sec-ch-ua", "\" Not;A Brand\";v=\"99\", \"Google Chrome\";v=\"91\", \"Chromium\";v=\"91\"");
                request.AddHeader("accept", "application/json");
                request.AddHeader("x-requested-with", "XMLHttpRequest");
                request.AddHeader("sec-ch-ua-mobile", "?0");
                client.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.77 Safari/537.36";
                request.AddHeader("sec-fetch-site", "same-origin");
                request.AddHeader("sec-fetch-mode", "cors");
                request.AddHeader("sec-fetch-dest", "empty");
                request.AddHeader("referer", "https://www.iesdouyin.com/share/user/2453725739491389?u_code=4k279gcm95g&sec_uid=MS4wLjABAAAAAjbu9fOUwa2avjqLWBVH6OTwkYnHBV2O-5BgM3Cx2H07pGddbSh00risKRRWey-A&did=MS4wLjABAAAA_luHKKI_ChTUwxzlduNnHmmCutkbuQ0g1XqxMkEKqz0&iid=MS4wLjABAAAAX4sbL8HmnKYYIH3x3-OM3CL9pxfDi0MTBKphNFDMa3Y&with_sec_did=1&app=aweme&utm_campaign=client_share&utm_medium=ios&tt_from=copy&utm_source=copy");
                request.AddHeader("accept-language", "vi-VN,vi;q=0.9,en-US;q=0.8,en;q=0.7,zh-CN;q=0.6,zh;q=0.5");
                request.AddHeader("cookie", "_tea_utm_cache_1243={%22utm_source%22:%22copy%22%2C%22utm_medium%22:%22ios%22%2C%22utm_campaign%22:%22client_share%22}");
                IRestResponse response = client.Execute(request);

                VideoList videoList = JsonConvert.DeserializeObject<VideoList>(response.Content);
                return videoList;
            }
            catch
            {
                return new VideoList();
            }

        }
    }
}

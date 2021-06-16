using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Tiktok
{
    class ReupTiktokTQ
    {
        int limitFile = 50, totalFileCount = 0, doneFileCount = 0;
        string channelLink = "";
        Form loggerForm;
        RichTextBox richTextLog;

        public ReupTiktokTQ (string channelLink, int limitFile = 50)
        {
            this.limitFile = limitFile;
            this.channelLink = channelLink;

            loggerForm = new Form();
            loggerForm.Text = "Hello world";
            loggerForm.Controls.Add(new ProgressBar() { Name = "progressBar1" });
            loggerForm.Controls.Add(new Label() { Name = "label" });
            richTextLog = new RichTextBox() { Name = "richTextLog" };
            loggerForm.Controls.Add(richTextLog);
            loggerForm.Show();
            loggerForm.Update();
        }
        public void Run()
        {
            WriteLog("Bat dau chay");

           
            this.DownloadLatestVideos();
            this.ReupVideos();
        }

        private void WriteLog(string content)
        {

        }

        private void ReupVideos()
        {

        }

        private int DownloadLatestVideos()
        {

            this.doneFileCount = 0;

            string realUrl = RedirectPath(channelLink);
            if (String.IsNullOrEmpty(realUrl))
            {
                return 404;
            }
            else
            {
                Uri myUri = new Uri(realUrl);
                string secUid = HttpUtility.ParseQueryString(myUri.Query).Get("sec_uid");
                if (String.IsNullOrEmpty(realUrl))
                {
                    MessageBox.Show("Cannot find sec_uid from real URL");
                    return 403;
                }
                else
                {
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
                            }
                            catch
                            {
                                continue;
                            }

                        }
                        maxCursor = result.MaxCursor;
                    } while (result.HasMore == true && allVideos.Count < limitFile);

                    // Tao folder moi
                    string folderPath = Directory.GetCurrentDirectory() + @"\Videos\" + secUid;
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Echo link
                    this.totalFileCount = allVideos.Count;
                    foreach (string videoUrl in allVideos)
                    {
                        DownloadFile(videoUrl, folderPath);
                    }
                    return 1;
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
            loggerForm.Invoke(new Action(() => {
               // loggerForm.progressBar1.Value = e.ProgressPercentage;
            }));
        }

        public void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.doneFileCount++;
            loggerForm.Invoke(new Action(() => {
               // loggerForm.label.Text = "Downloading " + doneFileCount.ToString() + "/" + totalFileCount.ToString() + " video(s)";
            }));
        }

        private bool DownloadFile(string url, string folderPath)
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


        private string RedirectPath(string url)
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

        private dynamic getVideoUrls(string secUid, long maxCursor)
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

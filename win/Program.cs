// WinChatty Notifier
// Copyright (c) 2014 Brian Luft
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Newtonsoft.Json.Linq;
using Notifier.Properties;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Notifier
{
   public static class Program
   {
      private sealed class NotifierContext : ApplicationContext
      {
         private readonly NotifyIcon _NotifyIcon = new NotifyIcon();
         private readonly Thread _Thread;
         private readonly SynchronizationContext _SyncContext = new WindowsFormsSynchronizationContext();
         private readonly string _ClientId = SetupClientId();
         private readonly OptionsForm _OptionsForm = new OptionsForm();
         
         // These are set when the bubble appears so that we have it when the user later clicks the bubble.
         private int _PostId;
         private int _ThreadId;

         public NotifierContext()
         {
            try
            {
               RegisterNotifierClient();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "WinChatty Notifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
               Exit();
               return;
            }

            SetupNotifyIcon();

            _Thread = new Thread(new ThreadStart(() =>
            {
               bool first = true;

               while (true)
               {
                  if (!first)
                     Thread.Sleep(5000);
                  first = false;

                  string json = null;
                  var query = new NameValueCollection();
                  query.Add("clientId", _ClientId);
                  using (var webClient = new WinChattyWebClient())
                  {
                     webClient.QueryString = query;
                     
                     try
                     {
                        // Use alternate hostname to avoid concurrent connection limit.
                        string url = "http://notifications.winchatty.com/v2/notifications/waitForNotification";
                        var bytes = webClient.UploadValues(url, query);
                        json = Encoding.UTF8.GetString(bytes);
                     }
                     catch (WebException)
                     {
                        continue;
                     }
                  }

                  var response = JObject.Parse(json);
                  JToken errorToken;
                  if (response.TryGetValue("error", out errorToken))
                  {
                     Invoke(() =>
                     {
                        string code = (string)response["code"];
                        if (code == "ERR_CLIENT_NOT_ASSOCIATED")
                        {
                           OpenNotificationsSetup();
                           MessageBox.Show(
                              "You must re-launch WinChatty Notifier after associating it with a Shacknews account.",
                              "WinChatty Notifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                           string errorMessage = (string)response["message"];
                           MessageBox.Show(errorMessage, "WinChatty Notifier", MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
                        }
                        Exit();
                     });
                     return;
                  }

                  var message = response["messages"].FirstOrDefault();
                  if (message != null)
                  {
                     string subject = (string)message["subject"];
                     string body = (string)message["body"];
                     int id = (int)message["postId"];
                     int threadId = (int)message["threadId"];

                     Invoke(() =>
                     {
                        _PostId = id;
                        _ThreadId = threadId;
                        _NotifyIcon.ShowBalloonTip(1000 * Settings.Default.Duration, subject, body, ToolTipIcon.None);
                     });
                  }
               }
            }));
            _Thread.Start();
         }

         private void OpenNotificationsSetup()
         {
            Process.Start("https://winchatty.com/v2/notifications/ui/login?clientId=" + _ClientId);
         }

         private void SetupNotifyIcon()
         {
            _NotifyIcon.Text = "WinChatty Notifier";
            _NotifyIcon.Icon = Resources.NotificationIcon;
            _NotifyIcon.ContextMenu = new ContextMenu(
               new[] 
               {
                  new MenuItem("&Setup notifications…", (sender, e) =>
                  {
                     OpenNotificationsSetup();
                  }),
                  new MenuItem("&Options…", (sender, e) =>
                  {
                     BeginInvoke(() =>
                     {
                        _OptionsForm.Show();
                        _OptionsForm.BringToFront();
                     });
                  }),
                  new MenuItem("&About WinChatty Notifier", (sender, e) =>
                  {
                     MessageBox.Show("WinChatty Notifier " + Application.ProductVersion + "\n© 2014 Brian Luft", 
                        "About WinChatty Notifier", MessageBoxButtons.OK, MessageBoxIcon.Information);
                  }),
                  new MenuItem("E&xit", (sender, e) =>
                  {
                     Exit();
                  })
               });
            _NotifyIcon.BalloonTipClicked += (sender, e) => OpenPost();
            _NotifyIcon.MouseClick += (sender, e) =>
            {
               if (e.Button == MouseButtons.Left)
                  OpenPost();
            };
            _NotifyIcon.Visible = true;
         }

         private void OpenPost()
         {
            if (_PostId <= 0)
               return;

            if (Settings.Default.LinkHandler == "Lamp")
            {
               try
               {
                  LampUtil.OpenPost(_ThreadId, _PostId);
               }
               catch (Exception ex)
               {
                  MessageBox.Show(ex.Message, "WinChatty Notifier",
                     MessageBoxButtons.OK, MessageBoxIcon.Error);
               }
            }
            else
            {
               Process.Start("http://www.shacknews.com/chatty?id=" + _PostId.ToString() +
                  "#item_" + _PostId.ToString());
            }
         }

         private static string SetupClientId()
         {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string settingPath = Path.Combine(appData, "WinChattyNotifier");
            Directory.CreateDirectory(settingPath);
            string clientIdFilePath = Path.Combine(settingPath, "ClientId");
            string clientId;
            if (File.Exists(clientIdFilePath))
               clientId = File.ReadAllText(clientIdFilePath).Trim();
            else
               clientId = Guid.NewGuid().ToString();

            if (clientId.Length > 36)
               clientId = clientId.Substring(0, 36);

            File.WriteAllText(clientIdFilePath, clientId);
            return clientId;
         }

         private void Invoke(Action callback)
         {
            _SyncContext.Send(x => callback(), null);
         }

         private void BeginInvoke(Action callback)
         {
            _SyncContext.Post(x => callback(), null);
         }

         private void Exit()
         {
            try
            {
               _NotifyIcon.Visible = false;
            }
            catch (Exception) { }
            Process.GetCurrentProcess().Kill();
         }

         private void RegisterNotifierClient()
         {
            var query = new NameValueCollection();
            query.Add("id", _ClientId);
            string computer = SystemInformation.ComputerName.ToUpper();
            query.Add("name", computer + " (Windows)");
            byte[] bytes;
            using (var webClient = new WinChattyWebClient())
               bytes = webClient.UploadValues(
                  "https://winchatty.com/v2/notifications/registerNotifierClient", query);
            var response = JObject.Parse(Encoding.UTF8.GetString(bytes));
            JToken errorToken;
            if (response.TryGetValue("error", out errorToken))
               throw new Exception((string)response["message"]);
            JToken resultToken;
            if (!response.TryGetValue("result", out resultToken) || (string)response["result"] != "success")
               throw new Exception("Unexpected server response.");
         }
      }

      private sealed class WinChattyWebClient : WebClient
      {
         protected override WebRequest GetWebRequest(Uri address)
         {
            var request = base.GetWebRequest(address);

            var httpRequest = request as HttpWebRequest;
            if (httpRequest != null)
            {
               httpRequest.UserAgent = "WinChatty Notifier (Win) " + Application.ProductVersion;
               httpRequest.Timeout = 60 * 11 * 1000;
            }

            return request;
         }
      }


      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      public static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         using (var mutex = new Mutex(false, "Global\\8CD3CDCC-723B-4E7C-9BC1-0251F79F08D0"))
         {
            bool acquired = false;

            try
            {
               acquired = mutex.WaitOne(0, false);
            }
            catch (AbandonedMutexException)
            {
               acquired = true;
            }
            
            if (acquired)
               Application.Run(new NotifierContext());
         }
      }
   }
}

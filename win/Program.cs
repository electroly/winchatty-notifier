/* WinChatty Notifier 
 * Copyright (c) 2014, Brian Luft.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice, 
 *   this list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice, 
 *   this list of conditions and the following disclaimer in the documentation 
 *   and/or other materials provided with the distribution.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE. 
 */

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
            _NotifyIcon.BalloonTipClicked += (sender, e) =>
            {
               if (Settings.Default.LinkHandler == "Lamp")
               {
                  // Try to find Lamp.
                  var possibleFilePaths = new[]
                  {
                     @"C:\Program Files (x86)\Lamp\Lamp.exe",
                     @"C:\Program Files\Lamp\Lamp.exe"
                  };
                  string lampFilePath = possibleFilePaths.FirstOrDefault(x => File.Exists(x));
                  if (lampFilePath == null)
                  {
                     MessageBox.Show("Unable to find Lamp.exe on disk.", "WinChatty Notifier", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                  }
                  else
                  {
                     Process.Start(lampFilePath, string.Format("{0} {1}", _ThreadId, _PostId));
                  }
               }
               else
               {
                  Process.Start("http://www.shacknews.com/chatty?id=" + _PostId.ToString() + "#item_" + _PostId.ToString());
               }
            };
            _NotifyIcon.Visible = true;
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
         Application.Run(new NotifierContext());
      }
   }
}

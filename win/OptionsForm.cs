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

using Notifier.Properties;
using System.Windows.Forms;

namespace Notifier
{
   public partial class OptionsForm : Form
   {
      public OptionsForm()
      {
         InitializeComponent();
         Reset();

         _OKBtn.Click += (sender, e) => OnOkClicked();
         _CancelBtn.Click += (sender, e) => Close();
      }

      private void Reset()
      {
         switch (Settings.Default.LinkHandler)
         {
            case "WebBrowser":
               _LinkHandlerCmb.SelectedIndex = 0;
               break;

            case "Lamp":
               _LinkHandlerCmb.SelectedIndex = 1;
               break;
         }
         _DurationUpd.Value = Settings.Default.Duration;
      }

      private void OnOkClicked()
      {
         switch (_LinkHandlerCmb.SelectedIndex)
         {
            case 0:
               Settings.Default.LinkHandler = "WebBrowser";
               break;
            case 1:
               Settings.Default.LinkHandler = "Lamp";
               break;
         }
         Settings.Default.Duration = (int)_DurationUpd.Value;
         Settings.Default.Save();
         Close();
      }

      protected override void OnFormClosing(FormClosingEventArgs e)
      {
         if (e.CloseReason == CloseReason.UserClosing)
         {
            Reset();
            _OKBtn.Focus();
            Hide();
            e.Cancel = true;
         }
         else
         {
            base.OnFormClosing(e);
         }
      }
   }
}

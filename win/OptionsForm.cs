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

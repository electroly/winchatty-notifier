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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Notifier
{
   public static class LampUtil
   {
      [DllImport("user32.dll", SetLastError = true)]
      private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool IsIconic(IntPtr hWnd);

      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool SetForegroundWindow(IntPtr hWnd);

      [return: MarshalAs(UnmanagedType.Bool)]
      [DllImport("user32.dll", SetLastError = true)]
      private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

      private const uint WM_USER = 0x0400;
      private const int SW_RESTORE = 9;

      private const uint LAMP_WM_OPEN_POST_ID = (WM_USER + 119);
      private const string LAMP_WINDOW_CLASS = "Lamp - Shack Client";

      public static void OpenPost(int threadId, int postId)
      {
         // If Lamp is already running, then talk to it directly by sending a Windows message.
         // Otherwise, launch Lamp and pass the IDs on the command line.
         
         var hWnd = FindWindow(LAMP_WINDOW_CLASS, null);
         if (hWnd != IntPtr.Zero)
         {
            if (IsIconic(hWnd))
               ShowWindow(hWnd, SW_RESTORE);

            SetForegroundWindow(hWnd);

            PostMessage(hWnd, LAMP_WM_OPEN_POST_ID, new IntPtr(threadId), new IntPtr(postId)); 
         }
         else
         {
            var possibleFilePaths = new[]
            {
               @"C:\Program Files (x86)\Lamp\Lamp.exe",
               @"C:\Program Files\Lamp\Lamp.exe"
            };
            string lampFilePath = possibleFilePaths.FirstOrDefault(x => File.Exists(x));
            if (lampFilePath == null)
               throw new Exception("Unable to find Lamp.exe on disk.");
            else
               Process.Start(lampFilePath, string.Format("{0} {1}", threadId, postId));
         }
      }
   }
}

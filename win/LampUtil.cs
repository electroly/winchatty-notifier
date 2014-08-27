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

      private const int SW_RESTORE = 9;

      private const string LAMP_WINDOW_CLASS = "Lamp - Shack Client";

      public static void OpenPost(int threadId, int postId)
      {
         // Try to bring Lamp to the front if it's already running.
         var hWnd = FindWindow(LAMP_WINDOW_CLASS, null);
         if (hWnd != IntPtr.Zero)
         {
            if (IsIconic(hWnd))
               ShowWindow(hWnd, SW_RESTORE);

            SetForegroundWindow(hWnd);
         }

         // Try to find Lamp on disk.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DrExt.Win32
{
    public static class WinNT
    {
        #region Win32 API
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AllocConsole();
        #endregion

        /// <summary>
        /// returns true if current process doesn't have console
        /// </summary>
        /// <returns></returns>
        public static bool ConsoleWindowsExist()
        {
            return (GetConsoleWindow() != IntPtr.Zero);
        }

    }
}

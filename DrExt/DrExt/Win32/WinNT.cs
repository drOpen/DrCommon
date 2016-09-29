using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DrExt.Win32
{
    public static class WinNT
    {
        #region The standard device. This parameter can be one of the following values.
        /// <summary>
        /// STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        /// </summary>
        public const int STD_INPUT_HANDLE = -10;
        /// <summary>
        /// The standard output device. Initially, this is the active console screen buffer, CONOUT$.
        /// </summary>
        public const int STD_OUTPUT_HANDLE = -11;
        /// <summary>
        /// The standard error device. Initially, this is the active console screen buffer, CONOUT$.
        /// </summary>
        public const int STD_ERROR_HANDLE = -12;
        #endregion The standard device. This parameter can be one of the following values.

        #region Win32 API
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AllocConsole();

        /// <summary>
        /// Retrieves the current input mode of a console's input buffer or the current output mode of a console screen buffer.
        /// </summary>
        /// <param name="hConsoleHandle">A handle to the console input buffer or the console screen buffer. The handle must have the GENERIC_READ access right. For more information, see Console Buffer Security and Access Rights.</param>
        /// <param name="lpMode">A pointer to a variable that receives the current mode of the specified buffer.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        /// <summary>
        /// Sets the input mode of a console's input buffer or the output mode of a console screen buffer.
        /// </summary>
        /// <param name="hConsoleHandle">A handle to the console input buffer or a console screen buffer. The handle must have the GENERIC_READ access right. For more information, see Console Buffer Security and Access Rights.</param>
        /// <param name="dwMode">The input or output mode to be set. If the hConsoleHandle parameter is an input handle, the mode can be one or more of the following values. When a console is created, all input modes except ENABLE_WINDOW_INPUT are enabled by default.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        /// <summary>
        /// Retrieves a handle to the specified standard device (standard input, standard output, or standard error).
        /// </summary>
        /// <param name="nStdHandle">The standard device. This parameter can be one of the following values. Supports the following params STD_INPUT_HANDLE, STD_OUTPUT_HANDLE, STD_ERROR_HANDLE</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the specified device, or a redirected handle set by a previous call to SetStdHandle. The handle has GENERIC_READ and GENERIC_WRITE access rights, unless the application has used SetStdHandle to set a standard handle with lesser access.
        /// If the function fails, the return value is INVALID_HANDLE_VALUE. To get extended error information, call GetLastError.
        /// If an application does not have associated standard handles, such as a service running on an interactive desktop, and has not redirected them, the return value is NULL.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

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

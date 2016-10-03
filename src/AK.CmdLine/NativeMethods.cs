// Copyright (c) Red Bull Technology Ltd. All rights reserved.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AK.CmdLine
{
    internal static class NativeMethods
    {
        public enum ConsoleCtrlType
        {
            ControlC = 0,
            ControlBreak = 1,
            Close = 2,
            LogOff = 5,
            Shutdown = 6
        }

        public delegate bool ConsoleCtrlHandlerRoutine(ConsoleCtrlType e);

        public static void AddConsoleCtrlHandler(ConsoleCtrlHandlerRoutine handler)
        {
            if (!SetConsoleCtrlHandler(handler, true))
            {
                ThrowLastWin32Error();
            }
        }

        public static void RemoveConsoleCtrlHandler(ConsoleCtrlHandlerRoutine handler)
        {
            if (!SetConsoleCtrlHandler(handler, false))
            {
                ThrowLastWin32Error();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandlerRoutine handler, bool addOrRemove);

        private static void ThrowLastWin32Error()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
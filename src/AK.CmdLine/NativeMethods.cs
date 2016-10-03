﻿// Copyright 2016 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
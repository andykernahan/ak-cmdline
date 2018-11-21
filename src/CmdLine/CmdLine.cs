// Copyright 2016 Andy Kernahan
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

using System;
using System.Threading;

namespace AK.CmdLine
{
    /// <summary>
    /// Provides support for running a single command which requires notification when the command line has received a
    /// request to exit, either by means of CTRL+C, CTRL+BREAK or the user closing the console window.
    /// This class is <see langword="static"/>.
    /// </summary>
    public static class CmdLine
    {
        private static readonly object _syncRoot = new object();
        private static readonly ManualResetEvent _exit;
        private static readonly ManualResetEvent _canExit;
        private static bool _ran;

        static CmdLine()
        {
            _exit = new ManualResetEvent(false);
            _canExit = new ManualResetEvent(true);
            // We employ the exact same method as
            // https://github.com/aspnet/Hosting/blob/master/src/Microsoft.Extensions.Hosting/Internal/ConsoleLifetime.cs
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                _exit.Set();
                _canExit.WaitOne();
            };
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _exit.Set();
            };
        }

        /// <summary>
        /// Runs the command, as represented by the specified <paramref name="action"/>.
        /// <para>Only a single command can be executed during the application's lifetime; it is an error otherwise.</para>
        /// </summary>
        /// <param name="action">The command action, which is supplied the exit wait handle.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The command has been already specified.</exception>
        public static void Run(Action<CmdLineExitWaitHandle> action)
        {
            Guard.NotNull(action, nameof(action));

            lock (_syncRoot)
            {
                if (_ran)
                {
                    throw new InvalidOperationException("The command has already been specified.");
                }
                _ran = true;
            }
            try
            {
                _canExit.Reset();
                action(CmdLineExitWaitHandle.CopyFrom(_exit));
            }
            finally
            {
                _canExit.Set();
            }
        }
    }
}

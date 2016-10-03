// Copyright (c) Red Bull Technology Ltd. All rights reserved.

using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
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
        private static readonly ConsoleCtrlEventHandler _consoleCtrlEventHandler;
        private static bool _ran;

        static CmdLine()
        {
            _exit = new ManualResetEvent(false);
            _canExit = new ManualResetEvent(true);
            _consoleCtrlEventHandler = new ConsoleCtrlEventHandler();
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

            try
            {
                _canExit.Reset();
                lock (_syncRoot)
                {
                    if (_ran)
                    {
                        throw new InvalidOperationException("The command has already been specified.");
                    }
                    _ran = true;
                }
                action(CmdLineExitWaitHandle.CopyFrom(_exit));
            }
            finally
            {
                _canExit.Set();
            }
        }

        private static void OnExitRequested()
        {
            _exit.Set();
            _canExit.WaitOne();
        }

        private sealed class ConsoleCtrlEventHandler : CriticalFinalizerObject
        {
            private readonly NativeMethods.ConsoleCtrlHandlerRoutine _handler = e =>
            {
                switch (e)
                {
                    case NativeMethods.ConsoleCtrlType.ControlC:
                    case NativeMethods.ConsoleCtrlType.ControlBreak:
                    case NativeMethods.ConsoleCtrlType.Close:
                    case NativeMethods.ConsoleCtrlType.LogOff:
                    case NativeMethods.ConsoleCtrlType.Shutdown:
                        OnExitRequested();
                        break;
                    default:
                        Trace.WriteLine($"unknown {e.GetType()}: {e}");
                        break;
                }
                return false;
            };

            public ConsoleCtrlEventHandler()
            {
                NativeMethods.AddConsoleCtrlHandler(_handler);
            }

            ~ConsoleCtrlEventHandler()
            {
                NativeMethods.RemoveConsoleCtrlHandler(_handler);
            }
        }
    }
}
// Copyright (c) Red Bull Technology Ltd. All rights reserved.

using System.Diagnostics;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace AK.CmdLine
{
    /// <summary>
    /// Notifies one or more waiting threads that the command line has received a request to exit, either by means of
    /// CTRL+C, CTRL+BREAK or the user closing the console window. This class cannot be inherited.
    /// </summary>
    public sealed class CmdLineExitWaitHandle : WaitHandle
    {
        internal static CmdLineExitWaitHandle CopyFrom(WaitHandle handle) => new CmdLineExitWaitHandle(handle);

        private CmdLineExitWaitHandle(WaitHandle handle)
        {
            Debug.Assert(handle != null);

            SafeWaitHandle = new SafeWaitHandle(handle.SafeWaitHandle.DangerousGetHandle(), ownsHandle: false);
        }
    }
}
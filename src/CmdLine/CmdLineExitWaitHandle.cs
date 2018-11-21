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
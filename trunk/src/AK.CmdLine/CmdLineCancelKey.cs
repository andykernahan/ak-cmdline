// Copyright 2011 Andy Kernahan
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
    /// A utility class which blocks the current thread until the console cancel keys (ctrl+c) are pressed.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class CmdLineCancelKey : IDisposable
    {
        #region Fields.

        private bool _isDisposed;
        private readonly ManualResetEventSlim _cancelled = new ManualResetEventSlim();

        #endregion

        /// <summary>
        /// Blocks the current thread until the console cancel keys (ctrl+c) are pressed.
        /// </summary>
        /// <returns>An <see cref="System.IDisposable"/> which blocks the current thread until the console
        /// cancel keys (ctrl+c) are pressed.</returns>
        public static IDisposable WaitFor()
        {
            return new CmdLineCancelKey();
        }

        /// <inheritdoc/>        
        public void Dispose()
        {
            if(!_isDisposed)
            {
                _isDisposed = true;
                _cancelled.Dispose();
                Console.CancelKeyPress -= OnConsoleCancelKeyPress;
            }
        }

        #region Private Impl.

        private CmdLineCancelKey()
        {
            Console.CancelKeyPress += OnConsoleCancelKeyPress;
            _cancelled.Wait();
        }

        private void OnConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _cancelled.Set();
        }

        #endregion
    }
}

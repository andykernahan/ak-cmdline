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

using AK.CmdLine.Impl;

namespace AK.CmdLine
{
    public static class TestUtility
    {
        public static readonly ComponentDescriptor ComponentDescriptor =
            new ComponentDescriptor(typeof(object));
        public static readonly MethodDescriptor MethodDescriptor =
            new MethodDescriptor(ComponentDescriptor, typeof(TestUtility).GetMethod("Method"));

        public static void Method()
        {
        }
    }
}

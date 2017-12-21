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
using System.IO;
using AK.CmdLine.Impl.Utility;
using Xunit;

namespace AK.CmdLine.Impl
{
    public class DefaultUsageWriterTest
    {
        [Fact]
        public void ctor_throws_if_component_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DefaultUsageWriter(null, new IndentedTextWriter(TextWriter.Null)));
        }

        [Fact]
        public void ctor_throws_if_output_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultUsageWriter(Describe<Component>(), null));
        }

        private ComponentDescriptor Describe<T>()
        {
            return new ComponentDescriptor(typeof(T));
        }

        private sealed class Component
        {
        }
    }
}

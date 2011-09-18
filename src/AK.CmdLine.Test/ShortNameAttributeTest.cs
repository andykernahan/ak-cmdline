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
using Xunit;

namespace AK.CmdLine
{
    public class ShortNameAttributeTest
    {
        [Fact]
        public void can_create()
        {
            var attr = new ShortNameAttribute("name");

            Assert.Equal("name", attr.ShortName);
        }

        [Fact]
        public void can_convert_to_a_string()
        {
            var attr = new ShortNameAttribute("name");

            Assert.Equal("ShortName('name')", attr.ToString());
        }

        [Fact]
        public void ctor_throws_is_short_name_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ShortNameAttribute(null));
        }

        [Fact]
        public void ctor_throws_if_short_name_is_empty()
        {
            Assert.Throws<ArgumentException>(() => new ShortNameAttribute(String.Empty));
        }
    }
}
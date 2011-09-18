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
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace AK.CmdLine.Impl
{
    public class SwitchTest
    {
        [Theory]
        [ClassData(typeof(InvalidSwitchValueProvider))]
        public void cannot_parse(string s)
        {
            Switch actual;
            Assert.False(Switch.TryParse(s, out actual));
            Assert.Null(actual);
        }

        [Theory]
        [ClassData(typeof(ValidSwitchValueProvider))]
        public void can_parse(string s, string expectedName, string expectedValue)
        {
            Switch actual;
            Assert.True(Switch.TryParse(s, out actual));
            Assert.NotNull(actual);
            Assert.Equal(expectedName, actual.Name);
            Assert.Equal(expectedValue, actual.Value);
        }

        [Fact]
        public void can_determine_if_the_switch_has_a_value()
        {
            Switch actual;
            Switch.TryParse("/name:value", out actual);
            Assert.True(actual.HasValue);
            Switch.TryParse("/name", out actual);
            Assert.False(actual.HasValue);
        }

        [Fact]
        public void can_convert_to_string()
        {
            Switch actual;
            Switch.TryParse("/name:value", out actual);
            Assert.Equal("name('value')", actual.ToString());
        }

        private sealed class InvalidSwitchValueProvider : ValidProvider
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { null };
                yield return V(String.Empty);
                yield return V("    ");
                // MS form.
                yield return V("/ name");
                yield return V("/ name:value");
                // TODO to strict?
                yield return V("/ name :value");

                // Short form.
                yield return V("- name");
                yield return V("- name=value");
                // TODO to strict?
                yield return V("- name =value");

                // Long form.
                yield return V("-- name");
                yield return V("-- name=value");
                // TODO to strict?
                yield return V("-- name =value");
            }
        }

        private sealed class ValidSwitchValueProvider : ValidProvider
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                // MS form.
                yield return V("/name", "name", "");
                yield return V("/name:value", "name", "value");
                yield return V("/name-", "name", "-");
                yield return V("/name:-", "name", "-");
                yield return V("/name+", "name", "+");
                yield return V("/name:+", "name", "+");

                // Short form.
                yield return V("-name", "name", "");
                yield return V("-name=value", "name", "value");
                yield return V("-name-", "name", "-");
                yield return V("-name=-", "name", "-");
                yield return V("-name+", "name", "+");
                yield return V("-name=+", "name", "+");

                // Long form.
                yield return V("--name", "name", "");
                yield return V("--name=value", "name", "value");
                yield return V("--name-", "name", "-");
                yield return V("--name=-", "name", "-");
                yield return V("--name+", "name", "+");
                yield return V("--name=+", "name", "+");
            }
        }

        private abstract class ValidProvider : IEnumerable<object[]>
        {
            public abstract IEnumerator<object[]> GetEnumerator();

            protected object[] V(params object[] args)
            {
                return args;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}

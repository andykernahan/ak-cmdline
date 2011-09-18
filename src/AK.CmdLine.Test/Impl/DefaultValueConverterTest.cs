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
    public class DefaultValueConverterTest
    {
        public DefaultValueConverterTest()
        {
            Converter = new DefaultValueConverter();
        }

        [Theory]
        [ClassData(typeof(ValidConversionsValueProvider))]
        public void can_convert(string s, object expected)
        {
            object actual;
            Assert.True(Converter.TryConvert(s, expected.GetType(), out actual));
            Assert.Equal(expected, actual);
        }

        [Theory]
        [ClassData(typeof(InvalidConversionsValueProvider))]
        public void cannot_convert(string s, Type targetType)
        {
            object actual;
            Assert.False(Converter.TryConvert(s, targetType, out actual));
            Assert.Null(actual);
        }

        [Fact]
        public void can_register_a_from_converter()
        {
            DefaultValueConverter.Register(x => new CanRegisterAFromConverter(x));
            object actual;
            Assert.True(Converter.TryConvert("value", typeof(CanRegisterAFromConverter), out actual));
            var typedActual = Assert.IsType<CanRegisterAFromConverter>(actual);
            Assert.Equal("value", typedActual.Value);
        }

        [Fact]
        public void register_converter_throws_if_convert_from_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => DefaultValueConverter.Register<string>(null));
        }

        private DefaultValueConverter Converter
        {
            get;
            set;
        }

        private sealed class CanRegisterAFromConverter
        {
            public readonly string Value;
            public CanRegisterAFromConverter(string value)
            {
                Value = value;
            }
        }

        private sealed class RegisteringAFromOnlyUsesTheToStringMethodAsTheToConverter { }

        private sealed class ValidConversionsValueProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return V("True", true);
                yield return V("true", true);
                yield return V("+", true);
                yield return V("False", false);
                yield return V("false", false);
                yield return V("-", false);
                yield return V("a", 'a');
                yield return V("1", (byte)1);
                yield return V("1", (short)1);
                yield return V("1", (ushort)1);
                yield return V("1", (int)1);
                yield return V("1", (uint)1);
                yield return V("1", (long)1);
                yield return V("1", (ulong)1);
                yield return V("1.1", (float)1.1);
                yield return V("1.1", (double)1.1);
                yield return V("1.1", (decimal)1.1);
                yield return V("2011-11-19 23:19:00", DateTime.Parse("2011-11-19 23:19:00"));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static object[] V(string s, object expected)
            {
                return new object[] { s, expected };
            }
        }

        private sealed class InvalidConversionsValueProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return V("ab", typeof(char));
                yield return V("a", typeof(byte));
                yield return V("a", typeof(short));
                yield return V("a", typeof(ushort));
                yield return V("a", typeof(int));
                yield return V("a", typeof(uint));
                yield return V("a", typeof(long));
                yield return V("a", typeof(ulong));
                yield return V("a", typeof(float));
                yield return V("a", typeof(double));
                yield return V("a", typeof(decimal));
                yield return V("a", typeof(DateTime));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static object[] V(string s, Type targetType)
            {
                return new object[] { s, targetType };
            }
        }
    }
}
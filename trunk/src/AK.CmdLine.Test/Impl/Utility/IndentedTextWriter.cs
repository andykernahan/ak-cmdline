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
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace AK.CmdLine.Impl.Utility
{
    public class IndentedTextWriterTest : TestClass
    {
        [Fact]
        public void can_create()
        {
            var writer = new IndentedTextWriter(TextWriter.Null);

            Assert.Zero(writer.IndentLevel);
        }

        [Fact]
        public void ctor_throws_if_writer_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new IndentedTextWriter(null));
        }

        [Fact]
        public void writer_disposes_of_inner_writer()
        {
            var inner = new StringWriter();
            var writer = new IndentedTextWriter(inner);

            writer.Dispose();
            Assert.Throws<ObjectDisposedException>(() => inner.WriteLine());
        }

        [Fact]
        public void can_push_and_pop_indent()
        {
            var writer = new IndentedTextWriter(TextWriter.Null);

            var indent1 = writer.PushIndent(1);
            Assert.Equal(1, writer.IndentLevel);

            var indent2 = writer.PushIndent(2);
            Assert.Equal(3, writer.IndentLevel);

            indent1.Dispose();
            Assert.Equal(2, writer.IndentLevel);

            indent2.Dispose();
            Assert.Zero(writer.IndentLevel);
        }

        [Fact]
        public void can_pop_and_indent_multiple_times_with_effect()
        {
            var writer = new IndentedTextWriter(TextWriter.Null);

            var indent2 = writer.PushIndent(2);

            writer.PushIndent(1);
            Assert.Equal(3, writer.IndentLevel);

            indent2.Dispose();
            Assert.Equal(1, writer.IndentLevel);

            indent2.Dispose();
            Assert.Equal(1, writer.IndentLevel);
        }

        [Fact]
        public void push_indent_throws_if_indent_is_not_positive()
        {
            var writer = new IndentedTextWriter(TextWriter.Null);

            Assert.Throws<ArgumentOutOfRangeException>(() => writer.PushIndent(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => writer.PushIndent(-1));
        }

        [Fact]
        public void indent_length_reflects_length_of_indent()
        {
            var writer = new IndentedTextWriter(TextWriter.Null);

            Assert.Equal(writer.IndentString.Length * writer.IndentLevel, writer.IndentLength);
            using (writer.PushIndent(5))
            {
                Assert.Equal(writer.IndentString.Length * writer.IndentLevel, writer.IndentLength);
            }

            writer.IndentString = null;
            Assert.Equal(0, writer.IndentLength);
        }

        [Fact]
        public void close_calls_inner()
        {
            AssertInnerCall(x => x.Close(), x => x.Close());
        }

        [Fact]
        public void flush_calls_inner()
        {
            AssertInnerCall(x => x.Flush(), x => x.Flush());
        }

        [Fact]
        public void encoding_gets_inner()
        {
            AssertInnerGet(x => x.Encoding, x => x.Encoding, Encoding.ASCII);
        }

        [Fact]
        public void format_provider_gets_inner()
        {
            AssertInnerGet(x => x.FormatProvider, x => x.FormatProvider, CultureInfo.InvariantCulture);
        }

        [Fact]
        public void new_line_gets_inner()
        {
            AssertInnerGet(x => x.NewLine, x => x.NewLine, "newLine");
        }

        [Fact]
        public void new_line_sets_inner()
        {
            var inner = new Mock<TextWriter>(MockBehavior.Strict);
            var writer = new IndentedTextWriter(inner.Object);

            inner.SetupSet(x => x.NewLine = "newLine");

            writer.NewLine = "newLine";

            inner.VerifyAll();
        }

        private static void AssertInnerCall(
            Action<IndentedTextWriter> outerCall,
            Expression<Action<TextWriter>> innerCall)
        {
            var inner = new Mock<TextWriter>(MockBehavior.Strict);
            var writer = new IndentedTextWriter(inner.Object);

            inner.Setup(innerCall);

            outerCall(writer);

            inner.VerifyAll();
        }

        private void AssertInnerGet<TProperty>(
            Func<IndentedTextWriter, TProperty> outerGet,
            Expression<Func<TextWriter, TProperty>> innerGet,
            TProperty expected)
        {
            var inner = new Mock<TextWriter>(MockBehavior.Strict);
            var writer = new IndentedTextWriter(inner.Object);

            inner.SetupGet(innerGet).Returns(expected);

            Assert.Equal(expected, outerGet(writer));

            inner.VerifyAll();
        }
    }
}
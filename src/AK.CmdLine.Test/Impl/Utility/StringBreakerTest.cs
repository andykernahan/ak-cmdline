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
using System.Linq;
using Xunit;

namespace AK.CmdLine.Impl.Utility
{
    public class StringBreakerTest
    {
        [Fact]
        public void breaks_input_into_segments()
        {
            AssertEqual(
                "Quisque ut leo mauris. Cras gravida est sit amet mauris rutrum vitae elementum elit consequat. Aliquam hendrerit pharetra turpis, eget gravida enim ultrices venenatis. Mauris augue sapien, lacinia id viverra non, porta non sem. Duis vulputate ligula a augue auctor gravida. Sed et metus dolor. Ut orci ligula, rhoncus ac gravida in, fringilla quis nunc. Nunc vel lacinia orci. Duis pellentesque molestie sapien id tristique. Ut accumsan neque sed enim sodales a vestibulum eros euismod. Vivamus eu tortor eget metus commodo bibendum.",
                80,
                "Quisque ut leo mauris. Cras gravida est sit amet mauris rutrum vitae elementum",
                "elit consequat. Aliquam hendrerit pharetra turpis, eget gravida enim ultrices",
                "venenatis. Mauris augue sapien, lacinia id viverra non, porta non sem. Duis",
                "vulputate ligula a augue auctor gravida. Sed et metus dolor. Ut orci ligula,",
                "rhoncus ac gravida in, fringilla quis nunc. Nunc vel lacinia orci. Duis",
                "pellentesque molestie sapien id tristique. Ut accumsan neque sed enim sodales a",
                "vestibulum eros euismod. Vivamus eu tortor eget metus commodo bibendum.");
        }

        [Fact]
        public void inserts_hyphons_when_cannot_break_a_segment()
        {
            AssertEqual(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                5,
                "Lorem",
                "ipsum",
                "dolor",
                "sit",
                "amet,",
                "cons-",
                "ecte-",
                "tur",
                "adip-",
                "isci-",
                "ng",
                "elit.");
        }

        [Fact]
        public void includes_break_character_when_segment_is_equal_to_break_length()
        {
            AssertEqual(
                "Lorem,ipsum.",
                6,
                "Lorem,",
                "ipsum.");
        }

        [Fact]
        public void breaks_input_into_segments_of_with_minimum_length()
        {
            AssertEqual(
                "Lorem.",
                2,
                "L-",
                "o-",
                "r-",
                "e-",
                "m.");
        }

        [Fact]
        public void does_not_break_input_when_lengths_are_equal()
        {
            var input = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            Assert.Equal(input, new StringBreaker(input, input.Length).Single());
        }

        [Fact(Skip = "TODO")]
        public void skips_lines_comprised_of_only_whitespace()
        {
            AssertEqual(
                "Lorem,      ipsum.",
                6,
                "Lorem,",
                "ipsum.");
        }

        [Fact]
        public void ctor_throws_is_input_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new StringBreaker(null, 1));
        }

        [Fact]
        public void ctor_throws_if_length_is_less_than_two_()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StringBreaker("test", -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new StringBreaker("test", 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new StringBreaker("test", 1));
            Assert.DoesNotThrow(() => new StringBreaker("test", 2));
        }

        private static void AssertEqual(string input, int length, params string[] expected)
        {
            Assert.Equal(expected, new StringBreaker(input, length).ToArray());
        }
    }
}

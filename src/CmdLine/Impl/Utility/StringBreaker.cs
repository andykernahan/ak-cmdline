// Copyright 2012 Andy Kernahan
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
using System.Collections.Generic;

namespace AK.CmdLine.Impl.Utility
{
    /// <summary>
    /// Provides supporting for breaking strings on whitespace and punctuation into segments of a maximum length.
    /// </summary>
    public struct StringBreaker : IEnumerable<string>
    {
        #region Fields.

        private readonly string _input;
        private readonly int _length;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="StringBreaker"/> structure.
        /// </summary>
        /// <param name="input">The input string to break.</param>
        /// <param name="length">The maximum segment length.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="input"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="length"/> is less than or equal to one.
        /// </exception>
        public StringBreaker(string input, int length)
        {
            Guard.NotNull(input, "input");
            Guard.InRange(length > 1, "length");

            _input = input;
            _length = length;
        }

        /// <inheritdoc/>        
        public IEnumerator<string> GetEnumerator()
        {
            if (_input.Length <= _length)
            {
                yield return _input;
                yield break;
            }
            int startIndex = 0;
            while (startIndex < _input.Length)
            {
                if (_input.Length - startIndex <= _length)
                {
                    yield return _input.Substring(startIndex);
                    yield break;
                }
                int breakIndex;
                switch (FindNextBreak(startIndex, out breakIndex))
                {
                    case BreakType.None:
                        yield return _input.Substring(startIndex, _length - 1) + "-";
                        startIndex += _length - 1;
                        break;
                    case BreakType.Whitespace:
                        yield return _input.Substring(startIndex, breakIndex - startIndex);
                        startIndex = breakIndex + 1;
                        break;
                    case BreakType.Puntuation:
                        yield return _input.Substring(startIndex, 1 + breakIndex - startIndex);
                        startIndex = breakIndex + 1;
                        break;
                    default:
                        throw Guard.ArgumentOutOfRange("BreakType");
                }
            }
        }

        #endregion

        #region Explicit Interface.

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Private Impl.

        private BreakType FindNextBreak(int startIndex, out int breakIndex)
        {
            breakIndex = -1;
            var type = BreakType.None;
            int endIndex = startIndex + _length;
            for (int i = startIndex; i < endIndex; ++i)
            {
                switch (GetBreakType(_input[i]))
                {
                    case BreakType.Whitespace:
                        breakIndex = i;
                        type = BreakType.Whitespace;
                        break;
                    case BreakType.Puntuation:
                        breakIndex = i;
                        type = BreakType.Puntuation;
                        break;
                }
            }
            if (GetBreakType(_input[endIndex]) == BreakType.Whitespace)
            {
                breakIndex = endIndex;
                type = BreakType.Whitespace;
            }
            return type;
        }

        private enum BreakType
        {
            None,
            Whitespace,
            Puntuation
        }

        private BreakType GetBreakType(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                return BreakType.Whitespace;
            }
            if (Char.IsPunctuation(c))
            {
                return BreakType.Puntuation;
            }
            return BreakType.None;
        }

        #endregion
    }
}

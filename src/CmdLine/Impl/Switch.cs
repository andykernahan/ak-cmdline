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
using System.Text.RegularExpressions;

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// Defines a command line switch and provides parsing support. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class Switch
    {
        #region Fields.

        private static readonly Regex[] Patterns =
        {
            // MS form.
            new Regex(@"^(?:/)(?<name>\w+)((?:[:=])?(?<value>.*))?$"),
            // Short/long form.
            new Regex(@"^(?:-{1,2})(?<name>\w+)((?:[=:])?(?<value>.*))?$")
        };

        #endregion

        #region Public Interface.

        /// <summary>
        /// Attemps to parse a <see cref="AK.CmdLine.Impl.Switch"/> from the specified input string
        /// and returns a value indicating success.
        /// </summary>
        /// <param name="s">The String.</param>
        /// <param name="result">When this methods returns, contains the parsed switch on success,
        /// otherwise; <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when parsing was successful, otherwide;
        /// <see langword="false"/>.</returns>
        public static bool TryParse(string s, out Switch result)
        {
            if (!String.IsNullOrWhiteSpace(s))
            {
                foreach (var pattern in Patterns)
                {
                    var match = pattern.Match(s);
                    if (match.Success)
                    {
                        result = new Switch(match.Groups["name"].Value,
                            match.Groups["value"].Value);
                        return true;
                    }
                }
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns> A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0}('{1}')", Name, Value);
        }

        /// <summary>
        /// Gets the switch's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the switch's value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets a value indicating if this switch has a value.
        /// </summary>
        public bool HasValue => Value.Length > 0;

        #endregion

        #region Private Impl.

        private Switch(string name, string value)
        {
            Name = name;
            Value = value;
        }

        #endregion
    }
}

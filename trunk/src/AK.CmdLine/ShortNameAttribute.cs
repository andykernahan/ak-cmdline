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

namespace AK.CmdLine
{
    /// <summary>
    /// Specifies the short name of the decorated member. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class ShortNameAttribute : Attribute
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.ShortNameAttribute"/>.
        /// </summary>
        /// <param name="shortName">The short name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="shortName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="shortName"/> is empty.
        /// </exception>
        public ShortNameAttribute(string shortName)
        {
            Guard.NotNullOrEmpty(shortName, "shortName");

            ShortName = shortName;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "ShortName('{0}')", ShortName);
        }

        /// <summary>
        /// Gets the short name.
        /// </summary>
        public string ShortName { get; private set; }

        #endregion
    }
}
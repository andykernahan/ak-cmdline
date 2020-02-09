// Copyright 2009 Andy Kernahan
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
using System.Reflection;

namespace AK.CmdLine.Impl.Utility
{
    /// <summary>
    /// Provides easy access to an assembly's metadata. This class cannot be inherited.
    /// </summary>
    public sealed class AssemblyInfo
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.Impl.Utility.AssemblyInfo"/>.
        /// </summary>
        /// <param name="assembly">The assembly to describe.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="assembly"/> is <see langword="null"/>.
        /// </exception>
        public AssemblyInfo(Assembly assembly)
        {
            Guard.NotNull(assembly, "assembly");

            Assembly = assembly;
        }

        /// <summary>
        /// Gets the version title of the assembly.
        /// </summary>
        public string VersionedTitle
        {
            get { return String.Format(CultureInfo.CurrentCulture, "{0} - v{1}", Title, Version); }
        }

        /// <summary>
        /// Gets the assembly title
        /// </summary>
        public string Title
        {
            get { return With<AssemblyTitleAttribute>(attr => attr.Title); }
        }

        /// <summary>
        /// Gets the assembly product.
        /// </summary>
        public string Product
        {
            get { return With<AssemblyProductAttribute>(attr => attr.Product); }
        }

        /// <summary>
        /// Gets the assembly description.
        /// </summary>
        public string Description
        {
            get { return With<AssemblyDescriptionAttribute>(attr => attr.Description); }
        }

        /// <summary>
        /// Gets the assembly copyright information.
        /// </summary>
        public string Copyright
        {
            get { return With<AssemblyCopyrightAttribute>(attr => attr.Copyright); }
        }

        /// <summary>
        /// Gets the assembly copyright information.
        /// </summary>
        public string Company
        {
            get { return With<AssemblyCompanyAttribute>(attr => attr.Company); }
        }

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        public Version Version
        {
            get { return Assembly.GetName().Version; }
        }

        /// <summary>
        /// Gets the assembly file version.
        /// </summary>
        public string FileVersion
        {
            get { return With<AssemblyFileVersionAttribute>(x => x.Version); }
        }

        /// <summary>
        /// Gets the assembly informational version.
        /// </summary>
        public string InformationalVersion
        {
            get { return With<AssemblyInformationalVersionAttribute>(x => x.InformationalVersion); }
        }

        #endregion

        #region Private Impl.

        private string With<T>(Func<T, string> selector) where T : Attribute
        {
            T attr = (T)Attribute.GetCustomAttribute(Assembly, typeof(T));
            return attr != null ? selector(attr) : String.Empty;
        }

        private Assembly Assembly { get; set; }

        #endregion
    }
}

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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// An <see cref="AK.CmdLine.Impl.IUsageWriter"/> implementation which uses the component's XML
    /// documentation to provide usage information. This class cannot be inherited.
    /// </summary>
    public sealed class XmlDocUsageProvider : DefaultUsageWriter
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.Impl.DefaultUsageWriter"/> class.
        /// </summary>
        /// <param name="component">The <see cref="AK.CmdLine.Impl.ComponentDescriptor"/> help is
        /// provided for.</param>
        /// <param name="output">The output <see cref="System.IO.TextWriter"/>.</param>
        public XmlDocUsageProvider(ComponentDescriptor component, TextWriter output)
            : base(component, output)
        {
            LoadDocument();
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Gets a descripton of the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        /// <returns>
        /// A descripton of the specified <paramref name="method"/>.
        /// </returns>
        protected override string GetDescription(MethodDescriptor method)
        {
            var element = GetElement(method);
            if(element != null && (element = element.Element("summary")) != null)
            {
                return ToString(element);
            }
            return base.GetDescription(method);
        }

        /// <summary>
        /// Gets a descripton of the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>.</param>
        /// <returns>
        /// A descripton of the specified <paramref name="parameter"/>.
        /// </returns>
        protected override string GetDescription(ParameterDescriptor parameter)
        {
            var element = GetElement(parameter.Method);
            if(element != null)
            {
                element = element.Elements("param")
                    .Where(x => x.Attribute("name").Value.Equals(parameter.Name))
                    .SingleOrDefault();
                if(element != null)
                {
                    return ToString(element);
                }
            }
            return base.GetDescription(parameter);
        }

        #endregion

        #region Private Impl.

        private void LoadDocument()
        {
            var path = GetDocumentationPath();
            try
            {
                Document = XDocument.Load(path);
                Members = Document.Element("doc")
                    .Element("members")
                    .Elements()
                    .ToList();
            }
            catch(FileNotFoundException)
            {
                Trace.WriteLine(String.Format(CultureInfo.CurrentCulture,
                    "{0}: no xml docs found at path '{1}'",
                    typeof(XmlDocUsageProvider).FullName, path));
                Members = Enumerable.Empty<XElement>();
            }
        }

        private string GetDocumentationPath()
        {
            var location = Component.ComponentType.Assembly.Location;
            return Path.Combine(Path.GetDirectoryName(location),
                Path.GetFileNameWithoutExtension(location) + ".xml");
        }

        private XElement GetElement(MethodDescriptor method)
        {
            var xmlName = GetXmlName(method);
            return Members.Where(x => x.Attribute("name").Value.Equals(xmlName))
                .SingleOrDefault();
        }

        private string GetXmlName(MethodDescriptor method)
        {
            var name = new StringBuilder();
            name.Append("M:").Append(Component.ComponentType.FullName).Append(".").Append(method.Name);
            if(method.Parameters.Count > 0)
            {
                name.Append('(');
                foreach(var parameter in method.Parameters)
                {
                    name.Append(parameter.ParameterType.FullName).Append(',');
                }
                name[name.Length - 1] = ')';
            }
            return name.ToString();
        }

        private string GetComponentXmlName()
        {
            return "T:" + Component.ComponentType.FullName;
        }

        private static string ToString(XElement element)
        {
            return element.Value.Trim();
        }

        private XDocument Document { get; set; }

        private IEnumerable<XElement> Members { get; set; }

        #endregion
    }
}
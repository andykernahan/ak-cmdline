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
using System.Text.RegularExpressions;
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
        /// Gets a descripton of the specified component.
        /// </summary>
        /// <returns>A descripton of the component.</returns>
        protected override string GetComponentDescription()
        {
            var summary = GetMemberSummaryValue(MakeComponentMemberName());
            return !String.IsNullOrWhiteSpace(summary) ? summary : base.GetComponentDescription();
        }

        /// <summary>
        /// Gets a descripton of the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        /// <returns>
        /// A descripton of the specified <paramref name="method"/>.
        /// </returns>
        protected override string GetDescription(MethodDescriptor method)
        {
            var summary = GetMemberSummaryValue(MakeMethodMemberName(method));
            return !String.IsNullOrWhiteSpace(summary) ? summary : base.GetDescription(method);
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
            var element = GetMember(MakeMethodMemberName(parameter.Method));
            if (element != null)
            {
                element = element.Elements("param")
                    .Where(x => x.Attribute("name").Value.Equals(parameter.Name))
                    .SingleOrDefault();
                if (element != null && !String.IsNullOrWhiteSpace(element.Value))
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
            catch (FileNotFoundException)
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

        private XElement GetMember(string name)
        {
            return Members.Where(x => x.Attribute("name").Value.Equals(name)).SingleOrDefault();
        }

        private string GetMemberSummaryValue(string name)
        {
            var member = GetMember(name);
            return member != null ? ToString(member.Element("summary")) : null;
        }

        private string MakeMethodMemberName(MethodDescriptor method)
        {
            var name = new StringBuilder();
            name.Append("M:");
            FormatFullName(Component.ComponentType, name);
            name.Append(".").Append(method.Name);
            if (method.Parameters.Count > 0)
            {
                name.Append('(');
                foreach (var parameter in method.Parameters)
                {
                    FormatFullName(parameter.ParameterType, name);
                    name.Append(',');
                }
                name[name.Length - 1] = ')';
            }
            return name.ToString();
        }

        private string MakeComponentMemberName()
        {
            var name = new StringBuilder();
            name.Append("T:");
            FormatFullName(Component.ComponentType, name);
            return name.ToString();
        }

        private static void FormatFullName(Type type, StringBuilder name)
        {
            if (!type.IsGenericType)
            {
                name.Append(type.FullName);
                return;
            }
            name.Append(type.FullName, 0, type.FullName.IndexOf('`')).Append('{');
            foreach (var genericArgument in type.GetGenericArguments())
            {
                FormatFullName(genericArgument, name);
                name.Append(',');
            }
            name[name.Length - 1] = '}';
        }

        private static string ToString(XElement element)
        {
            return Regex.Replace(element.Value, @"\s+", " ").Trim();
        }

        private XDocument Document { get; set; }

        private IEnumerable<XElement> Members { get; set; }

        #endregion
    }
}
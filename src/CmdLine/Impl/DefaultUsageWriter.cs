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
using System.IO;
using System.Linq;
using AK.CmdLine.Impl.Utility;

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// A default <see cref="AK.CmdLine.Impl.IUsageWriter"/> implementation.
    /// </summary>
    public class DefaultUsageWriter : IUsageWriter
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.Impl.DefaultUsageWriter"/> class.
        /// </summary>
        /// <param name="component">The <see cref="AK.CmdLine.Impl.ComponentDescriptor"/> help is
        /// provided for.</param>
        /// <param name="output">The output <see cref="System.IO.TextWriter"/>.</param>
        public DefaultUsageWriter(ComponentDescriptor component, TextWriter output)
        {
            Guard.NotNull(component, "component");
            Guard.NotNull(output, "output");

            Component = component;
            Output = new IndentedTextWriter(output);
            AssemblyInfo = new AssemblyInfo(component.ComponentType.Assembly);
        }

        /// <summary>
        /// Writes usage information about the component.
        /// </summary>        
        public virtual void Usage()
        {
            WriteUsage(null);
        }

        /// <summary>
        /// Writes usage information about the specifed exception thrown by the specified
        /// <paramref name="method"/> during invocation.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/> that threw the
        /// exception.</param>
        /// <param name="e">The <see cref="System.Exception"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="method"/> or <paramref name="e"/> is <see langword="null"/>.
        /// </exception>
        public virtual void Usage(MethodDescriptor method, Exception e)
        {
            Guard.NotNull(e, "e");
            Guard.NotNull(e, "e");

            WriteUsage(method, e.Message);
        }

        /// <summary>
        /// Writes usuage information indicating that a command name is required.
        /// </summary>
        public virtual void CommandNameRequired()
        {
            WriteUsage(Messages.DefaultUsageWriter_CommandRequired);
        }

        /// <summary>
        /// Writes usuage information indicating that the specified command <paramref name="name"/>
        /// is invalid.
        /// </summary>
        /// <param name="name">The invalid command name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="name"/> is empty.
        /// </exception>
        public virtual void InvalidCommandName(string name)
        {
            Guard.NotNullOrEmpty(name, "name");

            WriteUsage(Messages.DefaultUsageWriter_InvalidCommand, name);
        }

        /// <summary>
        /// Writes usuage information indicating that an invalid number of arguments have provided
        /// for the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="method"/> is <see langword="null"/>.
        /// </exception>
        public virtual void InvalidNumberOfArguments(MethodDescriptor method)
        {
            Guard.NotNull(method, "method");

            WriteUsage(method, Messages.DefaultUsageWriter_InvalidNumberOfArguments, method.Name);
        }

        /// <summary>
        /// Writes usuage information indicating that the format specified
        /// <paramref name="switch"/> is invalid.
        /// </summary>
        /// <param name="switch">The switch.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="switch"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="switch"/> is empty.
        /// </exception>
        public virtual void InvalidSwitchFormat(string @switch)
        {
            Guard.NotNullOrEmpty(@switch, "@switch");

            WriteUsage(Messages.DefaultUsageWriter_InvalidSwitchFormat, @switch);
        }

        /// <summary>
        /// Writes usuage information indicating that the specified argument
        /// <paramref name="name"/> is invalid for the specfied <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        /// <param name="name">The switch name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="method"/> or <paramref name="name"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="name"/> is empty.
        /// </exception>
        public void InvalidArgumentName(MethodDescriptor method, string name)
        {
            Guard.NotNull(method, "method");
            Guard.NotNullOrEmpty(name, "name");

            WriteUsage(method, Messages.DefaultUsageWriter_InvalidArgumentName, name, method.Name);
        }

        /// <summary>
        /// Writes usuage information indicating that the <paramref name="value"/> provided
        /// for the specified <paramref name="parameter"/> is invalid.
        /// </summary>
        /// <param name="parameter">The <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>.</param>
        /// <param name="value">The attempted value.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="parameter"/> is <see langword="null"/>.
        /// </exception>
        public void InvalidArgumentValue(ParameterDescriptor parameter, string value)
        {
            Guard.NotNull(parameter, "parameter");

            WriteUsage(parameter.Method, Messages.DefaultUsageWriter_InvalidArgumentValue,
                parameter.Name, value, parameter.ParameterType.Name);
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Writes usage information about the component.
        /// </summary>
        /// <param name="message">The optional message to print.</param>
        /// <param name="args">The optional message arguments.</param>
        protected virtual void WriteUsage(string message, params object[] args)
        {
            WriteComponentHeader();
            WriteMessage(message, args);
            WriteMethodList();
        }

        /// <summary>
        /// Writes usage information about the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        /// <param name="message">The optional message to print.</param>     
        /// <param name="args">The optional message arguments.</param>
        protected virtual void WriteUsage(MethodDescriptor method, string message, params object[] args)
        {
            WriteComponentHeader();
            WriteMessage(message, args);
            WriteMethod(method);
        }

        /// <summary>
        /// Gets a descripton of the specified component.
        /// </summary>
        /// <returns>A descripton of the component.</returns>
        protected virtual string GetComponentDescription()
        {
            return Component.Description.Length > 0 ? Component.Description : AssemblyInfo.Description;
        }

        /// <summary>
        /// Writes the component's information.
        /// </summary>
        protected virtual void WriteComponentHeader()
        {
            Output.WriteLine("{0} - v{1}", GetComponentDescription(), AssemblyInfo.Version);
            Output.WriteLine(AssemblyInfo.Copyright);
            Output.WriteLine();
        }

        /// <summary>
        /// Writes the component method list.
        /// </summary>
        protected virtual void WriteMethodList()
        {
            foreach (var method in Component.Methods)
            {
                WriteMethod(method);
            }
        }

        /// <summary>
        /// Writes the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        protected virtual void WriteMethod(MethodDescriptor method)
        {
            WriteMethodSignature(method);
            using (Output.PushIndent(2))
            {
                WriteMethodDescription(method);
                if (method.Parameters.Count > 0)
                {
                    using (Output.PushIndent(5))
                    {
                        WriteParameterList(method);
                    }
                }
            }
            Output.WriteLine();
        }

        /// <summary>
        /// Writes the signature of the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        protected virtual void WriteMethodSignature(MethodDescriptor method)
        {
            WriteName(method);
            foreach (var parameter in method.Parameters)
            {
                Output.Write(' ');
                WriteParameterSignature(parameter);
            }
            Output.WriteLine();
        }

        /// <summary>
        /// Writes the signature of the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>.</param>
        protected virtual void WriteParameterSignature(ParameterDescriptor parameter)
        {
            // TODO perhaps default should be in the parameter list instead?
            if (parameter.IsOptional)
            {
                Output.Write('[');
                WriteName(parameter);
                if (!parameter.IsBoolean)
                {
                    Output.Write('=');
                }
                WriteParameterDefaultValue(parameter);
                Output.Write(']');
            }
            else
            {
                Output.Write('<');
                WriteName(parameter);
                Output.Write('>');
            }
        }

        /// <summary>
        /// Writes the default value of the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>.</param>
        protected virtual void WriteParameterDefaultValue(ParameterDescriptor parameter)
        {
            Output.Write(parameter.IsBoolean ?
                (bool)parameter.DefaultValue ? '+' : '-' :
                parameter.DefaultValue);
        }

        /// <summary>
        /// Writes the description of the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        protected virtual void WriteMethodDescription(MethodDescriptor method)
        {
            Output.Write("- ");
            WriteWrappedString(2, GetDescription(method));
        }

        /// <summary>
        /// Gets a descripton of the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        /// <returns>A descripton of the specified <paramref name="method"/>.</returns>
        protected virtual string GetDescription(MethodDescriptor method)
        {
            return method.Description;
        }

        /// <summary>
        /// Writes the <see cref="AK.CmdLine.Impl.ParameterDescriptor"/> list of the specified
        /// <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        protected virtual void WriteParameterList(MethodDescriptor method)
        {
            var parameters = method.Parameters.Select(x => new
            {
                Description = GetDescription(x),
                Name = x.IsBoolean ? x.Name + "[+|-]" : x.Name
            }).ToList();
            var widestName = parameters.Max(x => x.Name.Length);
            foreach (var parameter in parameters)
            {
                Output.Write("--{0}{1}  ",
                    parameter.Name,
                    ":".PadRight(1 + widestName - parameter.Name.Length, ' '));
                WriteWrappedString(2 + 1 + widestName + 2, parameter.Description);
            }
        }

        /// <summary>
        /// Gets a descripton of the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>.</param>
        /// <returns>A descripton of the specified <paramref name="parameter"/>.</returns>
        protected virtual string GetDescription(ParameterDescriptor parameter)
        {
            return parameter.Description;
        }

        /// <summary>
        /// Writes the name specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>.</param>
        protected virtual void WriteName(ParameterDescriptor parameter)
        {
            Output.Write("--");
            Output.Write(parameter.Name);
            if (parameter.ShortName.Length > 0)
            {
                Output.Write("[-{0}]", parameter.ShortName);
            }
        }

        /// <summary>
        /// Writes the name specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        protected virtual void WriteName(MethodDescriptor method)
        {
            Output.Write(method.Name);
            if (method.ShortName.Length > 0)
            {
                Output.Write("[{0}]", method.ShortName);
            }
        }

        /// <summary>
        /// Gets the <see cref="AK.CmdLine.Impl.ComponentDescriptor"/>.
        /// </summary>
        protected ComponentDescriptor Component { get; }

        /// <summary>
        /// Gets the output <see cref="AK.CmdLine.Impl.Utility.IndentedTextWriter"/>.
        /// </summary>
        protected IndentedTextWriter Output { get; }

        /// <summary>
        /// Gets the component's <see cref="AK.CmdLine.Impl.Utility.AssemblyInfo"/>.
        /// </summary>
        /// <value>The <see cref="AK.CmdLine.Impl.Utility.AssemblyInfo"/>.</value>
        protected AssemblyInfo AssemblyInfo { get; }

        #endregion

        #region Private Impl.

        /// <summary>
        /// Writes a wrapped string.
        /// </summary>
        /// <param name="currentLineLength">The length of the current line.</param>
        /// <param name="s">The string to write.</param>
        private void WriteWrappedString(int currentLineLength, string s)
        {
            var firstSegment = true;
            foreach (var segment in new StringBreaker(s, 88 - currentLineLength - Output.IndentLength))
            {
                if (firstSegment)
                {
                    Output.WriteLine(segment);
                    firstSegment = false;
                }
                else
                {
                    using (Output.PushIndent(currentLineLength))
                    {
                        Output.WriteLine(segment);
                    }
                }
            }
        }

        private void WriteMessage(string message, params object[] args)
        {
            if (!String.IsNullOrEmpty(message))
            {
                Output.WriteLine(message, args);
                Output.WriteLine();
            }
        }

        #endregion
    }
}

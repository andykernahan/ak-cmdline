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
// See the License for the specific

using System;

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// Writes usage information about a <see cref="AK.CmdLine.Impl.ComponentDescriptor"/>.
    /// </summary>
    public interface IUsageWriter
    {
        /// <summary>
        /// Writes usage information about the component.
        /// </summary>        
        void Usage();

        /// <summary>
        /// Writes usage information about the specified exception thrown by the specified
        /// <paramref name="method"/> during invocation.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/> that threw the
        /// exception.</param>
        /// <param name="e">The <see cref="System.Exception"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="method"/> or <paramref name="e"/> is <see langword="null"/>.
        /// </exception>
        void Usage(MethodDescriptor method, Exception e);

        /// <summary>
        /// Writes usage information indicating that a command name is required.
        /// </summary>
        void CommandNameRequired();

        /// <summary>
        /// Writes usage information indicating that the specified command <paramref name="name"/>
        /// is invalid.
        /// </summary>
        /// <param name="name">The invalid command name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="name"/> is empty.
        /// </exception>
        void InvalidCommandName(string name);

        /// <summary>
        /// Writes usage information indicating that an invalid number of arguments have provided
        /// for the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="method"/> is <see langword="null"/>.
        /// </exception>
        void InvalidNumberOfArguments(MethodDescriptor method);

        /// <summary>
        /// Writes usage information indicating that the format specified
        /// <paramref name="switch"/> is invalid.
        /// </summary>
        /// <param name="switch">The switch.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="switch"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="switch"/> is empty.
        /// </exception>
        void InvalidSwitchFormat(string @switch);

        /// <summary>
        /// Writes usage information indicating that the specified argument
        /// <paramref name="name"/> is invalid for the specified <paramref name="method"/>.
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
        void InvalidArgumentName(MethodDescriptor method, string name);

        /// <summary>
        /// Writes usage information indicating that the <paramref name="value"/> provided
        /// for the specified <paramref name="parameter"/> is invalid.
        /// </summary>
        /// <param name="parameter">The <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>.</param>
        /// <param name="value">The attempted value.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="parameter"/> is <see langword="null"/>.
        /// </exception>
        void InvalidArgumentValue(ParameterDescriptor parameter, string value);
    }
}

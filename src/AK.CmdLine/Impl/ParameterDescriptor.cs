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
using System.Reflection;

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// Describes a <see cref="System.Reflection.ParameterInfo"/>. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class ParameterDescriptor : DescriptorBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.Impl.ParameterDescriptor"/> class.
        /// </summary>
        /// <param name="method">The <see cref="AK.CmdLine.Impl.MethodDescriptor"/> which defines
        /// the parameter.</param>
        /// <param name="parameter">The <see cref="System.Reflection.ParameterInfo"/> to describe.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="method"/> or <paramref name="parameter"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Thrown when <paramref name="parameter"/> could not be described.
        /// </exception>
        public ParameterDescriptor(MethodDescriptor method, ParameterInfo parameter)
        {
            Guard.NotNull(method, "method");
            Guard.NotNull(parameter, "parameter");

            Method = method;
            Parameter = parameter;
            if(parameter.IsOptional)
            {
                IsOptional = true;
                DefaultValue = parameter.DefaultValue;
            }
            else if(parameter.ParameterType.IsArray)
            {
                if(GetAttribute<ParamArrayAttribute>() == null)
                {
                    throw Guard.ParameterDescriptor_OnlyParamArraysAreSupported(method.Method, parameter);
                }
                IsParams = true;
                IsOptional = true;
                DefaultValue = Array.CreateInstance(parameter.ParameterType.GetElementType(), 0);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Parameter.ToString();
        }

        /// <summary>
        /// Gets the name of this descriptor.
        /// </summary>
        public override string Name
        {
            get { return this.Parameter.Name; }
        }

        /// <summary>
        /// Gets the <see cref="AK.CmdLine.Impl.MethodDescriptor"/> which defines this
        /// <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>.
        /// </summary>
        public MethodDescriptor Method { get; private set; }

        /// <summary>
        /// Gets the <see cref="System.Type"/> of this parameter.
        /// </summary>
        public Type ParameterType
        {
            get { return Parameter.ParameterType; }
        }

        /// <summary>
        /// Gets a value indicating whether this parameter is optional.
        /// </summary>
        public bool IsOptional { get; private set; }

        /// <summary>
        /// Gets a value indicating if this parameter is decorated with the
        /// <see cref="System.ParamArrayAttribute"/>.
        /// </summary>
        public bool IsParams { get; private set; }

        /// <summary>
        /// Gets the default value of this parameter.
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Gets a value indicating if this parameter is a boolean parameter.
        /// </summary>
        public bool IsBoolean
        {
            get { return ParameterType == typeof(Boolean); }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// When overriden in a derived class; gets the attribute of type
        /// <typeparamref name="TAttribute"/> from the underlying member info.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute <see cref="System.Type"/>.</typeparam>
        /// <returns>
        /// The attribute if founds, otherwise; <see langword="null"/>.
        /// </returns>
        protected override TAttribute GetAttribute<TAttribute>()
        {
            return (TAttribute)Attribute.GetCustomAttribute(Parameter, typeof(TAttribute), true);
        }

        #endregion

        #region Private Impl.

        private ParameterInfo Parameter { get; set; }

        #endregion
    }
}
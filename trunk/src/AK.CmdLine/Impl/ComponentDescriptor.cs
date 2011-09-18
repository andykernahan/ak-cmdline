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
using System.Linq;
using System.Reflection;

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// Describes a <see cref="System.Type"/>. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class ComponentDescriptor : DescriptorBase
    {
        #region Fields.

        private const BindingFlags MethodBindingFlags = BindingFlags.Public |
            BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.Impl.ComponentDescriptor"/> class.
        /// </summary>
        /// <param name="componentType">The <see cref="System.Type"/> to describe.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="componentType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Thrown when <paramref name="componentType"/> could not be described.
        /// </exception>
        public ComponentDescriptor(Type componentType)
        {
            Guard.NotNull(componentType, "componentType");

            ComponentType = componentType;
            Methods = CreateMethodDescriptors();
        }

        /// <summary>
        /// Gets the <see cref="AK.CmdLine.Impl.MethodDescriptor"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the method to get.</param>
        /// <returns>The <see cref="AK.CmdLine.Impl.MethodDescriptor"/> with the specified
        /// <paramref name="name"/> if one exists, otherwise; <see langword="null"/>.</returns>
        public MethodDescriptor GetMethod(string name)
        {
            return !String.IsNullOrWhiteSpace(name) ?
                Methods.Where(x => x.IsNamed(name)).SingleOrDefault() : null;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return ComponentType.ToString();
        }

        /// <summary>
        /// Gets the name of this descriptor.
        /// </summary>
        public override string Name
        {
            get { return this.ComponentType.Name; }
        }

        /// <summary>
        /// Gets the collection of <see cref="AK.CmdLine.Impl.MethodDescriptor"/>s for this type.
        /// </summary>
        public IList<MethodDescriptor> Methods { get; private set; }

        /// <summary>
        /// Gets the <see cref="System.Type"/> of the component.
        /// </summary>
        /// <value>The <see cref="System.Type"/> of the component.</value>
        public Type ComponentType { get; private set; }

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
            return (TAttribute)Attribute.GetCustomAttribute(ComponentType, typeof(TAttribute), true);
        }

        #endregion

        #region Private Impl.

        private IList<MethodDescriptor> CreateMethodDescriptors()
        {
            var descriptors = new List<MethodDescriptor>();
            foreach(var method in ComponentType.GetMethods(MethodBindingFlags)
                .Where(x => !(x.IsSpecialName || x.DeclaringType == typeof(object))))
            {
                var descriptor = new MethodDescriptor(this, method);
                if(descriptors.Contains(descriptor))
                {
                    throw Guard.ComponentDescriptor_DuplicateMethod(method);
                }
                descriptors.Add(descriptor);
            }
            return descriptors.AsReadOnly();
        }

        #endregion
    }
}
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
using System.Threading.Tasks;

namespace AK.CmdLine.Impl
{
    /// <summary>
    /// Describes a <see cref="System.Reflection.MethodInfo"/>. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class MethodDescriptor : DescriptorBase
    {
        #region Fields.

        private readonly MethodInvoker _methodInvoker;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.Impl.MethodDescriptor"/> class.
        /// </summary>
        /// <param name="component">The <see cref="AK.CmdLine.Impl.ComponentDescriptor"/> which defines
        /// the method.</param>
        /// <param name="method">The <see cref="System.Reflection.MethodInfo"/> to describe.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="method"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Thrown when <paramref name="method"/> could not be described.
        /// </exception>
        public MethodDescriptor(ComponentDescriptor component, MethodInfo method)
        {
            Guard.NotNull(component, "component");
            Guard.NotNull(method, "method");

            Component = component;
            Method = method;
            Parameters = CreateParameterDescriptors();
            _methodInvoker = MethodInvoker.Create(method);
        }

        /// <summary>
        /// Gets the <see cref="AK.CmdLine.Impl.ParameterDescriptor"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the parameter to get.</param>
        /// <returns>The <see cref="AK.CmdLine.Impl.ParameterDescriptor"/> with the specified
        /// <paramref name="name"/> if one exists, otherwise; <see langword="null"/>.</returns>
        public ParameterDescriptor GetParameter(string name)
        {
            return !String.IsNullOrWhiteSpace(name) ?
                Parameters.Where(x => x.IsNamed(name)).SingleOrDefault() : null;
        }

        /// <summary>
        /// Invokes the underlying method.
        /// </summary>
        /// <param name="component">The component instance.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The result of the underlying methods invocation.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="component"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.Reflection.TargetInvocationException">
        /// Thrown when underlying methods throws an exception.
        /// </exception>
        public object Invoke(object component, object[] parameters)
        {
            return _methodInvoker.Invoke(component, parameters);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Method.ToString();
        }

        /// <summary>
        /// Gets the name of this descriptor.
        /// </summary>
        public override string Name
        {
            get { return this.Method.Name; }
        }

        /// <summary>
        /// Gets the <see cref="AK.CmdLine.Impl.MethodDescriptor"/> which defines this
        /// <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>.
        /// </summary>
        public ComponentDescriptor Component { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="AK.CmdLine.Impl.ParameterDescriptor"/>s for this method.
        /// </summary>
        public IList<ParameterDescriptor> Parameters { get; private set; }

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
            return (TAttribute)Attribute.GetCustomAttribute(Method, typeof(TAttribute), true);
        }

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Gets the underlying <see cref="System.Reflection.MethodInfo"/>.
        /// </summary>
        internal MethodInfo Method { get; set; }

        #endregion

        #region Private Impl.

        private IList<ParameterDescriptor> CreateParameterDescriptors()
        {
            var descriptors = new List<ParameterDescriptor>();
            foreach(var parameter in Method.GetParameters())
            {
                var descriptor = new ParameterDescriptor(this, parameter);
                if(descriptors.Contains(descriptor))
                {
                    throw Guard.MethodDescriptor_DuplicateParameter(Method, parameter);
                }
                descriptors.Add(descriptor);
            }
            return descriptors.AsReadOnly();
        }

        private abstract class MethodInvoker
        {
            public object Invoke(object component, object[] parameters)
            {
                try
                {
                    return InvokeCore(component, parameters);
                }
                catch (AggregateException e)
                {
                    var flat = e.Flatten();
                    if (flat.InnerExceptions.Count == 1)
                    {
                        throw new TargetInvocationException(flat.Message, flat.InnerExceptions[0]);
                    }
                    throw new TargetInvocationException(e.Message, e);
                }
            }

            protected abstract object InvokeCore(object component, object[] parameters);

            public static MethodInvoker Create(MethodInfo method)
            {
                if (!typeof(Task).IsAssignableFrom(method.ReturnType))
                {
                    return new Sync(method);
                }
                for (var type = method.ReturnType; type != typeof(Task); type = type.BaseType)
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        var asyncType = typeof(Async<>).MakeGenericType(type.GetGenericArguments()[0]);
                        return (MethodInvoker)Activator.CreateInstance(asyncType, method);
                    }
                }
                return new Async(method);
            }

            private sealed class Sync : MethodInvoker
            {
                private readonly MethodInfo _method;

                public Sync(MethodInfo method)
                {
                    _method = method;
                }

                protected override object InvokeCore(object component, object[] parameters)
                {
                    return _method.Invoke(component, parameters);
                }
            }

            private sealed class Async : MethodInvoker
            {
                private readonly MethodInfo _method;

                public Async(MethodInfo method)
                {
                    _method = method;
                }

                protected override object InvokeCore(object component, object[] parameters)
                {
                    // Invoke the method on the ThreadPool, where there is no sync-context.
                    R(Task.Run(() => R((Task)_method.Invoke(component, parameters))));
                    return null;
                }

                private static void R(Task task)
                {
                    task.Wait();
                }
            }

            private sealed class Async<T> : MethodInvoker
            {
                private readonly MethodInfo _method;

                public Async(MethodInfo method)
                {
                    _method = method;
                }

                protected override object InvokeCore(object component, object[] parameters)
                {
                    // Invoke the method on the ThreadPool, where there is no sync-context.
                    return R(Task.Run(() => R((Task<T>)_method.Invoke(component, parameters))));
                }

                private static T R(Task<T> task)
                {
                    return task.Result;
                }
            }
        }

        #endregion
    }
}

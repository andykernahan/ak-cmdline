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
using System.IO;
using System.Linq;
using System.Reflection;
using AK.CmdLine.Impl;
// Prevent conflict with Switch.
using Debug = System.Diagnostics.Debug;

namespace AK.CmdLine
{
    /// <summary>
    /// A command line driver which invokes methods on a component.
    /// </summary>
    public class CmdLineDriver
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.CmdLineDriver"/> class.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="usageWriter">The <see cref="AK.CmdLine.Impl.IUsageWriter"/>.</param>
        /// <param name="valueConverter">The <see cref="AK.CmdLine.Impl.IValueConverter"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="component"/>, <paramref name="usageWriter"/> or 
        /// <paramref name="valueConverter"/> is <see langword="null"/>.
        /// </exception>
        public CmdLineDriver(object component, IUsageWriter usageWriter, IValueConverter valueConverter)
            : this(component)
        {
            Guard.NotNull(usageWriter, "usageWriter");
            Guard.NotNull(valueConverter, "valueConverter");

            UsageWriter = usageWriter;
            ValueConverter = valueConverter;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.CmdLine.CmdLineDriver"/> class.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="err">The error <see cref="System.IO.TextWriter"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="component"/> or <paramref name="err"/> is <see langword="null"/>.
        /// </exception>
        public CmdLineDriver(object component, TextWriter err)
            : this(component)
        {
            Guard.NotNull(err, "err");

            UsageWriter = new XmlDocUsageProvider(Descriptor, err);
            ValueConverter = new DefaultValueConverter();
        }

        /// <summary>
        /// Processes the specified command line arguments and invokes the corresponding method on
        /// the component. A return value indicates if the arguments were parsed and the method
        /// successfully invoked.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns><see langword="true"/> if the arguments were sucessfuly processed, otherwise;
        /// <see langword="false"/>.</returns>
        public bool TryProcess(params string[] args)
        {
            State = new ProcessState(args);
            var result = TryProcess();
            State = null;
            return result;
        }

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Gets the <see cref="AK.CmdLine.Impl.ComponentDescriptor"/>.
        /// </summary>
        internal ComponentDescriptor Descriptor { get; set; }

        #endregion

        #region Private Impl.

        private CmdLineDriver(object component)
        {
            Guard.NotNull(component, "component");

            Component = component;
            Descriptor = new ComponentDescriptor(component.GetType());
        }

        private bool TryProcess()
        {
            return TryParseMethod() && TryParseArguments() && TryInvokeMethod();
        }

        private bool TryParseMethod()
        {
            if(!State.HasArgs)
            {
                UsageWriter.CommandNameRequired();
                return false;
            }
            var name = State.Args.Dequeue();
            var method = Descriptor.GetMethod(name);
            if(method == null)
            {
                UsageWriter.InvalidCommandName(name);
                return false;
            }
            State.Method = method;
            return true;
        }

        private bool TryParseArguments()
        {
            if(State.Method.Parameters.Count == 0 && State.HasArgs)
            {
                UsageWriter.InvalidNumberOfArguments(State.Method);
                return false;
            }
            while(State.HasArgs)
            {
                string value;
                ParameterDescriptor descriptor;
                if(!TryCollectNextParameter(out descriptor, out value))
                {
                    return false;
                }
                object convertedValue;
                if(!TryConvertParameterValue(value, descriptor, out convertedValue))
                {
                    return false;
                }
                UpdateParameterStateValue(descriptor, convertedValue);
            }
            if(State.HasArgs || !State.IsSatified)
            {
                UsageWriter.InvalidNumberOfArguments(State.Method);
                return false;
            }
            return true;
        }

        private void UpdateParameterStateValue(ParameterDescriptor descriptor, object value)
        {
            var parameterState = State.ParameterStates[descriptor];
            if(descriptor.ParameterType.IsArray)
            {
                Array newValues;
                if(parameterState.Value == null)
                {
                    newValues = Array.CreateInstance(descriptor.ParameterType.GetElementType(), 1);
                }
                else
                {
                    var oldValues = (Array)parameterState.Value;
                    newValues = Array.CreateInstance(oldValues.GetType().GetElementType(), oldValues.Length + 1);
                    oldValues.CopyTo(newValues, 0);
                }
                newValues.SetValue(value, newValues.Length - 1);
                parameterState.Value = newValues;
            }
            else
            {
                parameterState.Value = value;
            }
            parameterState.IsSatisfied = true;
        }

        private bool TryCollectNextParameter(out ParameterDescriptor descriptor, out string value)
        {
            descriptor = null;
            value = null;
            // TODO this still smells.
            Switch @switch;
            string arg = State.Args.Dequeue();
            if(Switch.TryParse(arg, out @switch))
            {
                if((descriptor = State.Method.GetParameter(@switch.Name)) == null)
                {
                    UsageWriter.InvalidArgumentName(State.Method, @switch.Name);
                    return false;
                }
                value = @switch.Value;
                return true;
            }
            value = arg;
            if(State.PositionalIndex < State.Method.Parameters.Count)
            {
                descriptor = State.Method.Parameters[State.PositionalIndex++];
                return true;
            }
            if(State.Method.Parameters.Last().IsParams)
            {
                descriptor = State.Method.Parameters.Last();
                return true;
            }
            UsageWriter.InvalidSwitchFormat(arg);
            return false;
        }

        private bool TryConvertParameterValue(string value, ParameterDescriptor descriptor,
            out object convertedValue)
        {
            convertedValue = null;
            // If only a switch name has been specified then try and consume the next argument
            // unless it's a boolean argument in which case its presence denotes +.
            if(value.Length == 0)
            {
                if(descriptor.IsBoolean)
                {
                    convertedValue = true;
                    return true;
                }
                if(!State.HasArgs)
                {
                    UsageWriter.InvalidNumberOfArguments(State.Method);
                    return false;
                }
                value = State.Args.Dequeue();
            }
            var targetType = descriptor.ParameterType.IsArray ?
                descriptor.ParameterType.GetElementType() : descriptor.ParameterType;
            if(ValueConverter.TryConvert(value, targetType, out convertedValue))
            {
                return true;
            }
            UsageWriter.InvalidArgumentValue(descriptor, value);
            return false;
        }

        private bool TryInvokeMethod()
        {
            try
            {
                State.Method.Invoke(Component, State.GetParameterValues());
                return true;
            }
            catch(TargetInvocationException e)
            {
                UsageWriter.Usage(State.Method, e.InnerException);
                return false;
            }
        }

        private ProcessState State { get; set; }

        private object Component { get; set; }

        private IUsageWriter UsageWriter { get; set; }

        private IValueConverter ValueConverter { get; set; }

        private sealed class ProcessState
        {
            private MethodDescriptor _method;

            public ProcessState(string[] args)
            {
                Args = new Queue<string>(args != null ? args : Enumerable.Empty<string>());
            }

            public object[] GetParameterValues()
            {
                Debug.Assert(IsSatified);
                if(Method.Parameters.Count == 0)
                {
                    return null;
                }
                return Method.Parameters.Select(x => ParameterStates[x].Value).ToArray();
            }

            public MethodDescriptor Method
            {
                get { return _method; }
                set
                {
                    Debug.Assert(_method == null);
                    Debug.Assert(value != null);
                    _method = value;
                    BuildParameterStates();
                }
            }

            private void BuildParameterStates()
            {
                ParameterStates = Method.Parameters.Select(x => new ParameterState(x))
                    .ToDictionary(x => x.Descriptor);
            }

            public IDictionary<ParameterDescriptor, ParameterState> ParameterStates { get; private set; }

            public bool IsSatified
            {
                get
                {
                    return ParameterStates != null && (ParameterStates.Count == 0 ||
                        ParameterStates.Values.All(x => x.IsSatisfied));
                }
            }

            public Queue<string> Args { get; private set; }

            public bool HasArgs
            {
                get { return Args.Count > 0; }
            }

            public int PositionalIndex { get; set; }
        }

        private sealed class ParameterState
        {
            public ParameterState(ParameterDescriptor descriptor)
            {
                Descriptor = descriptor;
                if(Descriptor.IsOptional)
                {
                    Value = Descriptor.DefaultValue;
                    IsSatisfied = true;
                }
            }

            public ParameterDescriptor Descriptor { get; private set; }

            public bool IsSatisfied { get; set; }

            public object Value { get; set; }
        }

        #endregion
    }
}
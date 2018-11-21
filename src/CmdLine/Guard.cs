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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace AK.CmdLine
{
    /// <summary>
    /// Library guard. This class is <see langword="static"/>.
    /// </summary>
    internal static class Guard
    {
        #region Validation.

        [DebuggerStepThrough]
#if DEBUG
        internal static void NotNull<T>(T instance, string paramName) where T : class
        {
#else
        internal static void NotNull(object instance, string paramName) {
#endif
            if(instance == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        [DebuggerStepThrough]
        internal static void InRange(bool condition, string paramName)
        {
            if(!condition)
            {
                throw ArgumentOutOfRange(paramName);
            }
        }

        [DebuggerStepThrough]
        internal static void NotNullOrEmpty(string s, string paramName)
        {
            NotNull(s, paramName);
            if(String.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException(Messages.Guard_NonEmptyString, paramName);
            }
        }

        [DebuggerStepThrough]
        internal static void NotNullOrEmpty<T>(IEnumerable<T> collection, string paramName)
        {
            NotNull(collection, paramName);
            using(var elements = collection.GetEnumerator())
            {
                if(!elements.MoveNext())
                {
                    throw new ArgumentException(Messages.Guard_NonEmptyCollection, paramName);
                }
            }
        }

        #endregion

        #region Exception Factory Methods.

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName);
        }

        internal static NotSupportedException MethodDescriptor_DuplicateParameter(MethodInfo method,
            ParameterInfo parameter)
        {
            return new NotSupportedException(Format(Messages.MethodDescriptor_DuplicateParameter,
                parameter.Name, method.Name, method.DeclaringType.FullName));
        }

        internal static NotSupportedException ParameterDescriptor_OnlyParamArraysAreSupported(
            MethodInfo method, ParameterInfo parameter)
        {
            return new NotSupportedException(Format(Messages.ParameterDescriptor_OnlyParamArraysAreSupported,
                method.Name, method.DeclaringType.FullName, parameter.Name));
        }

        internal static NotSupportedException ComponentDescriptor_DuplicateMethod(MethodInfo method)
        {
            return new NotSupportedException(Format(Messages.ComponentDescriptor_DuplicateMethod,
                method.Name, method.DeclaringType.FullName));
        }

        private static string Format(string format, params string[] args)
        {
            return String.Format(Messages.Culture, format, args);
        }

        #endregion
    }
}
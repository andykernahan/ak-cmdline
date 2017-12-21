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

namespace AK.CmdLine.Contracts
{
    public abstract class EquivalenceContractAssertionBase<T>
    {
        protected static readonly Type TypeOfT = typeof(T);

        protected EquivalenceContractAssertionBase(
            IEnumerable<T> equivalentInstances,
            IEnumerable<T> distinctInstances)
        {
            if (equivalentInstances == null)
            {
                throw new ArgumentNullException(nameof(equivalentInstances));
            }
            if (distinctInstances == null)
            {
                throw new ArgumentNullException(nameof(distinctInstances));
            }
            EquivalentInstances = equivalentInstances;
            DistinctInstances = distinctInstances;
        }

        public void AssertContract()
        {
            var equivalentInstancesCopy = EquivalentInstances.ToArray();
            var distinctInstancesCopy = DistinctInstances.ToArray();

            DoGeneralAssertions(equivalentInstancesCopy);
            DoEquivalentAssertions(equivalentInstancesCopy);
            DoGeneralAssertions(distinctInstancesCopy);
            DoDistinctAssertions(distinctInstancesCopy);
        }

        protected abstract void DoGeneralAssertions(T x);

        protected abstract void DoEquivalentAssertions(T x, T y);

        protected abstract void DoDistinctAssertions(T x, T y);

        protected static void InvokeOperator(string name, T x, T y, Delegate assertion)
        {
            var method = GetMethod("op_" + name, TypeOfT, TypeOfT);
            if (method != null)
            {
                assertion.DynamicInvoke(method.Invoke(x, new object[] {x, y}));
            }
        }

        protected static MethodInfo GetMethod(string name, params Type[] types)
        {
            MethodInfo method;
            var scope = TypeOfT;
            do
            {
                if ((method = scope.GetMethod(name, types)) != null)
                {
                    return method;
                }
            }
            while ((scope = scope.BaseType) != null);

            return null;
        }

        private void DoGeneralAssertions(IEnumerable<T> instances)
        {
            foreach (var x in instances)
            {
                DoGeneralAssertions(x);
            }
        }

        private void DoEquivalentAssertions(IEnumerable<T> instances)
        {
            foreach (var x in instances)
            {
                foreach (var y in instances)
                {
                    DoEquivalentAssertions(x, y);
                }
            }
        }

        private void DoDistinctAssertions(IEnumerable<T> instances)
        {
            var xIndex = 0;
            var yIndex = 0;
            foreach (var x in instances)
            {
                foreach (var y in instances)
                {
                    if (yIndex != xIndex)
                    {
                        DoDistinctAssertions(x, y);
                    }
                    ++yIndex;
                }
                ++xIndex;
                yIndex = 0;
            }
        }

        private IEnumerable<T> EquivalentInstances { get; }
        private IEnumerable<T> DistinctInstances { get; }
    }
}

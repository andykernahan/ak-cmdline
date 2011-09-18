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
using Xunit.Extensions;

namespace AK.Icm.Contracts
{
    public class EqualityContractAssertion<T> : EquivalenceContractAssertionBase<T> where T : IEquatable<T>
    {
        public EqualityContractAssertion(
            Assertions assert,
            IEnumerable<T> equivalentInstances,
            IEnumerable<T> distinctInstances)
            : base(assert, equivalentInstances, distinctInstances) { }

        protected override void DoEquivalentAssertions(T x, T y)
        {
            AssertEqual(x, y);
            AssertEqual(y, x);
        }

        protected override void DoDistinctAssertions(T x, T y)
        {
            AssertNotEqual(x, y);
            AssertNotEqual(y, x);
        }

        protected override void DoGeneralAssertions(T x)
        {
            AssertEqual(x, x);
            if(!TypeOfT.GetType().IsValueType)
            {
                Assert.False((bool)GetMethod("Equals", TypeOfT).Invoke(x, new object[] { null }));
            }
            Assert.False(x.Equals((object)null));
        }

        private void AssertEqual(T x, T y)
        {
            Assert.True(x.Equals(y));
            Assert.True(x.Equals((object)y));
            if(y != null)
            {
                Assert.True(x.GetHashCode() == y.GetHashCode());
            }
            InvokeEqualityOperator(x, y, Assert.True);
            InvokeInequalityOperator(x, y, Assert.False);
        }

        private void AssertNotEqual(T x, T y)
        {
            Assert.False(x.Equals(y));
            Assert.False(x.Equals((object)y));
            if(y != null)
            {
                Assert.False(x.GetHashCode() == y.GetHashCode());
            }
            InvokeInequalityOperator(x, y, Assert.True);
            InvokeEqualityOperator(x, y, Assert.False);
        }

        private static void InvokeEqualityOperator(T x, T y, Action<bool> assertion)
        {
            InvokeOperator("Equality", x, y, assertion);
        }

        private static void InvokeInequalityOperator(T x, T y, Action<bool> assertion)
        {
            InvokeOperator("Inequality", x, y, assertion);
        }
    }
}
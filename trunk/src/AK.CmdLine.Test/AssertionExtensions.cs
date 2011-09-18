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
using AK.Icm.Contracts;
using Xunit.Extensions;

namespace AK.CmdLine
{
    public static class AssertionExtensions
    {
        public static void EqualityContract<T>(
            this Assertions assert,
            IEnumerable<T> equivalentInstances,
            IEnumerable<T> distinctInstances)
            where T : IEquatable<T>
        {
            new EqualityContractAssertion<T>(assert, equivalentInstances, distinctInstances).AssertContract();
        }

        public static void Zero(this Assertions assert, int actual)
        {
            assert.Equal(0, actual);
        }

        public static void Immutable<T>(this Assertions assert, IList<T> collection)
        {
            assert.NotNull(collection);
            assert.Throws<NotSupportedException>(() => collection.Add(default(T)));
            assert.Throws<NotSupportedException>(() => collection.Clear());
            assert.Throws<NotSupportedException>(() => collection.Remove(default(T)));
            assert.Throws<NotSupportedException>(() => collection.RemoveAt(0));
        }
    }
}
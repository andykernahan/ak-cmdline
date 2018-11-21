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
using AK.CmdLine.Contracts;
using Xunit;

namespace AK.CmdLine
{
    internal static class AssertEx
    {
        public static void EqualityContract<T>(
            IEnumerable<T> equivalentInstances,
            IEnumerable<T> distinctInstances)
            where T : IEquatable<T>
        {
            new EqualityContractAssertion<T>(equivalentInstances, distinctInstances).AssertContract();
        }

        public static void Zero(int actual)
        {
            Assert.Equal(0, actual);
        }

        public static void Immutable<T>(IList<T> collection)
        {
            Assert.NotNull(collection);
            Assert.Throws<NotSupportedException>(() => collection.Add(default(T)));
            Assert.Throws<NotSupportedException>(() => collection.Clear());
            Assert.Throws<NotSupportedException>(() => collection.Remove(default(T)));
            Assert.Throws<NotSupportedException>(() => collection.RemoveAt(0));
        }
    }
}

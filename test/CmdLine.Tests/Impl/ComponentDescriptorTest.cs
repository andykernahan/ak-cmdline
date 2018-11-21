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
using Xunit;

namespace AK.CmdLine.Impl
{
    public class ComponentDescriptorTest
    {
        [Fact]
        public void ctor_throws_if_methods_exist_with_the_same_short_name()
        {
            Assert.Throws<NotSupportedException>(() =>
                Describe<CtorThrowsIfMethodsExistWithTheSameShortName>());
        }

        [Fact]
        public void ctor_throws_if_overloaded_methods_exist()
        {
            Assert.Throws<NotSupportedException>(() =>
                Describe<CtorThrowsIfOverloadedMethodsExist>());
        }

        [Fact]
        public void can_get_component_type()
        {
            var descriptor = Describe<CanGetComponentType>();

            Assert.Equal(typeof(CanGetComponentType), descriptor.ComponentType);
        }

        [Fact]
        public void can_get_name()
        {
            Assert.Equal("CanGetName", Describe<CanGetName>().Name);
        }

        [Fact]
        public void can_get_public_methods()
        {
            var methods = Describe<CanGetPublicMethods>().Methods;

            Assert.Equal(2, methods.Count);
            Assert.Equal("Public1", methods[0].Name);
            Assert.Equal("Public2", methods[1].Name);
        }

        [Fact]
        public void methods_includes_inherited_public_methods_except_those_from_object()
        {
            var methods = Describe<MethodsIncludesInheritedPublicMethodsExceptThoseFromObject_Derived>().Methods;

            Assert.Equal(2, methods.Count);
            Assert.Equal("Declared", methods[0].Name);
            Assert.Equal("Inherited", methods[1].Name);
        }

        [Fact]
        public void methods_includes_overriden_methods_from_deepest_level()
        {
            var descriptor = Describe<MethodsIncludesOverridenMethodsFromDeepestLevel_Derived2>();

            Assert.Equal(2, descriptor.Methods.Count);
            Assert.Equal(typeof(MethodsIncludesOverridenMethodsFromDeepestLevel_Derived1),
                descriptor.GetMethod("OverridenInDerived1").Method.DeclaringType);
            Assert.Equal(typeof(MethodsIncludesOverridenMethodsFromDeepestLevel_Derived2),
                descriptor.GetMethod("OverridenInDerived2").Method.DeclaringType);
        }

        [Fact(Skip = "Determine if this is correct bahviour")]
        public void methods_includes_new_methods_from_deepest_level()
        {
            var descriptor = Describe<MethodsIncludesNewMethodsFromDeepestLevel_Derived2>();

            Assert.Equal(2, descriptor.Methods.Count);
            Assert.Equal(typeof(MethodsIncludesNewMethodsFromDeepestLevel_Derived1),
                descriptor.GetMethod("NewInDerived1").Method.DeclaringType);
            Assert.Equal(typeof(MethodsIncludesNewMethodsFromDeepestLevel_Derived2),
                descriptor.GetMethod("NewInDerived2").Method.DeclaringType);
        }

        [Fact]
        public void methods_excludes_events_and_property_accessors()
        {
            Assert.Empty(Describe<MethodsExcludesEventAndPropertyAccessors>().Methods);
        }

        [Fact]
        public void cannot_mutate_methods_collection()
        {
            AssertEx.Immutable(Describe<CannotMutateMethodsCollection>().Methods);
        }

        [Fact]
        public void can_get_a_method_by_its_name()
        {
            var descriptor = Describe<CanGetAMethodByItsName>();

            var method = descriptor.GetMethod("Method");
            Assert.NotNull(method);
            Assert.Equal("Method", method.Name);

            Assert.Null(descriptor.GetMethod(null));
            Assert.Null(descriptor.GetMethod(String.Empty));
            Assert.Null(descriptor.GetMethod("SomeMethod"));
        }

        private ComponentDescriptor Describe<T>()
        {
            return new ComponentDescriptor(typeof(T));
        }

        private sealed class MethodsExcludesEventAndPropertyAccessors
        {
            public string Property
            {
                get;
                set;
            }
#pragma warning disable 67
            public event EventHandler Event;
#pragma warning restore 67
        }

        private sealed class CanGetComponentType { }

        private sealed class CtorThrowsIfMethodsExistWithTheSameShortName
        {
            [ShortName("shortName")]
            public void Method1() { }

            [ShortName("shortName")]
            public void Method2() { }
        }

        private sealed class CtorThrowsIfOverloadedMethodsExist
        {
            public void Method() { }
            public void Method(int x) { }
        }

        private sealed class CanGetName
        {
            public void Method(string x) { }
        }

        private class CanGetPublicMethods
        {
            public void Public1() { }
            public void Public2() { }
            protected void Protected() { }
            internal void Internal() { }
            private void Private() { }
        }

        private class MethodsIncludesInheritedPublicMethodsExceptThoseFromObject_Base
        {
            public void Inherited() { }
            protected void Protected() { }
            internal void Internal() { }
            private void Private() { }
        }

        private sealed class MethodsIncludesInheritedPublicMethodsExceptThoseFromObject_Derived
            : MethodsIncludesInheritedPublicMethodsExceptThoseFromObject_Base
        {
            public void Declared() { }
        }

        private sealed class CanGetAMethodByItsName
        {
            public void Method() { }
            public void AnotherMethod() { }
        }

        private sealed class CannotMutateMethodsCollection { }

        private class MethodsIncludesOverridenMethodsFromDeepestLevel_Base
        {
            public virtual void OverridenInDerived1() { }
            public virtual void OverridenInDerived2() { }
        }

        private class MethodsIncludesOverridenMethodsFromDeepestLevel_Derived1 :
            MethodsIncludesOverridenMethodsFromDeepestLevel_Base
        {
            public override void OverridenInDerived1() { }
        }

        private class MethodsIncludesOverridenMethodsFromDeepestLevel_Derived2 :
            MethodsIncludesOverridenMethodsFromDeepestLevel_Derived1
        {
            public override void OverridenInDerived2() { }
        }

        private class MethodsIncludesNewMethodsFromDeepestLevel_Base
        {
            public void NewInDerived1() { }
            public void NewInDerived2() { }
        }

        private class MethodsIncludesNewMethodsFromDeepestLevel_Derived1 :
            MethodsIncludesNewMethodsFromDeepestLevel_Base
        {
            public new void NewInDerived1() { }
        }

        private class MethodsIncludesNewMethodsFromDeepestLevel_Derived2 :
            MethodsIncludesNewMethodsFromDeepestLevel_Derived1
        {
            public new void NewInDerived2() { }
        }
    }
}

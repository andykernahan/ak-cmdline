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
using Moq;
using Xunit;

namespace AK.CmdLine.Impl
{
    public class MethodDescriptorTest
    {
        [Fact]
        public void ctor_throws_if_parameters_exist_with_the_same_short_name()
        {
            Assert.Throws<NotSupportedException>(() =>
                Describe<CtorThrowsIfParametersExistWithTheSameShortName>());
        }

        [Fact]
        public void can_get_name()
        {
            Assert.Equal("Method", Describe<CanGetName>().Name);
        }

        [Fact]
        public void can_get_parameters()
        {
            var descriptor = Describe<CanGetParameters>();

            Assert.NotNull(descriptor.Parameters);
            Assert.Equal(2, descriptor.Parameters.Count);
            Assert.Equal("x", descriptor.Parameters[0].Name);
            Assert.Equal("y", descriptor.Parameters[1].Name);
        }

        [Fact]
        public void can_get_a_parameter_by_its_name()
        {
            var descriptor = Describe<CanGetAParameterByItsName>();
            var parameter = descriptor.GetParameter("x");

            Assert.NotNull(parameter);
            Assert.Equal("x", parameter.Name);

            Assert.Null(descriptor.GetParameter(null));
            Assert.Null(descriptor.GetParameter(String.Empty));
            Assert.Null(descriptor.GetParameter("SomeParameter"));
        }

        [Fact]
        public void cannot_mutate_parameters_collection()
        {
            AssertEx.Immutable(Describe<CannotMutateParametersCollection>().Parameters);
        }

        [Fact]
        public void can_invoke_method()
        {
            var component = new Mock<CanInvokeMethod>(MockBehavior.Strict);
            var descriptor = Describe(component.Object.GetType());
            component.Setup(x => x.Method(1, "y")).Returns("result");

            Assert.Equal("result", descriptor.Invoke(component.Object, new object[] {1, "y"}));

            component.Verify(x => x.Method(1, "y"), Times.Once());
        }

        [Fact]
        public void exceptions_are_bubbled_up_from_invoke()
        {
            var component = new ExceptionsAreBubbledUpFromInvoke();
            var descriptor = Describe<ExceptionsAreBubbledUpFromInvoke>();

            Assert.Throws<TargetInvocationException>(() => descriptor.Invoke(component, null));
            try
            {
                descriptor.Invoke(component, null);
            }
            catch (TargetInvocationException e)
            {
                Assert.IsType<ArgumentException>(e.InnerException);
            }
        }

        private static MethodDescriptor Describe<T>()
        {
            return Describe(typeof(T));
        }

        private static MethodDescriptor Describe(Type type)
        {
            return new MethodDescriptor(TestUtility.ComponentDescriptor, type.GetMethod("Method"));
        }

        private sealed class ExceptionsAreBubbledUpFromInvoke
        {
            public void Method()
            {
                throw new ArgumentException();
            }
        }

        public class CanInvokeMethod
        {
            public virtual string Method(int x, string y)
            {
                // Should be mocked.
                throw new NotImplementedException();
            }
        }

        private sealed class CtorThrowsIfParametersExistWithTheSameShortName
        {
            public void Method([ShortName("x")] string x, [ShortName("x")] string y)
            {
            }
        }

        private sealed class CanGetName
        {
            public void Method()
            {
            }
        }

        private sealed class CanGetParameters
        {
            public void Method(string x, string y)
            {
            }
        }

        private sealed class CanGetAParameterByItsName
        {
            public void Method(string x)
            {
            }
        }

        private sealed class CannotMutateParametersCollection
        {
            public void Method()
            {
            }
        }
    }
}

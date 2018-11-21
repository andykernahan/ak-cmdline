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
using System.Linq;
using Xunit;

namespace AK.CmdLine.Impl
{
    public class ParameterDescriptorTest
    {
        [Fact]
        public void can_get_name()
        {
            Assert.Equal("x", Describe<CanGetName>().Name);
        }

        [Fact]
        public void can_get_parameter_type()
        {
            Assert.Equal(typeof(int), Describe<CanGetParameterType>().ParameterType);
        }

        [Fact]
        public void can_determine_if_parameter_is_optional()
        {
            Assert.True(Describe<CanDetermineIfParameterIsOptional_Optional>().IsOptional);
            Assert.False(Describe<CanDetermineIfParameterIsOptional_Required>().IsOptional);
        }

        [Fact]
        public void can_determine_if_parameter_is_a_params()
        {
            Assert.True(Describe<CanDetermineIfParameterIsAParams_True>().IsParams);
            Assert.False(Describe<CanDetermineIfParameterIsAParams_False>().IsParams);
        }

        [Fact]
        public void parameters_decorated_with_param_array_attribute_are_optional()
        {
            Assert.True(Describe<ParametersDecoratedWithParamArrayAttributeAreOptional>().IsOptional);
        }

        [Fact(Skip = "Enforcement causes all mocked tests to fail and I don't want to require a CommandAttribute.")]
        public void array_type_parameters_must_be_decorated_with_the_param_array_attribute()
        {
            Assert.Throws<NotSupportedException>(() => Describe<ArrayTypeParametersMustBeDecoratedWithTheParamArrayAttribute>());
        }

        [Fact]
        public void can_determine_if_parameter_is_a_boolean()
        {
            Assert.True(Describe<CanDetermineIfParameterIsABoolean_IsBoolean>().IsBoolean);
            Assert.False(Describe<CanDetermineIfParameterIsABoolean_IsNotBoolean>().IsBoolean);
        }

        [Fact]
        public void can_get_the_default_value()
        {
            Assert.Equal("defaultValue", Describe<CanGetTheDefaultValue>().DefaultValue);
            Assert.Equal(new string[0], Describe<CanGetTheDefaultValue_Params>().DefaultValue);
        }

        private ParameterDescriptor Describe<T>()
        {
            return Describe<T>("x");
        }

        private ParameterDescriptor Describe<T>(string paramName)
        {
            return new ParameterDescriptor(TestUtility.MethodDescriptor, typeof(T)
                .GetMethod("Method")
                .GetParameters()
                .Where(x => x.Name.Equals(paramName))
                .Single());
        }

        private sealed class ArrayTypeParametersMustBeDecoratedWithTheParamArrayAttribute
        {
            public void Method(int[] x) { }
        }

        private sealed class CanDetermineIfAParameterIsTheLastOneDefinedInTheFormalParameterList
        {
            public void Method(int x, int y) { }
        }

        private sealed class CanGetName
        {
            public void Method(string x) { }
        }

        private sealed class CanDetermineIfParameterIsAParams_True
        {
            public void Method(params string[] x) { }
        }

        private sealed class CanDetermineIfParameterIsAParams_False
        {
            public void Method(string[] x) { }
        }

        private sealed class CanGetParameterType
        {
            public void Method(int x) { }
        }

        private sealed class CanDetermineIfParameterIsOptional_Required
        {
            public void Method(bool x) { }
        }

        private sealed class CanDetermineIfParameterIsOptional_Optional
        {
            public void Method(bool x = true) { }
        }

        private sealed class CanDetermineIfParameterIsABoolean_IsBoolean
        {
            public void Method(bool x) { }
        }

        private sealed class CanDetermineIfParameterIsABoolean_IsNotBoolean
        {
            public void Method(string x) { }
        }

        private sealed class CanGetTheDefaultValue
        {
            public void Method(string x = "defaultValue") { }
        }

        private sealed class CanGetTheDefaultValue_Params
        {
            public void Method(params string[] x) { }
        }

        private sealed class ImplmentsEqualityContract_Equivalent
        {
            public void Method(string x) { }
        }

        private sealed class ParametersDecoratedWithParamArrayAttributeAreOptional
        {
            public void Method(params string[] x) { }
        }
    }
}

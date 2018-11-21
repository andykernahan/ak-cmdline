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
using System.ComponentModel;
using System.Reflection;
using Moq;
using Xunit;

namespace AK.CmdLine.Impl
{
    public class DescriptorBaseTest
    {
        [Fact]
        public void can_get_short_name()
        {
            var provider = CreateMockProvider(new ShortNameAttribute("shortName"));
            var descriptor = new DescriptorStub(provider.Object);

            Assert.Equal("shortName", descriptor.ShortName);
            provider.Verify();
        }

        [Fact]
        public void short_name_is_blank_if_non_is_provided()
        {
            var provider = CreateMockProvider<ShortNameAttribute>();
            var descriptor = new DescriptorStub(provider.Object);

            Assert.Equal(String.Empty, descriptor.ShortName);
            provider.Verify();
        }

        [Fact]
        public void can_get_description()
        {
            var provider = CreateMockProvider(new DescriptionAttribute("description"));
            var descriptor = new DescriptorStub(provider.Object);

            Assert.Equal("description", descriptor.Description);
            provider.Verify();
        }

        [Fact]
        public void description_is_blank_if_non_is_provided()
        {
            var provider = CreateMockProvider<DescriptionAttribute>();
            var descriptor = new DescriptorStub(provider.Object);

            Assert.Equal(String.Empty, descriptor.Description);
            provider.Verify();
        }

        [Fact]
        public void can_determine_if_descriptor_matches_specified_name()
        {
            var provider = CreateMockProvider<DescriptionAttribute>();
            var descriptor = new DescriptorStub(provider.Object);

            Assert.True(descriptor.IsNamed("name"));
            Assert.True(descriptor.IsNamed("Name"));
            Assert.False(descriptor.IsNamed(String.Empty));

            provider = CreateMockProvider(new ShortNameAttribute("shortName"));
            descriptor = new DescriptorStub(provider.Object);

            Assert.True(descriptor.IsNamed("shortName"));
            Assert.True(descriptor.IsNamed("ShortName"));
            Assert.False(descriptor.IsNamed(String.Empty));

            Assert.False(descriptor.IsNamed(null));
        }

        private Mock<ICustomAttributeProvider> CreateMockProvider<TAttribute>(params TAttribute[] attributes)
            where TAttribute : Attribute
        {
            var provider = new Mock<ICustomAttributeProvider>();

            provider.Setup(x => x.GetCustomAttributes(typeof(TAttribute), true)).Returns(attributes).Verifiable();

            return provider;
        }

        private sealed class DescriptorStub : DescriptorBase
        {
            private readonly ICustomAttributeProvider _provider;

            public DescriptorStub(ICustomAttributeProvider provider)
            {
                _provider = provider;
            }

            protected override TAttribute GetAttribute<TAttribute>()
            {
                var attrs = _provider.GetCustomAttributes(typeof(TAttribute), true);
                return attrs.Length > 0 ? (TAttribute)attrs[0] : null;
            }

            public override string ToString()
            {
                throw new NotImplementedException();
            }

            public override string Name
            {
                get { return "name"; }
            }
        }
    }
}

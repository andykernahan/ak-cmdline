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
using System.Linq.Expressions;
using AK.CmdLine.Impl;
using Moq;
using Xunit;

namespace AK.CmdLine
{
    public class CmdLineDriverTest
    {
        [Fact]
        public void can_invoke_command_by_name()
        {
            AssertInvoked(
                x => x.Status(),
                "Status");
        }

        [Fact]
        public void can_invoke_command_by_name_using_different_casing()
        {
            AssertInvoked(
                x => x.Status(),
                "sTaTuS");
        }

        [Fact]
        public void can_invoke_command_by_short_name()
        {
            AssertInvoked(
                x => x.Status(),
                "st");
        }

        [Fact]
        public void can_invoke_command_by_short_name_using_different_casing()
        {
            AssertInvoked(
                x => x.Status(),
                "ST");
        }

        [Fact]
        public void can_invoke_command_using_switches()
        {
            AssertInvoked(
                x => x.Commit("program.cs", "deleted"),
                "Commit", "{0}path{1}program.cs", "{0}message{1}deleted");
        }

        [Fact]
        public void can_invoke_command_using_switch_short_names()
        {
            AssertInvoked(
                x => x.Commit("program.cs", "deleted"),
                "Commit", "{0}p{1}program.cs", "{0}m{1}deleted");
        }

        [Fact]
        public void can_invoke_command_using_positional_arguments()
        {
            AssertInvoked(
                x => x.Commit("program.cs", "deleted"),
                "Commit", "program.cs", "deleted");
        }

        [Fact]
        public void can_invoke_command_using_mixed_positional_and_switch_arguments()
        {
            AssertInvoked(
                x => x.PropSet("svn:ignore", "*", "."),
                "PropSet", "svn:ignore", "*", "{0}path{1}.");
        }

        [Fact]
        public void can_invoke_command_that_contains_params()
        {
            AssertInvoked(
                x => x.Diff(),
                "Diff");

            AssertInvoked(
                x => x.Diff("1"),
                "Diff", "1");

            AssertInvoked(
                x => x.Diff("1", "2", "3"),
                "Diff", "1", "2", "3");

            AssertInvoked(
                x => x.Resolve("accept"),
                "Resolve", "accept");

            AssertInvoked(
                x => x.Resolve("accept", "1"),
                "Resolve", "accept", "1");

            AssertInvoked(
                x => x.Resolve("accept", "1", "2", "3"),
                "Resolve", "accept", "1", "2", "3");
        }

        [Fact]
        public void writes_command_name_required_if_no_arguments_are_provided()
        {
            With((driver, svn, writer) =>
            {
                writer.Setup(x => x.CommandNameRequired());

                Assert.False(driver.TryProcess());
                Assert.False(driver.TryProcess(null));
                Assert.False(driver.TryProcess(new string[0]));

                writer.Verify(x => x.CommandNameRequired(), Times.Exactly(3));
            });
        }

        [Fact]
        public void writes_invalid_number_of_arguments_if_too_many_are_provided()
        {
            With((driver, svn, writer) =>
            {
                var statusMethod = driver.Descriptor.GetMethod("Status");
                writer.Setup(x => x.InvalidNumberOfArguments(statusMethod));

                Assert.False(driver.TryProcess("Status", "--arg0=test"));

                writer.Verify(x => x.InvalidNumberOfArguments(statusMethod), Times.Once());
            });
        }

        [Fact]
        public void writes_invalid_command_name_if_command_name_is_invalid()
        {
            With((driver, svn, writer) =>
            {
                foreach(var name in new[] { null, "", "SomeCommand" })
                {
                    writer.Setup(x => x.InvalidCommandName(name));

                    driver.TryProcess(name);

                    writer.Verify(x => x.InvalidCommandName(name), Times.Once());
                }
            });
        }

        [Fact]
        public void writes_invalid_argument_name_if_an_unrecognised_argument_is_provided()
        {
            With((driver, svn, writer) =>
            {
                var commitMethod = driver.Descriptor.GetMethod("Commit");
                writer.Setup(x => x.InvalidArgumentName(commitMethod, "invalid"));

                driver.TryProcess("Commit", "--invalid:test");

                writer.Verify(x => x.InvalidArgumentName(commitMethod, "invalid"), Times.Once());
            });
        }

        [Fact]
        public void ctor_throws_if_component_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new CmdLineDriver(null, TextWriter.Null));
        }

        [Fact]
        public void ctor_throws_if_err_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new CmdLineDriver(new Svn(), null));
        }

        private static void AssertInvoked(Expression<Action<Svn>> expected, params string[] args)
        {
            var combinations = from prefix in GetSwitchPrefixes()
                               from separator in GetSwitchSeparators()
                               select args.Select(x => String.Format(x, prefix, separator)).ToArray();
            foreach(var combination in combinations)
            {
                With((driver, svn) =>
                {
                    svn.Setup(expected);

                    Assert.True(driver.TryProcess(combination));

                    svn.Verify(expected, Times.Once());
                });
            }
        }

        private static IEnumerable<string> GetSwitchPrefixes()
        {
            yield return "/";
            yield return "-";
            yield return "--";
        }

        private static IEnumerable<string> GetSwitchSeparators()
        {
            yield return ":";
            yield return "=";
        }

        private static void With(Action<CmdLineDriver, Mock<Svn>> body)
        {
            With((driver, svn, usage) => body(driver, svn));
        }

        private static void With(Action<CmdLineDriver, Mock<Svn>, Mock<IUsageWriter>> body)
        {
            var svn = new Mock<Svn>(MockBehavior.Strict);
            var usageWriter = new Mock<IUsageWriter>(MockBehavior.Strict);
            var driver = new CmdLineDriver(svn.Object, usageWriter.Object, new DefaultValueConverter());
            body(driver, svn, usageWriter);
        }

        /// <summary>
        /// Our main test class which kind of mirrors subversion's command. This was chosen as
        /// it should be familiar to any devs reading this test class.
        /// </summary>
        public class Svn
        {
            [ShortName("st")]
            public virtual void Status()
            {
                ShouldBeMockedOut();
            }

            [ShortName("ci")]
            public virtual void Commit(
                [ShortName("p")]string path,
                [ShortName("m")]string message)
            {
                ShouldBeMockedOut();
            }

            public virtual void PropSet(string name, string value, string path)
            {
                ShouldBeMockedOut();
            }

            public virtual void Diff(params string[] files)
            {
                ShouldBeMockedOut();
            }

            public virtual void Resolve(string accept, params string[] files)
            {
                ShouldBeMockedOut();
            }

            private static void ShouldBeMockedOut()
            {
                throw new NotImplementedException("Caller should be mocked out.");
            }
        }
    }
}

// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExampleBackgroundTasks;
using Microsoft.Extensions.Logging;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    public class TestRunsImmediatelyFor1Sec
    {

        [Fact]
        public async Task TestMethodToRunAsync()
        {
            //SETUP

            var logs = new List<LogOutput>();
            var logger = new Logger<RunsImmediatelyFor1Sec>(new LoggerFactory(new[] {new MyLoggerProvider(logs)}));
            var bg = new RunsImmediatelyFor1Sec(logger);

            //ATTEMPT
            var start = DateTime.UtcNow;
            var task = bg.MethodToRunAsync();
            await Task.Delay(10);
            logs.Count.ShouldEqual(1);
            await task;
            var end = DateTime.UtcNow;

            //VERIFY
            logs.Count.ShouldEqual(3);
            end.Subtract(start).TotalMilliseconds.ShouldBeInRange(1000, 1200 );
        }
    }
}
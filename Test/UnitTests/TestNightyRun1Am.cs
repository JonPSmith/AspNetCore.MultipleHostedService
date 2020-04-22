// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleBackgroundTasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    public class TestNightyRun1Am
    {
        [Theory] //This triggers at 1am every night
        [InlineData("2019-01-06T00:00:00", "00.01:00:00")] //midnight
        [InlineData("2019-01-06T00:00:10", "00.00:59:50")] //midnight+10second
        [InlineData("2019-01-07T00:30:00", "00.00:30:00")] //0:30am
        [InlineData("2019-01-06T01:30:00", "00.23:30:00")] //1:30am
        [InlineData("2019-01-06T12:00:00", "00.13:00:00")] //12 noon
        public void TestNightyRun1AmDelay(string edtTimeString, string expectedTimeSpanString)
        {
            //SETUP
            var edtTime = DateTime.Parse(edtTimeString);
            var bg = new NightlyRun1Am(new NullLogger<NightlyRun1Am>());

            //ATTEMPT
            var delayTime = bg.TimeToWait(edtTime);

            //VERIFY
            delayTime.ToString("dd\\.hh\\:mm\\:ss").ShouldEqual(expectedTimeSpanString);
        }

        [Fact]
        public async Task TestNightyRun1AmMethodToRunAsync()
        {
            //SETUP
            var logs = new List<LogOutput>();
            var logger = new Logger<NightlyRun1Am>(new LoggerFactory(new[] {new MyLoggerProvider(logs)}));
            var bg = new NightlyRun1Am(logger);

            //ATTEMPT
            await bg.MethodToRunAsync(default);

            //VERIFY
            logs.Single().ToString().ShouldStartWith("Information: I ran at UTC = ");
        }
    }
}
// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MultipleHostedService;

namespace ExampleBackgroundTasks
{
    public class RunsEvery10Seconds : ITaskWithDelayIncluded
    {
        private readonly ILogger<RunsEvery10Seconds> _logger;

        public RunsEvery10Seconds(ILogger<RunsEvery10Seconds> logger)
        {
            _logger = logger;
        }

        public Task MethodToRunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Task ran at UTC {DateTime.UtcNow:T}.");

            return Task.CompletedTask;
        }

        public TimeZoneInfo TimeZoneToUse { get; } = null;

        public TimeSpan TimeToWait(DateTime localTime)
        {
            //This ignores the given time and just sends back a delay of 10 seconds
            return TimeSpan.FromSeconds(10);
        }
    }
}
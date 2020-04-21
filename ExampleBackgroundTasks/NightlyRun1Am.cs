// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MultipleHostedService;

namespace ExampleBackgroundTasks
{
    public class NightlyRun1Am : ITaskWithDelayIncluded
    {
        private const int HourToTriggerOn = 1;
        private readonly ILogger<NightlyRun1Am> _logger;

        public NightlyRun1Am(ILogger<NightlyRun1Am> logger)
        {
            _logger = logger;
            TimeZoneToUse = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public Task MethodToRunAsync()
        {
            var utcNow = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneToUse);
            _logger.LogInformation($"I ran at UTC = {utcNow:s}, local = {localTime:s}");

            return Task.CompletedTask;
        }

        public TimeZoneInfo TimeZoneToUse { get; }

        public TimeSpan TimeToWait(DateTime localTime)
        {
            if (localTime.Hour < HourToTriggerOn)
                //The time is before the time we want it to run, so set the time to the trigger time
                return localTime.TimeOfDay.Subtract(TimeSpan.FromHours(HourToTriggerOn));

            //else it needs to run tomorrow at HourToTriggerOn
            return new TimeSpan(1, 0, 0, 0).Subtract(localTime.TimeOfDay)
                .Add(TimeSpan.FromHours(HourToTriggerOn));
        }
    }
}
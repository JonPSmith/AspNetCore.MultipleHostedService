// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using MultipleHostedService;

namespace ExampleBackgroundTasks
{
    public class DelayOverride2Sec : ICalcDelay
    {
        public TimeZoneInfo TimeZoneToUse { get; } = null;
        public TimeSpan TimeToWait(DateTime localTime)
        {
            return TimeSpan.FromSeconds(2);
        }
    }
}
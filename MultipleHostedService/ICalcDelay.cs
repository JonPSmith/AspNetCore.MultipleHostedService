// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace MultipleHostedService
{
    public interface ICalcDelay
    {
        /// <summary>
        /// This is the timezone you wish to use. The RecurringBackgroundRunner
        /// uses this to work out the local time from DateTime.UtcNow.
        /// If null, then the DateTime.UtcNow time used
        /// </summary>
        TimeZoneInfo TimeZoneToUse { get; }

        /// <summary>
        /// This method should how long a delay should be applied before the task is run
        /// </summary>
        /// <param name="localTime">The RecurringBackgroundRunner provides the current
        /// UTC time converted to your timezone.  The reason for having this parameter is
        /// to allow you to check your delay calculations in a unit test. </param>
        /// <returns></returns>
        TimeSpan TimeToWait(DateTime localTime);
    }
}
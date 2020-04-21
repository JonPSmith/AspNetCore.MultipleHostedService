// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace MultipleHostedService
{
    public interface ICalcDelayTillRunTask
    {
        /// <summary>
        /// This method should how long a delay should be applied before the task is run
        /// </summary>
        /// <param name="utcNow">This is given the value of DateTime.UtcNow by the RecurringBackgroundRunner.
        /// The reason for having this parameter is to allow you to check your delay calculations with  </param>
        /// <returns></returns>
        TimeSpan TimeToWait(DateTime utcNow);
    }
}
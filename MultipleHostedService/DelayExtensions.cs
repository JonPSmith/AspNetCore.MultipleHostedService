// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace MultipleHostedService
{
    public static class DelayExtensions
    {
        /// <summary>
        /// Extension method to convert to your local time.
        /// Useful if you want the background task to run at 1am local time.
        /// </summary>
        /// <param name="utcTime">The utc time you want to convert</param>
        /// <param name="yourTimeZone">The timezone in which you want to covert the utc time to</param>
        /// <returns></returns>
        public static DateTime ConvertUtcToLocalTime(this DateTime utcTime, TimeZoneInfo yourTimeZone)
        {
            if (utcTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("the DateTime was not of type UTC.", nameof(utcTime));

            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, yourTimeZone);
        }
    }
}
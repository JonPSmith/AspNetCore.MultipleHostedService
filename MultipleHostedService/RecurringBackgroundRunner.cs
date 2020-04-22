// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using MultipleHostedService.PublicButHidden;

namespace MultipleHostedService
{
    /// <summary>
    /// This declares a recurring background service that contains the TimeToWait code inside it 
    /// </summary>
    /// <typeparam name="TCombined">Type must have both the task to run repeatedly and the code to provide the TimeToWait calc</typeparam>
    public class RecurringBackgroundRunner<TCombined> 
        : BaseRecurringBackgroundRunner<TCombined, TCombined>
        where TCombined : ITaskToRun, ICalcDelay
    {
        public RecurringBackgroundRunner(IServiceProvider services, TCombined delayCalc,
            ILogger<RecurringBackgroundRunner<TCombined>> logger) 
            : base(services, delayCalc, logger) {}
    }

    /// <summary>
    /// This declares a recurring background service with a separate class for the TimeToWait code
    /// </summary>
    /// <typeparam name="TTaskToRun"></typeparam>
    /// <typeparam name="TDelay"></typeparam>
    public class RecurringBackgroundRunner<TTaskToRun, TDelay>
        : BaseRecurringBackgroundRunner<TTaskToRun, TDelay>
        where TTaskToRun : ITaskToRun where TDelay : ICalcDelay 
    {
        public RecurringBackgroundRunner(IServiceProvider services, TDelay delayCalc,
            ILogger<RecurringBackgroundRunner<TTaskToRun, TDelay>> logger)
            : base(services, delayCalc, logger ) { }
    }
}

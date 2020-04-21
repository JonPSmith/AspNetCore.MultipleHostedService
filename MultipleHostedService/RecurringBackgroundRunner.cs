// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MultipleHostedService
{
    /// <summary>
    /// This declares a recurring background service that contains the TimeToWait code inside it 
    /// </summary>
    /// <typeparam name="TCombined">Type must have both the task to run repeatedly and the code to provide the TimeToWait calc</typeparam>
    public class RecurringBackgroundRunner<TCombined> 
        : RecurringBackgroundRunner<TCombined, TCombined>
        where TCombined : ITaskWithDelayIncluded
    {
        public RecurringBackgroundRunner(IServiceProvider services, TCombined delayCalc,
            ILogger<RecurringBackgroundRunner<TCombined, TCombined>> logger) 
            : base(services, delayCalc, logger) {}
    }

    /// <summary>
    /// This declares a recurring background service with a separate class for the TimeToWait code
    /// </summary>
    /// <typeparam name="TTaskToRun"></typeparam>
    /// <typeparam name="TDelay"></typeparam>
    public class RecurringBackgroundRunner<TTaskToRun, TDelay> : IOneBackgroundService
        where TTaskToRun : IBackgroundTaskToCall where TDelay : ICalcDelayTillRunTask 
    {
        private readonly TDelay _delayCalc;
        private readonly ILogger _logger;

        private readonly IServiceProvider _services;
        private CancellationToken _stopCancellationToken;

        public RecurringBackgroundRunner(IServiceProvider services, TDelay delayCalc,
            ILogger<RecurringBackgroundRunner<TTaskToRun, TDelay>> logger)
        {
            _services = services;
            _delayCalc = delayCalc;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting recurring hosted service {typeof(TTaskToRun).Name}.");

            while (!_stopCancellationToken.IsCancellationRequested)
            {
                var delayTime = _delayCalc.TimeToWait(DateTime.UtcNow);
                await Task.Delay(delayTime, _stopCancellationToken).ConfigureAwait(false);
                if (!_stopCancellationToken.IsCancellationRequested)
                    await GetScopedTaskAndCallAsync(delayTime).ConfigureAwait(false);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "recurring hosted service is stopping.");

            //This stops the loop and the delay
            _stopCancellationToken = new CancellationToken(true);

            return Task.CompletedTask;
        }

        private async Task GetScopedTaskAndCallAsync(TimeSpan delayTime)
        {
            _logger.LogInformation($"Delay was {delayTime:g}. The time is now {DateTime.UtcNow:T}.");

            using (var scope = _services.CreateScope())
            {
                var taskToCall = scope.ServiceProvider.GetRequiredService<TTaskToRun>();
                try
                {
                    await taskToCall.MethodToRunAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(1,"HostedService"),e, $"The task in {typeof(TTaskToRun).Name} failed with an exception." );
                    throw;
                }
            }
        }
    }
}

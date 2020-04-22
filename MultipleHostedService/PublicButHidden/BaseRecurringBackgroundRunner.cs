// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MultipleHostedService.PublicButHidden
{

    /// <summary>
    /// This is the base class that implements the RecurringBackgroundRunner.
    /// Needed this to allow the TDelay part to be overridden
    /// </summary>
    /// <typeparam name="TTaskToRun"></typeparam>
    /// <typeparam name="TDelay"></typeparam>
    public abstract class BaseRecurringBackgroundRunner<TTaskToRun, TDelay> : IOneBackgroundService
        where TTaskToRun : ITaskToRun where TDelay : ICalcDelay 
    {
        private readonly TDelay _delayCalc;
        private readonly ILogger _logger;

        private readonly IServiceProvider _services;
        private CancellationToken _stopCancellationToken;

        public BaseRecurringBackgroundRunner(IServiceProvider services, TDelay delayCalc,
            ILogger logger)
        {
            _services = services;
            _delayCalc = delayCalc;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting recurring hosted service {typeof(TTaskToRun).Name} with delay type of {typeof(TDelay).Name}.");

            while (!_stopCancellationToken.IsCancellationRequested)
            {
                var localTime = ConvertUtcToLocalTime(DateTime.UtcNow, _delayCalc.TimeZoneToUse);
                var delayTime = _delayCalc.TimeToWait(localTime);
                await Task.Delay(delayTime, _stopCancellationToken).ConfigureAwait(false);
                if (!_stopCancellationToken.IsCancellationRequested)
                    await GetServiceAndCallAsync(cancellationToken, delayTime).ConfigureAwait(false);
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

        //-------------------------------------------------------
        //private methods

        private async Task GetServiceAndCallAsync(CancellationToken cancellationToken, TimeSpan delayTime)
        {
            _logger.LogDebug($"Delay was {delayTime:g} provided by {typeof(TDelay).Name}.");

            var taskToCall = _services.GetService<TTaskToRun>();
            if (taskToCall == null)
                _logger.LogError($"The class {typeof(TTaskToRun).Name} was not found as a service.");
            else
            {
                try
                {
                    await taskToCall.MethodToRunAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(1, "HostedService"), e,
                        $"The task in {typeof(TTaskToRun).Name} failed with an exception.");
                    throw;
                }
            }
        }

        private static DateTime ConvertUtcToLocalTime(DateTime utcTime, TimeZoneInfo yourTimeZone)
        {
            if (yourTimeZone == null)
                return utcTime;

            if (utcTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("the DateTime was not of type UTC.", nameof(utcTime));

            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, yourTimeZone);
        }
    }
}

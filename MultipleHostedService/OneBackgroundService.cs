// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultipleHostedService;

namespace BSS.Infrastructure.BackgroundRunners
{
    
    public class OneBackgroundService<TDelay, TTaskToRun> : IOneBackgroundService 
        where TDelay : ICalcDelayTillRunTask where TTaskToRun : IBackgroundTaskToCall
    {
        private readonly TDelay _delayCalc;
        private readonly ILogger _logger;

        private readonly IServiceProvider _services;
        private CancellationToken _stopCancellationToken = new CancellationToken();

        public OneBackgroundService(IServiceProvider services, TDelay delayCalc,
            ILogger<OneBackgroundService<TDelay, TTaskToRun>> logger)
        {
            _services = services;
            _delayCalc = delayCalc;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting hosted service.");

            while (!_stopCancellationToken.IsCancellationRequested)
            {
                var delayTime = _delayCalc.TimeToWait();
                await Task.Delay(delayTime, _stopCancellationToken);
                if (!_stopCancellationToken.IsCancellationRequested)
                    await GetScopedTaskAndCallAsync(delayTime);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            //This stops the loop and the delay
            _stopCancellationToken = new CancellationToken(true);

            return Task.CompletedTask;
        }

        private async Task GetScopedTaskAndCallAsync(TimeSpan delayTime)
        {
            _logger.LogInformation($"Delay was {delayTime:g}. The time is now {DateTime.UtcNow:T}.");

            using (var scope = _services.CreateScope())
            {
                var taskToCall =
                    scope.ServiceProvider.GetRequiredService<TTaskToRun>();

                try
                {
                    await taskToCall.MethodToRunAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(1,"HostedService"),e, $"The task in {taskToCall.GetType().Name} failed with an exception." );
                    throw;
                }
            }
        }
    }
}

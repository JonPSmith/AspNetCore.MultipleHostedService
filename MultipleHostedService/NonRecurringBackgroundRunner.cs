// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MultipleHostedService
{
    public class NonRecurringBackgroundRunner<TTaskToRun> : IOneBackgroundService
        where TTaskToRun : IBackgroundTaskToCall
    {
        private readonly ILogger _logger;

        private readonly IServiceProvider _services;
        private CancellationToken _stopCancellationToken;

        public NonRecurringBackgroundRunner(IServiceProvider services,
            ILogger<NonRecurringBackgroundRunner<TTaskToRun>> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting non-recurring hosted service {typeof(TTaskToRun).Name}.");

            using (var scope = _services.CreateScope())
            {
                var taskToCall = scope.ServiceProvider.GetService<TTaskToRun>();
                if (taskToCall == null)
                    throw new NullReferenceException($"The class {typeof(TTaskToRun).Name} was not found as a service.");
                try
                {
                    await taskToCall.MethodToRunAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(1, "HostedService"), e, $"The task in {taskToCall.GetType().Name} failed with an exception.");
                    throw;
                }
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
    }
}
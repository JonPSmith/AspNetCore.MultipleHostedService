// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MultipleHostedService
{
    /// <summary>
    /// This will run multiple 
    /// </summary>
    public class MultipleHostedServiceRunner : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        //see https://leeconlin.co.uk/blog/recurring-tasks-in-net-core-20-without-a-scheduling-library
        private List<Task> _tasksRunning;

        public MultipleHostedServiceRunner(IServiceProvider services)
        {
            _services = services;
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _tasksRunning = new List<Task>();
            var backgroundServices = _services.GetServices<IOneBackgroundService>();
            return Task.Run(() =>
            {
                //This runs all the background tasks in parallel
                foreach (var backgroundService in backgroundServices)
                {
                    _tasksRunning.Add(backgroundService.StartAsync(_stoppingCts.Token));
                }
            }, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _stoppingCts.Cancel(); //this sets the CancellationToken given to all the tasks to Cancel, i.e. stop
            foreach (var executingTask in _tasksRunning)
            {
                await Task.WhenAny(executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }
    }
}
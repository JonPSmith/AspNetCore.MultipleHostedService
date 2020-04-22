// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MultipleHostedService;

namespace ExampleBackgroundTasks
{
    public class RunsImmediatelyFor10Secs : ITaskToRun
    {
        private readonly ILogger<RunsImmediatelyFor10Secs> _logger;

        public RunsImmediatelyFor10Secs(ILogger<RunsImmediatelyFor10Secs> logger)
        {
            _logger = logger;
        }

        public async Task MethodToRunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("I have started.");
            for (int i = 1; i < 10; i++)
            {
                await Task.Delay(1000);
                _logger.LogInformation($"{i}: I delayed for 1 sec.");
            }

            await Task.Delay(1000);
            _logger.LogInformation("I finished after 10 seconds.");
        }
    }
}
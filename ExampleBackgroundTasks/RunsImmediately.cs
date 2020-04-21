// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MultipleHostedService;

namespace ExampleBackgroundTasks
{
    public class RunsImmediatelyFor1Sec : IBackgroundTaskToCall
    {
        private readonly ILogger<RunsImmediatelyFor1Sec> _logger;

        public RunsImmediatelyFor1Sec(ILogger<RunsImmediatelyFor1Sec> logger)
        {
            _logger = logger;
        }

        public async Task MethodToRunAsync()
        {
            _logger.LogInformation("I have started.");
            await Task.Delay(500);
            _logger.LogInformation("I delayed by 1/2 sec.");
            await Task.Delay(500);
            _logger.LogInformation("I finished after 1 second.");
        }
    }
}
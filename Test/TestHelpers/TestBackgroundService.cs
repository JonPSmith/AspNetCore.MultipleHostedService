// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MultipleHostedService;

namespace Test.TestHelpers
{
    public class TestBackgroundService : IOneBackgroundService
    {
        private readonly int _logValue;
        private readonly ConcurrentStack<int> _logs;

        public TestBackgroundService(int logValue, ConcurrentStack<int> logs)
        {
            _logValue = logValue;
            _logs = logs;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            _logs.Push(_logValue);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
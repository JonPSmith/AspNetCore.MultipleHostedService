// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MultipleHostedService;
using Test.DataLayer;

namespace Test.TestHelpers
{

    public class ScopedDbTask : ITaskToRun
    {
        private readonly IServiceProvider _services;

        public ScopedDbTask(IServiceProvider services)
        {
            _services = services;
        }

        public Task MethodToRunAsync(CancellationToken cancellationToken)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                context.Add(new LogData {MyString = context.InstanceKey});
                context.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}
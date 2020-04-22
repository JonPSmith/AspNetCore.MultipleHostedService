// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleBackgroundTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MultipleHostedService;
using Test.DataLayer;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    public class TestMultipleHostedServiceRunner
    {
        private readonly ITestOutputHelper _output;

        public TestMultipleHostedServiceRunner(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestRegisterIOneBackgroundServicesOk()
        {
            //SETUP
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTransient<RunsEvery10Seconds>();
            services.AddTransient<DelayOverride2Sec>();
            services.AddTransient<NightlyRun1Am>();
            services.AddTransient<RunsImmediatelyFor10Secs>();
            services.AddTransient<IOneBackgroundService, RecurringBackgroundRunner<RunsEvery10Seconds>>();
            services.AddTransient<IOneBackgroundService, RecurringBackgroundRunner<NightlyRun1Am, DelayOverride2Sec>>();//Overrides normal delay
            services.AddTransient<IOneBackgroundService, NonRecurringBackgroundRunner<RunsImmediatelyFor10Secs>>();
            var serviceProvider = services.BuildServiceProvider();

            //ATTEMPT
            var backgroundServices = serviceProvider.GetServices<IOneBackgroundService>().ToList();

            //VERIFY
            backgroundServices.Count.ShouldEqual(3);
        }

        [Fact]
        public async Task TestMultipleHostedServiceRunnerRunOk()
        {
            //SETUP
            var logs = new ConcurrentStack<int>();
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IOneBackgroundService>(x => new TestBackgroundService(1, logs));
            services.AddSingleton<IOneBackgroundService>(x => new TestBackgroundService(2, logs));
            services.AddSingleton<IHostedService, MultipleHostedServiceRunner>();
            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetRequiredService<IHostedService>();

            //ATTEMPT
            await service.StartAsync(default);
            await Task.Delay(1000);

            //VERIFY
            logs.OrderBy(x => x).ToList().ShouldEqual(new List<int> { 1, 2 });
        }

        [Fact]
        public async Task TestMultipleHostedServiceRunnerEachHasScopedServiceProviderOk()
        {
            //SETUP
            var sqliteConnection = SqliteServiceExtensions.SetupSqliteInMemoryConnection();
            using (var context = new MyDbContext(sqliteConnection.GetSqliteOptions()))
            {
                context.Database.EnsureCreated();

                var services = new ServiceCollection();
                services.AddLogging();
                services.AddDbContext<MyDbContext>(options => options.UseSqlite(sqliteConnection));
                services.AddSingleton<ScopedDbTask>();
                services.AddSingleton<IOneBackgroundService, NonRecurringBackgroundRunner<ScopedDbTask>>();
                services.AddSingleton<IOneBackgroundService, NonRecurringBackgroundRunner<ScopedDbTask>>();
                services.AddSingleton<IHostedService, MultipleHostedServiceRunner>();
                var serviceProvider = services.BuildServiceProvider();
                var service = serviceProvider.GetRequiredService<IHostedService>();

                //ATTEMPT
                await service.StartAsync(default);
                await Task.Delay(2000);

                //VERIFY
                var logs = context.Logs.ToList();
                logs.Count.ShouldEqual(2);
                logs[0].ShouldNotEqual(logs[1]);
                var inScope1 = serviceProvider.GetRequiredService<MyDbContext>().InstanceKey;
                var inScope2 = serviceProvider.GetRequiredService<MyDbContext>().InstanceKey;
                inScope1.ShouldEqual(inScope2);
            }
        }


    }
}
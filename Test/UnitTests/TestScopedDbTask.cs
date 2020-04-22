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
    public class TestScopedDbTask
    {
        [Fact]
        public async Task TestScopedDbTaskRunsOk()
        {
            //SETUP
            var sqliteConnection = SqliteServiceExtensions.SetupSqliteInMemoryConnection();
            using (var context = new MyDbContext(sqliteConnection.GetSqliteOptions()))
            {
                context.Database.EnsureCreated();

                //Because the ScopedDbTask uses the IServiceProvider's CreateScope() to get a unique instance of the DbContext
                //then you need to use a service collection/DI to test it.
                var services = new ServiceCollection();
                services.AddLogging();
                services.AddDbContext<MyDbContext>(options => options.UseSqlite(sqliteConnection));
                services.AddSingleton<ScopedDbTask>();
                var serviceProvider = services.BuildServiceProvider();
                var service = serviceProvider.GetRequiredService<ScopedDbTask>();

                //ATTEMPT
                await service.MethodToRunAsync(default);

                //VERIFY
                var logs = context.Logs.ToList();
                logs.Count.ShouldEqual(1);
            }
        }


    }
}
using ExampleBackgroundTasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultipleHostedService;

namespace ExampleWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddTransient<RunsEvery10Seconds>();
                    services.AddTransient<DelayOverride2Sec>();
                    services.AddTransient<RunsImmediatelyFor1Sec>();
                    services.AddTransient<IOneBackgroundService, RecurringBackgroundRunner<RunsEvery10Seconds>>();
                    services.AddTransient<IOneBackgroundService, RecurringBackgroundRunner<RunsEvery10Seconds, DelayOverride2Sec>>();
                    services.AddTransient<IOneBackgroundService, NonRecurringBackgroundRunner<RunsImmediatelyFor1Sec>>();
                    services.AddHostedService<MultipleHostedServiceRunner>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

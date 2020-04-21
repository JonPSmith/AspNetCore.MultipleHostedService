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
                    //Define the Tasks and Delay parts you want to use
                    services.AddTransient<RunsEvery10Seconds>();
                    services.AddTransient<DelayOverride2Sec>();
                    services.AddTransient<NightlyRun1Am>();
                    services.AddTransient<RunsImmediatelyFor1Sec>();
                    //Now register the services as something you want to run via the HostedService
                    services.AddTransient<IOneBackgroundService, RecurringBackgroundRunner<RunsEvery10Seconds>>();
                    services.AddTransient<IOneBackgroundService, RecurringBackgroundRunner<NightlyRun1Am, DelayOverride2Sec>>();//Overrides normal delay
                    services.AddTransient<IOneBackgroundService, NonRecurringBackgroundRunner<RunsImmediatelyFor1Sec>>();
                    //You can only register ONE service to be run by the HostedService
                    //So you register the MultipleHostedServiceRunner which will run in parallel ALL the classes you registered as IOneBackgroundService 
                    services.AddHostedService<MultipleHostedServiceRunner>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

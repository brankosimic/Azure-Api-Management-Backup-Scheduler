using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerArgs;
using System;
using System.Linq;

namespace AzUagBackupsScheduler
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (!args.Any()) throw new ArgException("");

                var context = Args.Parse<Context>(args);

                if(context != null) CreateHostBuilder(args, context).Build().Run();
            }
            catch (ArgException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<Context>());
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, Context context) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(context.GetType(), context);
                    services.AddHostedService<Worker>();
                    Worker.RegisterServices(services, hostContext.Configuration);
                });
    }
}

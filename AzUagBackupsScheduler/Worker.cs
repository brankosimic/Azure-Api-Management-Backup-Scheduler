using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using System.Collections.Specialized;
using Quartz.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace AzUagBackupsScheduler
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly Config _config;
        private readonly IRestore _restore;
        private readonly Context _context;

        public Worker(IConfiguration configuration, IOptionsMonitor<Config> config, 
            IRestore restore, Context context)
        {
            _configuration = configuration;
            _config = config?.CurrentValue;
            _restore = restore;
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_context.BackupServiceMode)
                await ScheduleJob().ConfigureAwait(false);
            else if(_context.RestoreMode)
                await _restore.Execute().ConfigureAwait(false);           
        }

        private async Task ScheduleJob()
        {
            var factory = new StdSchedulerFactory(new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            });

            var sched = await factory.GetScheduler().ConfigureAwait(false);
            sched.JobFactory = new JobFactory(BuildServices());
            await sched.Start().ConfigureAwait(false);

            var job = JobBuilder.Create<BackupJob>().Build();

            var trigger = TriggerBuilder.Create()
                .WithCronSchedule(_config.CronSchedule)
                .Build();

            await sched.ScheduleJob(job, trigger).ConfigureAwait(false);
        }

        private ServiceProvider BuildServices()
        {
            var serviceCollection = new ServiceCollection();
            RegisterServices(serviceCollection, _configuration);
            return serviceCollection.BuildServiceProvider();
        }

        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(configuration);
            });
            services.AddScoped<BackupJob>();
            services.AddSingleton<IAzureAuthentication, AzureAuthentication>();
            services.AddSingleton<IRestore, Restore>();
            services.Configure<Config>(configuration);
        }
    }
}

using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace AzUagBackupsScheduler
{
    public class BackupJob : IJob
    {
        private readonly Config _config;
        private readonly IAzureAuthentication _authentication;
        private readonly ILogger<BackupJob> _logger;
        private const string dateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";

        public BackupJob(IOptionsMonitor<Config> config, IAzureAuthentication authentication, 
            ILogger<BackupJob> logger)
        {
            _config = config?.CurrentValue;
            _authentication = authentication;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Backup Worker shift started.");
            var token = await _authentication.GetToken().ConfigureAwait(false);
            await CreateBackup(token).ConfigureAwait(false);
        }

        private async Task CreateBackup(string token)
        {
            var url = _config.BackupApi.Format(_config.SubscriptionId, _config.ResourceGroupName,
                _config.ServiceName, _config.ApiVersion);

            var body = new BackupInfo
            {
                AccessKey = _config.AccessKey,
                BackupName = $"{_config.ServiceName}_{DateTime.Now.ToString(dateFormat, CultureInfo.InvariantCulture)}.blob",
                ContainerName = _config.ContainerName,
                StorageAccount = _config.StorageAccount
            };

            try
            {
                var res = await url.WithOAuthBearerToken(token).PostJsonAsync(body).ConfigureAwait(false);
                var resMessage = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogInformation("Backup trigger sucessfuly completed. Please note it takes couple of minutes to complete.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create a backup.");
            }
        }
    }
}

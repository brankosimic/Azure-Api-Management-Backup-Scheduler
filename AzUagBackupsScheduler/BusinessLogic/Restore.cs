using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AzUagBackupsScheduler
{
    public class Restore : IRestore
    {
        private readonly Config _config;
        private readonly IAzureAuthentication _authentication;
        private readonly Context _context;
        private readonly ILogger<Restore> _logger;

        public Restore(IOptionsMonitor<Config> config, IAzureAuthentication authentication, Context context, ILogger<Restore> logger)
        {
            _config = config?.CurrentValue;
            _authentication = authentication;
            _context = context;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Restore started.");
            var token = await _authentication.GetToken().ConfigureAwait(false);
            await RestoreIt(token).ConfigureAwait(false);
        }

        private async Task RestoreIt(string token)
        {
            var url = _config.RestoreApi.Format(_config.SubscriptionId, _config.ResourceGroupName,
                _context.ServiceName, _config.ApiVersion);

            var body = new BackupInfo
            {
                AccessKey = _config.AccessKey,
                BackupName = _context.BackupName,
                ContainerName = _config.ContainerName,
                StorageAccount = _config.StorageAccount
            };

            try
            {
                var res = await url.WithOAuthBearerToken(token).PostJsonAsync(body).ConfigureAwait(false);
                var resMessage = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogInformation("Restore trigger sucessfuly completed. Please note it may take up to 30 or more minutes to complete");
                _logger.LogInformation("You can close the app now.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not trigger a restore.");
            }
        }
    }
}

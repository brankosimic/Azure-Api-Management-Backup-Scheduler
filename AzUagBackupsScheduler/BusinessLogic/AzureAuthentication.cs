using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;

namespace AzUagBackupsScheduler
{
    public class AzureAuthentication : IAzureAuthentication
    {
        private readonly Config _config;
        private readonly ILogger<AzureAuthentication> _logger;

        public AzureAuthentication(IOptionsMonitor<Config> config, ILogger<AzureAuthentication> logger)
        {
            _config = config?.CurrentValue;
            _logger = logger;
        }

        public async Task<string> GetToken()
        {
            var credentials = new ClientCredential(_config.ApplicationClientId, _config.ClientSecret);
            var authenticationContext = new AuthenticationContext(_config.AuthApi.Format(_config.TenantId));

            var result = await authenticationContext.AcquireTokenAsync(_config.ResourceScopeApi, credentials)
                .ConfigureAwait(false);

            _logger.LogInformation("Sucessfuly authenticated with Azure.");
            return result.AccessToken;
        }
    }
}

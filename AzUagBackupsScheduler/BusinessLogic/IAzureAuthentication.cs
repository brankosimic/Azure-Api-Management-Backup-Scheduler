using System.Threading.Tasks;

namespace AzUagBackupsScheduler
{
    public interface IAzureAuthentication
    {
        Task<string> GetToken();
    }
}

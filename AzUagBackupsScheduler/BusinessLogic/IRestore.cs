using System.Threading.Tasks;

namespace AzUagBackupsScheduler
{
    public interface IRestore
    {
        Task Execute();
    }
}

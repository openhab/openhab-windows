using System.Threading.Tasks;

namespace OpenHAB.Core.Services
{
    public interface IAppManager
    {
        Task<bool> CanEnableAutostart();

        Task<bool> IsStartupEnabled();

        Task ToggleAutostart();
    }
}
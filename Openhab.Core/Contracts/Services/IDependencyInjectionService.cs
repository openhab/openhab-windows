using Microsoft.Extensions.DependencyInjection;

namespace OpenHAB.Core.Contracts.Services
{
    /// <summary>
    /// Interface for Dependency Injection service.
    /// </summary>
    public interface IDependencyInjectionService
    {
        /// <summary>
        /// Gets the service instance.
        /// </summary>
        /// <value>The instance.</value>
        ServiceProvider Services
        {
            get;
        }
    }
}
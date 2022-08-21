using Microsoft.Extensions.DependencyInjection;

namespace OpenHAB.Core.Contracts.Services
{
    /// <summary>
    /// Interface for Dependency Injection service.
    /// </summary>
    public interface IDependencyInjectionService
    {
        /// <summary>
        /// Gets the a instance of a specif type from the dependency injection service.
        /// </summary>
        /// <value>The services.</value>
        /// <returns>Requested type.</returns>
        T GetService<T>() where T : class;
    }
}
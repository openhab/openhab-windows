namespace openHAB.Core.Services.Contracts
{
    /// <summary>
    /// Interface for Dependency Injection service.
    /// </summary>
    public interface IDependencyInjectionService
    {
        /// <summary>
        /// Gets the a instance of a specif type from the dependency injection service.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The requested service instance.</returns>
        T GetService<T>()
            where T : class;
    }
}
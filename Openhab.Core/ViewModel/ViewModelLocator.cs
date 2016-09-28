using System;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.SDK;
using OpenHAB.Core.Services;

namespace OpenHAB.Core.ViewModel
{
    /// <summary>
    /// Locator object to bind the views to the viewmodels
    /// </summary>
    public class ViewModelLocator : IDisposable
    {
        private IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class.
        /// </summary>
        public ViewModelLocator()
        {
            _container = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(_container));

            RegisterServices();
            RegisterViewModels();
        }

        private void RegisterServices()
        {
            _container.RegisterType<IOpenHAB, SDK.OpenHAB>();
            _container.RegisterType<ISettingsService, SettingsService>();
        }

        private void RegisterViewModels()
        {
            _container.RegisterType<MainViewModel>();
            _container.RegisterType<SettingsViewModel>();
        }

        /// <summary>
        /// Gets the MainViewModel for binding a View's DataContext
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <summary>
        /// Gets the SettingsViewModel for binding a View's DataContext
        /// </summary>
        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        /// <inheritdoc />
        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}

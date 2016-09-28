using System;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using OpenHAB.Core.SDK;

namespace OpenHAB.Windows.ViewModel
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
            _container.RegisterType<IOpenHAB, Core.SDK.OpenHAB>();
        }

        private void RegisterViewModels()
        {
            _container.RegisterType<MainViewModel>();
        }

        /// <summary>
        /// Gets the MainViewModel for binding a View's DataContext
        /// </summary>
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        /// <inheritdoc />
        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}

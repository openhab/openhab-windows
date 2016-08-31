using System;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using OpenHAB.Core.SDK;

namespace OpenHAB.Windows.ViewModel
{
    public class ViewModelLocator : IDisposable
    {
        private IUnityContainer _container;

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

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}

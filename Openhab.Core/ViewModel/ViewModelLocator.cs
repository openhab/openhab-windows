using System;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            RegisterServices();
            RegisterViewModels();
        }

        private void RegisterServices()
        {
            SimpleIoc.Default.Register(() => Messenger.Default);
            SimpleIoc.Default.Register<IOpenHAB, SDK.OpenHAB>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
        }

        private void RegisterViewModels()
        {
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
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
        }
    }
}

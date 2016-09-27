using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Base class for models that need property changed notifications
    /// </summary>
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The event that fires whenever a property changes, comes from INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// A helper method to fire PropertyChanged in a correct way
        /// </summary>
        /// <param name="propertyName">The property that was changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

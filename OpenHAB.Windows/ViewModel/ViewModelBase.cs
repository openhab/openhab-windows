using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenHAB.Core.Common;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// IViewModelModelProp.
    /// </summary>
    /// <typeparam name="T">Model type interface.</typeparam>
    public interface IViewModel<T>
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        T Model
        {
            get;
            set;
        }
    }

    /// <summary>
    /// MVVM ViewModel Base Class.
    /// </summary>
    public class ViewModelBase<T> : DataErrorInfo, INotifyPropertyChanged, IViewModel<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public ViewModelBase(T model)
        {
            Model = model;
        }

        /// <summary>
        /// PropertyChanged Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="TS">Field value type </typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        /// <returns>True when value was updated, else false.</returns>
        public bool Set<TS>(ref TS field, TS value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<TS>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public T Model
        {
            get;
            set;
        }
    }
}

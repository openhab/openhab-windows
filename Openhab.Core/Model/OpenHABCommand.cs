using System;
using System.Windows.Input;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// A bindable command that can be triggered from an OpenHAB widget.
    /// </summary>
    public class OpenHABCommand : ICommand
    {
        private OpenHABWidget _widget;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABCommand"/> class.
        /// </summary>
        /// <param name="widget">The OpenHAB widget that can trigger the command.</param>
        public OpenHABCommand(OpenHABWidget widget)
        {
            _widget = widget;
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            string command = parameter.ToString();
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;
    }
}

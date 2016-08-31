using System;
using System.Windows.Input;

namespace OpenHAB.Core.Model
{
    public class OpenHABCommand : ICommand
    {
        private OpenHABWidget _widget;

        public OpenHABCommand(OpenHABWidget widget)
        {
            _widget = widget;
        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            string command = parameter.ToString();
        }

        public event EventHandler CanExecuteChanged;
    }
}

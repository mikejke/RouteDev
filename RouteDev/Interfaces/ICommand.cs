using System;
using System.Collections.Generic;
using System.Text;

namespace RouteDev.Interfaces
{
    public interface ICommand
    {
        event EventHandler CanExecuteChanged;
        void Execute(object parameter);
        bool CanExecute(object parameter);
    }
}

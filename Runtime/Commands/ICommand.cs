using System;

namespace Rehawk.UIFramework
{
    public interface ICommand
    {
        event EventHandler CanExecuteChanged; 
        
        bool CanExecute(object args);
        void Execute(object args);
    }
}
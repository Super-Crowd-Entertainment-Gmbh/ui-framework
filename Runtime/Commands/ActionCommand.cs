using System;

namespace Rehawk.UIFramework
{
    public delegate void CommandActionDelegate(object args);

    public class ActionCommand : ICommand
    {
        private readonly CommandActionDelegate commandAction;
        
        public event EventHandler CanExecuteChanged;
        
        public ActionCommand(CommandActionDelegate commandAction)
        {
            this.commandAction = commandAction;
        }
        
        public bool CanExecute(object args)
        {
            return true;
        }

        public void Execute(object args)
        {
            commandAction.Invoke(args);
        }

        public static ICommand Create(CommandActionDelegate commandAction)
        {
            return new ActionCommand(commandAction);
        }
    }
}
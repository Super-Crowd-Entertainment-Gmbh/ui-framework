namespace Rehawk.UIFramework
{
    public delegate void CommandActionDelegate(UIControlBase control, ICommandArgs args);
    public delegate void CommandActionDelegate<in T>(UIControlBase control, T args) where T : ICommandArgs;
    public delegate void AnonymousCommandActionDelegate();

    public class ActionCommand : ICommand
    {
        private readonly CommandActionDelegate commandAction;
        
        public ActionCommand(CommandActionDelegate commandAction)
        {
            this.commandAction = commandAction;
        }
        
        public virtual void Execute(UIControlBase control, ICommandArgs args)
        {
            commandAction.Invoke(control, args);
        }

        public static ICommand Create(CommandActionDelegate commandAction)
        {
            return new ActionCommand(commandAction);
        }

        public static ICommand Create<T>(CommandActionDelegate<T> commandAction) where T : ICommandArgs
        {
            return new ActionCommand<T>(commandAction);
        }

        public static ICommand Create(AnonymousCommandActionDelegate commandAction)
        {
            return new AnonymousActionCommand(commandAction);
        }
    }
    
    public class ActionCommand<T> : CommandBase<T> where T : ICommandArgs
    {
        private readonly CommandActionDelegate<T> commandAction;
        
        public ActionCommand(CommandActionDelegate<T> commandAction)
        {
            this.commandAction = commandAction;
        }

        protected override void Execute(UIControlBase control, T args)
        {
            commandAction.Invoke(control, args);
        }
    }
    
    public class AnonymousActionCommand : ICommand
    {
        private readonly AnonymousCommandActionDelegate commandAction;
        
        public AnonymousActionCommand(AnonymousCommandActionDelegate commandAction)
        {
            this.commandAction = commandAction;
        }
        
        public virtual void Execute(UIControlBase control, ICommandArgs args)
        {
            commandAction.Invoke();
        }
    }
}
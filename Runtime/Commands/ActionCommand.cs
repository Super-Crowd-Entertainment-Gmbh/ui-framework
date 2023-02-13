namespace Rehawk.UIFramework
{
    public delegate void CommandActionDelegate(UIControlBase node, ICommandArgs args);
    public delegate void CommandActionDelegate<in T>(UIControlBase node, T args) where T : ICommandArgs;
    public delegate void AnonymousCommandActionDelegate();

    public class ActionCommand : CommandBase
    {
        private readonly CommandActionDelegate commandAction;
        
        public ActionCommand(CommandActionDelegate commandAction)
        {
            this.commandAction = commandAction;
        }
        
        public override void Execute(UIControlBase node, ICommandArgs args)
        {
            commandAction.Invoke(node, args);
        }
    }
    
    public class ActionCommand<T> : CommandBase<T> where T : ICommandArgs
    {
        private readonly CommandActionDelegate<T> commandAction;
        
        public ActionCommand(CommandActionDelegate<T> commandAction)
        {
            this.commandAction = commandAction;
        }

        protected override void Execute(UIControlBase node, T args)
        {
            commandAction.Invoke(node, args);
        }
    }
}
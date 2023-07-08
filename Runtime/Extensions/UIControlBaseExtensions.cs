namespace Rehawk.UIFramework
{
    public static class UIControlBaseExtensions
    {
        public static void SetCommand<TControl>(this TControl control, string commandName, CommandActionDelegate commandAction) where TControl : UIControlBase 
        {
            control.SetCommand(commandName, new ActionCommand(commandAction));
        }
        
        public static void SetCommand<TControl, TCommandArgs>(this TControl control, string commandName, CommandActionDelegate<TCommandArgs> commandAction) where TControl : UIControlBase where TCommandArgs : ICommandArgs
        {
            control.SetCommand(commandName, new ActionCommand<TCommandArgs>(commandAction));
        }
        
        public static void SetCommand<TControl>(this TControl control, string commandName, AnonymousCommandActionDelegate commandAction) where TControl : UIControlBase 
        {
            control.SetCommand(commandName, new ActionCommand((control, args) =>
            {
                commandAction.Invoke();
            }));
        }
    }
}
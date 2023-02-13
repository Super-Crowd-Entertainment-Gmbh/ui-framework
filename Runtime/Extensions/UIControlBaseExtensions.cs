namespace Rehawk.UIFramework
{
    public static class UIControlBaseExtensions
    {
        public static void SetCommand<TNode>(this TNode node, string commandName, CommandActionDelegate commandAction) where TNode : UIControlBase 
        {
            node.SetCommand(commandName, new ActionCommand(commandAction));
        }
        
        public static void SetCommand<TNode, TCommandArgs>(this TNode node, string commandName, CommandActionDelegate<TCommandArgs> commandAction) where TNode : UIControlBase where TCommandArgs : ICommandArgs
        {
            node.SetCommand(commandName, new ActionCommand<TCommandArgs>(commandAction));
        }
        
        public static void SetCommand<TNode>(this TNode node, string commandName, AnonymousCommandActionDelegate commandAction) where TNode : UIControlBase 
        {
            node.SetCommand(commandName, new ActionCommand((_, _) =>
            {
                commandAction.Invoke();
            }));
        }
    }
}
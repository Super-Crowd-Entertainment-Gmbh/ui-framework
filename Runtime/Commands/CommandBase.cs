namespace Rehawk.UIFramework
{
    public abstract class CommandBase<T> : ICommand where T : ICommandArgs
    {
        public void Execute(UIControlBase node, ICommandArgs args)
        {
            Execute(node, (T) args);
        }

        protected abstract void Execute(UIControlBase node, T args);
    }
}
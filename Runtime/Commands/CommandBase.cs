namespace Rehawk.UIFramework
{
    public abstract class CommandBase<T> : ICommand where T : ICommandArgs
    {
        public void Execute(UIControlBase control, ICommandArgs args)
        {
            Execute(control, (T) args);
        }

        protected abstract void Execute(UIControlBase control, T args);
    }
}
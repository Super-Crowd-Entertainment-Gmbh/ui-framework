namespace Rehawk.UIFramework
{
    public abstract class CommandBase
    {
        public abstract void Execute(UIControlBase node, ICommandArgs args);
    }

    public abstract class CommandBase<T> : CommandBase where T : ICommandArgs
    {
        public sealed override void Execute(UIControlBase node, ICommandArgs args)
        {
            Execute(node, (T) args);
        }

        protected abstract void Execute(UIControlBase node, T args);
    }

    public interface ICommandArgs
    {
        public static ICommandArgs Empty
        {
            get { return null; }
        }
    }
}
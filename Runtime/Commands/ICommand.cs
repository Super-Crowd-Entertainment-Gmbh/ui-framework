namespace Rehawk.UIFramework
{
    public interface ICommand
    {
        void Execute(UIControlBase control, ICommandArgs args);
    }
}
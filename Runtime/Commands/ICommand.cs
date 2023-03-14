namespace Rehawk.UIFramework
{
    public interface ICommand
    {
        void Execute(UIControlBase node, ICommandArgs args);
    }
}
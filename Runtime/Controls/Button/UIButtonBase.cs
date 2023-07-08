namespace Rehawk.UIFramework
{
    public abstract class UIButtonBase : UIControlBase
    {
        public abstract bool IsVisible { get; set; }
        public abstract bool IsInteractable { get; set; }

        public abstract ICommand ClickCommand { get; set; }
        public abstract ICommand HoverBeginCommand { get; set; }
        public abstract ICommand HoverEndCommand { get; set; }
    }
}
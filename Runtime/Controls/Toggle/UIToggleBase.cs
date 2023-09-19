namespace Rehawk.UIFramework
{
    public abstract class UIToggleBase : UIControlBase
    {
        public abstract bool IsVisible { get; set; }
        public abstract bool IsInteractable { get; set; }
        
        public abstract bool Value { get; set; }
        
        public abstract ICommand ChangedCommand { get; set; }
    }
}
namespace Rehawk.UIFramework
{
    public abstract class UIInputFieldBase : UIControlBase
    {
        public abstract bool IsVisible { get; set; }

        public abstract bool Enabled { get; set; }
        
        public abstract bool IsInteractable { get; set; }
        
        public abstract string Value { get; set; }
        
        public abstract ICommand ChangedCommand { get; set; }
    }
}
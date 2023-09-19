namespace Rehawk.UIFramework
{
    public abstract class UISliderBase : UIControlBase
    {
        public abstract bool IsVisible { get; set; }
        public abstract bool IsInteractable { get; set; }
        
        public abstract float Value { get; set; }
        public abstract float NormalizedValue { get; set; }
        public abstract float MinValue { get; set; }
        public abstract float MaxValue { get; set; }
        

        public abstract ICommand ChangedCommand { get; set; }
    }
}
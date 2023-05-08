namespace Rehawk.UIFramework
{
    public abstract class UIButtonBase : UIControlBase
    {
        public const string HOVER_BEGIN_COMMAND = "HoverBegin";
        public const string HOVER_END_COMMAND = "HoverEnd";
        public const string CLICK_COMMAND = "Click";

        public abstract bool IsVisible { get; set; }
        public abstract bool IsInteractable { get; set; }
    }
}
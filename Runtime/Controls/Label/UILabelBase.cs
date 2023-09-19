using UnityEngine;

namespace Rehawk.UIFramework
{
    public abstract class UILabelBase : UIControlBase
    {
        public abstract bool IsVisible { get; set; }
        
        public abstract string Text { get; set; }

        public abstract Color Color { get; set; }

        public abstract void SetStrategy(IUILabelTextStrategy strategy);
    }
}
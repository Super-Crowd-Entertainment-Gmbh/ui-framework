using UnityEngine;

namespace Rehawk.UIFramework
{
    public abstract class UILabelBase : UIControlBase
    {
        public abstract string Text { get; set; }

        public abstract Color Color { get; set; }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    public abstract class UIImageBase : UIControlBase
    {
        public abstract bool IsVisible { get; set; }

        public abstract bool Enabled { get; set; }
        
        public abstract Sprite Sprite { get; set; }
        
        public abstract Material Material { get; set; }

        public abstract Color Color { get; set; }

        public abstract float FillAmount { get; set; }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    public abstract class UIRawImageBase : UIControlBase
    {
        public abstract bool IsVisible { get; set; }

        public abstract bool Enabled { get; set; }
        
        public abstract Texture Texture { get; set; }
        
        public abstract Color Color { get; set; }
    }
}
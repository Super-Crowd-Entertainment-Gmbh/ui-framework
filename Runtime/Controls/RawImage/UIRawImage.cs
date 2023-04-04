using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    [RequireComponent(typeof(RawImage))]
    public class UIRawImage : UIRawImageBase
    {
        [SerializeField]
        private RawImage target;

        public override bool Enabled
        {
            get { return target.enabled; }
            set
            {
                if (target.enabled != value)
                {
                    target.enabled = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public override Texture Texture
        {
            get { return target.texture; }
            set
            {
                if (target.texture != value)
                {
                    target.texture = value;
                    OnPropertyChanged();
                }
            }
        }

        public override Color Color
        {
            get { return target.color; }
            set
            {
                if (target.color != value)
                {
                    target.color = value;
                    OnPropertyChanged();
                }
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            
            if (target == null)
            {
                target = GetComponent<RawImage>();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (target == null)
            {
                target = GetComponent<RawImage>();
            }
        }
#endif
    }
}
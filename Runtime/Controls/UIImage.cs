using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    [RequireComponent(typeof(Image))]
    public class UIImage : UIControlBase
    {
        [SerializeField]
        private Image target;

        public bool Enabled
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
        
        public Sprite Sprite
        {
            get { return target.sprite; }
            set
            {
                if (target.sprite != value)
                {
                    target.sprite = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color Color
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

        public float FillAmount
        {
            get { return target.fillAmount; }
            set
            {
                if (target.fillAmount != value)
                {
                    target.fillAmount = value;
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
                target = GetComponent<Image>();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (target == null)
            {
                target = GetComponent<Image>();
            }
        }
#endif
    }
}
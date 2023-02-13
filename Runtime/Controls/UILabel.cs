using TMPro;
using UnityEngine;

namespace Rehawk.UIFramework
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UILabel : UIControlBase
    {
        [SerializeField]
        private TextMeshProUGUI target;
        
        public string Text
        {
            get { return target.text; }
            set
            {
                if (target.text != value)
                {
                    target.text = value;
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

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            
            if (target == null)
            {
                target = GetComponent<TextMeshProUGUI>();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (target == null)
            {
                target = GetComponent<TextMeshProUGUI>();
            }
        }
#endif
    }
}
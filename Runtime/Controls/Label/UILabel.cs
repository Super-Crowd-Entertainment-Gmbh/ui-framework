using TMPro;
using UnityEngine;

namespace Rehawk.UIFramework
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UILabel : UILabelBase
    {
        [SerializeField]
        private TextMeshProUGUI target;
        
        private IUILabelTextStrategy strategy;
        
        public override string Text
        {
            get { return strategy.GetText(this); }
            set
            {
                if (strategy.SetText(this, value))
                {
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

        protected override void Awake()
        {
            base.Awake();

            if (strategy == null)
            {
                SetStrategy(new DefaultUILabelTextStrategy());
            }
        }

        public override void SetStrategy(IUILabelTextStrategy strategy)
        {
            if (this.strategy != null)
            {
                this.strategy.TextChanged -= OnTextChanged;
            }
            
            this.strategy = strategy;
            
            if (this.strategy != null)
            {
                this.strategy.TextChanged += OnTextChanged;
            }
        }

        private void OnTextChanged(string text)
        {
            target.text = text;
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
using TMPro;
using UnityEngine;

namespace Rehawk.UIFramework
{
    [RequireComponent(typeof(TMP_InputField))]
    public class UIInputField : UIControlBase
    {
        [SerializeField]
        private TMP_InputField target;
        
        public string Text
        {
            get { return target.text; }
            set { target.text = value; }
        }

        protected override void Awake()
        {
            base.Awake();

            if (target)
            {
                target.onValueChanged.AddListener(OnTargetTextChanged);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (target)
            {
                target.onValueChanged.RemoveListener(OnTargetTextChanged);
            }
        }

        private void OnTargetTextChanged(string text)
        {
            OnPropertyChanged(nameof(Text));
        }
        
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            
            if (target == null)
            {
                target = GetComponent<TMP_InputField>();
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (target == null)
            {
                target = GetComponent<TMP_InputField>();
            }
        }
#endif
    }
}
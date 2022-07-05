using System;
using Rehawk.UIFramework.Utilities;
using TMPro;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class TextBindingAdapter : SingleBindingAdapterBase
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Mode mode;
        
        private Color defaultLabelColor;

        protected override void Awake()
        {
            base.Awake();

            defaultLabelColor = label.color;
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();
            
            switch (mode)
            {
                case Mode.Text:
                    HandleText();
                    break;
                case Mode.Color:
                    HandleColor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleText()
        {
            label.text = string.Empty;
            
            object value = GetValue<object>(Binding);
            if (!ObjectUtility.IsNull(value))
            {
                label.text = value.ToString();
            } 
        }

        private void HandleColor()
        {
            label.color = defaultLabelColor;
            
            var value = GetValue<Color>(Binding);
            if (!ObjectUtility.IsNull(value))
            {
                label.color = value;
            } 
        }

        public enum Mode
        {
            Text,
            Color
        }
    }
}
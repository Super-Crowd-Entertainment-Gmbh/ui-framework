using System;
using Rehawk.UIFramework.Utilities;
using TMPro;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class TextBindingAdapter : SingleBindingAdapterBase
    {
        [SerializeField] 
        private TextMeshProUGUI[] labels;
        [SerializeField] 
        private Mode mode;
        
        [SerializeField, HideInInspector] 
        private TextMeshProUGUI label;
        
        private Color[] defaultLabelColor;

        protected override void Awake()
        {
            base.Awake();
            
            defaultLabelColor = new Color[labels.Length];
            for (int i = 0; i < labels.Length; i++)
            {
                defaultLabelColor[i] = labels[i].color;
            }
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
            object value = GetValue<object>(Binding);
            
            if (ObjectUtility.IsNull(value))
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    labels[i].text = string.Empty;
                }   
            }
            else
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    labels[i].text = value.ToString();
                }   
            }
        }

        private void HandleColor()
        {
            var value = GetValue<Color>(Binding);
            
            if (ObjectUtility.IsNull(value))
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    labels[i].color = defaultLabelColor[i];
                }   
            }
            else
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    labels[i].color = value;
                }   
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (labels == null || labels.Length <= 0)
            {
                labels = new[]
                {
                    label
                };
            }
        }
#endif
        
        public enum Mode
        {
            Text,
            Color
        }
    }
}
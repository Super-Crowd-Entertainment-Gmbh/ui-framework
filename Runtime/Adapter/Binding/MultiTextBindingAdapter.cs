using Rehawk.UIFramework.Utilities;
using TMPro;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class MultiTextBindingAdapter : MultiBindingAdapterBase
    {
        [SerializeField] private string format;
        
        [SerializeField] 
        private TextMeshProUGUI[] labels;

        [SerializeField, HideInInspector] 
        private TextMeshProUGUI label;

        protected override void OnRefresh()
        {
            base.OnRefresh();

            var args = new object[Bindings.Length];
            for (int i = 0; i < Bindings.Length; i++)
            {
                args[i] = GetValue<object>(Bindings[i]);
            }

            string value = string.Format(format, args);
            
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
                    labels[i].text = value;
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
    }
}
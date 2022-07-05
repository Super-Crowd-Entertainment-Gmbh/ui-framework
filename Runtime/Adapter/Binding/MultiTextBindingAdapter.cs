using TMPro;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class MultiTextBindingAdapter : MultiBindingAdapterBase
    {
        [SerializeField] private string format;
        
        [SerializeField] private TextMeshProUGUI label;

        protected override void OnRefresh()
        {
            base.OnRefresh();

            label.text = string.Empty;

            var args = new object[Bindings.Length];
            for (int i = 0; i < Bindings.Length; i++)
            {
                args[i] = GetValue<object>(Bindings[i]);
            }
            
            label.text = string.Format(format, args);
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework.Adapter
{
    public class ValidationBindingAdapter : BindingAdapter
    {
        [SerializeField] private UnityEvent gotValue;
        [SerializeField] private UnityEvent gotNull;
        
        protected override void OnGotContext()
        {
            object value = GetValue<object>(Binding);
            if (value != null)
            {
                gotValue.Invoke();
            }
            else
            {
                gotNull.Invoke();
            }
        }

        protected override void OnLostContext()
        {
            gotNull.Invoke();
        }
    }
}
using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class ContextBindingAdapter : BindingAdapter
    {
        [SerializeField] private ContextControl control;
        
        protected override void OnGotContext()
        {
            control.SetContext(GetValue<object>(Binding));
        }

        protected override void OnLostContext()
        {
            control.ResetContext();
        }
    }
}
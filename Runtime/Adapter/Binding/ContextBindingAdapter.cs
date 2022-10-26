using Rehawk.UIFramework.Utilities;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ContextBindingAdapter : SingleBindingAdapterBase
    {
        [SerializeField] 
        private ContextControlBase[] controls;
        
        [SerializeField, HideInInspector] 
        private ContextControlBase control;

        protected override void OnRefresh()
        {
            base.OnRefresh();

            object value = GetValue<object>(Binding);

            if (!ObjectUtility.IsNull(value))
            {
                foreach (ContextControlBase control in controls)
                {
                    control.SetContext(value);
                }
            }
            else
            {
                foreach (ContextControlBase control in controls)
                {
                    control.ResetContext();
                }
            }
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (controls == null || controls.Length <= 0)
            {
                controls = new[]
                {
                    control
                };
            }
        }
#endif
    }
}
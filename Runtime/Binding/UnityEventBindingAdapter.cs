using Rehawk.UIFramework.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework
{
    public class UnityEventBindingAdapter : SingleBindingAdapter
    {
        [SerializeField] private UnityEvent onGotValue;
        [SerializeField] private UnityEvent onLostValue;
        [SerializeField] private UnityEvent onValueChanged;

        private bool contextChangedBefore;
        private bool hadValue;
        private object previousValue;
        
        protected override void OnRefresh()
        {
            base.OnRefresh();

            object value = GetValue<object>(Binding);

            if (!ObjectUtility.IsNull(value))
            {
                if ((!contextChangedBefore || !hadValue))
                {
                    onGotValue?.Invoke();
                }
                
                hadValue = true;
            }
            else
            {
                if ((!contextChangedBefore || hadValue))
                {
                    onLostValue?.Invoke();
                }

                hadValue = false;
            }

            if (!ObjectUtility.IsEqual(previousValue, value))
            {
                onValueChanged?.Invoke();
            }

            contextChangedBefore = true;
            previousValue = value;
        }
    }
}
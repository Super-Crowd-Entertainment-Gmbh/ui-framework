using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    public class ButtonBindingAdapter : SingleBindingAdapterBase
    {
        [SerializeField] private Button button;

        protected override void Awake()
        {
            base.Awake();
            
            button.onClick.AddListener(OnClick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            object value = GetValue<object>(Binding);
            if (value is Delegate invokableDelegate)
            {
                invokableDelegate.DynamicInvoke();
            }
        }
    }
}
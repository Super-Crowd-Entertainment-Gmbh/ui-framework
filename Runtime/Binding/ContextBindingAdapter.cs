﻿using Rehawk.UIFramework.Utilities;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ContextBindingAdapter : SingleBindingAdapter
    {
        [SerializeField] private ContextControl control;

        protected override void OnRefresh()
        {
            base.OnRefresh();

            object value = GetValue<object>(Binding);

            if (!ObjectUtility.IsNull(value))
            {
                control.SetContext(value);
            }
            else
            {
                control.ResetContext();
            }
        }
    }
}
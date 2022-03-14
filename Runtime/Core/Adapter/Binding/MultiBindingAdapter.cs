﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public abstract class MultiBindingAdapter : BindingAdapter
    {
        [Binding]
        [ListDrawerSettings(ShowIndexLabels = true)]
        [BoxGroup("ContextBox")]
        [PropertyOrder(-1)]
        [SerializeField] private string[] bindings;

        protected string[] Bindings
        {
            get { return bindings; }
        }
    }
}
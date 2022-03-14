using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public abstract class SingleBindingAdapter : BindingAdapter
    {
        [Binding]
        [PropertyOrder(-1)]
        [BoxGroup("ContextBox")]
        [SerializeField] private string binding;
        [OdinSerialize] private Formatter formatter;

        protected string Binding
        {
            get { return binding; }
        }

        protected override T GetValue<T>(string path, T fallback = default)
        {
            T value = base.GetValue(path, fallback);

            if (formatter != null)
            {
                object formattedValue = formatter.ApplyFormat(value);
                
                if (formattedValue is T castedValue)
                {
                    return castedValue;
                }
            }

            return value;
        }
    }
}
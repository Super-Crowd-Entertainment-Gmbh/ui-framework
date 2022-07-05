using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public abstract class SingleBindingAdapterBase : BindingAdapterBase
    {
        [Binding]
        [PropertyOrder(-1)]
        [BoxGroup("ContextBox")]
        [SerializeField] private string binding;
        [OdinSerialize] private FormatterBase[] formatters = new FormatterBase[0];

        protected string Binding
        {
            get { return binding; }
        }

        protected override T GetValue<T>(string path, T fallback = default)
        {
            object value = base.GetValue(path, fallback);

            if (formatters != null)
            {
                object formattedValue = value;
                
                for (int i = 0; i < formatters.Length; i++)
                {
                    formattedValue = formatters[i].ApplyFormat(formattedValue);
                }
                
                if (formattedValue is T castedValueA)
                {
                    return castedValueA;
                }
            }
            
            if (value is T castedValueB)
            {
                return castedValueB;
            }

            return default;
        }
    }
}
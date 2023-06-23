using System;
using System.Collections.Generic;

namespace Rehawk.UIFramework
{
    public class MultiBindingStrategy : IBindingStrategy
    {
        private readonly List<IBindingStrategy> bindingStrategies = new List<IBindingStrategy>();

        private IMultiValueConverter valueConverter;
        
        private object[] values;
        
        public event EventHandler GotDirty;

        public void SetValueConverter(IMultiValueConverter valueConverter)
        {
            this.valueConverter = valueConverter;
        }
        
        public void AddBindingStrategy(IBindingStrategy bindingStrategy)
        {
            bindingStrategies.Add(bindingStrategy);
            bindingStrategy.GotDirty += OnBindingStrategyGotDirty;
            
            values = new object[bindingStrategies.Count];
        }
        
        public void Evaluate()
        {
            for (int i = 0; i < bindingStrategies.Count; i++)
            {
                bindingStrategies[i].Evaluate();
            }
        }

        public void Release()
        {
            for (int i = 0; i < bindingStrategies.Count; i++)
            {
                bindingStrategies[i].Release();
            }
        }

        public object Get()
        {
            for (int i = 0; i < bindingStrategies.Count; i++)
            {
                values[i] = bindingStrategies[i].Get();
            }

            return valueConverter.Convert(values);
        }

        public void Set(object value)
        {
            values = valueConverter.ConvertBack(value);
            
            for (int i = 0; i < bindingStrategies.Count; i++)
            {
                bindingStrategies[i].Set(values[i]);
            }
        }
        
        private void OnBindingStrategyGotDirty(object sender, EventArgs e)
        {
            GotDirty?.Invoke(this, e);
        }
    }
}
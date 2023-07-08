using System;

namespace Rehawk.UIFramework
{
    public class StaticValueBindingStrategy : IBindingStrategy
    {
        private object value;
        
        public event EventHandler GotDirty;

        public StaticValueBindingStrategy(object value)
        {
            this.value = value;
        }

        public void Evaluate() { }

        public void Release() { }
        
        public object Get()
        {
            return value;
        }

        public void Set(object value)
        {
            if (this.value != value)
            {
                this.value = value;
                GotDirty?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
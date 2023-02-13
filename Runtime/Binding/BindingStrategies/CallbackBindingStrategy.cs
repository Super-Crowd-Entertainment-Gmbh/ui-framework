using System;

namespace Rehawk.UIFramework
{
    public class CallbackBindingStrategy : IBindingStrategy
    {
        private readonly Func<object> getCallback;
        private readonly Action<object> setCallback;
        
        // Has no implementation for that.
        public event Action GotDirty;

        public CallbackBindingStrategy(Func<object> getCallback, Action<object> setCallback)
        {
            this.getCallback = getCallback;
            this.setCallback = setCallback;
        }

        public void Evaluate() { }

        public object Get()
        {
            return getCallback.Invoke();
        }

        public void Set(object value)
        {
            setCallback.Invoke(value);
        }
    }
}
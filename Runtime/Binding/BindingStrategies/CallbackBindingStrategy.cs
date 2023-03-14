using System;

namespace Rehawk.UIFramework
{
    public class CallbackBindingStrategy<T> : IBindingStrategy
    {
        private readonly Func<T> getCallback;
        private readonly Action<T> setCallback;
        
        // Has no implementation for that.
        public event Action GotDirty;

        public CallbackBindingStrategy(Func<T> getCallback, Action<T> setCallback)
        {
            this.getCallback = getCallback;
            this.setCallback = setCallback;
        }

        public void Evaluate() { }

        public void Release() { }
        
        public object Get()
        {
            return getCallback.Invoke();
        }

        public void Set(object value)
        {
            setCallback.Invoke((T) value);
        }
    }
}
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
            if (getCallback != null)
            {
                return getCallback.Invoke();
            }

            return null;
        }

        public void Set(object value)
        {
            if (value != null)
            {
                setCallback.Invoke((T) value);
            }
            else
            {
                setCallback.Invoke(default);
            }
        }
    }
}
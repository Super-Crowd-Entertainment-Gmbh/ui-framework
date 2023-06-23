using System;

namespace Rehawk.UIFramework
{
    public class CallbackBindingStrategy<T> : IBindingStrategy
    {
        private readonly Func<T> getFunction;
        private readonly Action<T> setCallback;
        
        // Has no implementation for that.
        public event EventHandler GotDirty;

        public CallbackBindingStrategy(Func<T> getFunction, Action<T> setCallback)
        {
            this.getFunction = getFunction;
            this.setCallback = setCallback;
        }

        public void Evaluate() { }

        public void Release() { }
        
        public object Get()
        {
            if (getFunction != null)
            {
                return getFunction.Invoke();
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
using System;

namespace Rehawk.UIFramework
{
    public class ContextBindingStrategy : IBindingStrategy
    {
        private readonly Func<UIContextControlBase> getControlFunction;

        private UIContextControlBase control;
        
        public event EventHandler GotDirty;

        public ContextBindingStrategy(Func<UIContextControlBase> getControlFunction)
        {
            this.getControlFunction = getControlFunction;
        }

        public void Evaluate()
        {
            UnLinkFromEvents();

            control = getControlFunction.Invoke();
            
            LinkToEvents();
        }

        public void Release()
        {
            UnLinkFromEvents();
        }

        public object Get()
        {
            if (control != null)
            {
                return control.RawContext;
            }
            
            return null;
        }

        public void Set(object value)
        {
            if (control != null)
            {
                control.SetContext(value);
            }
        }
        
        private void LinkToEvents()
        {
            if (control != null)
            {
                control.ContextChanged += OnControlContextChanged;
            }
        }

        private void UnLinkFromEvents()
        {
            if (control != null)
            {
                control.ContextChanged -= OnControlContextChanged;
            }
        }
        
        private void OnControlContextChanged(object sender, EventArgs e)
        {
            GotDirty?.Invoke(this, e);
        }
    }
}
using System;

namespace Rehawk.UIFramework
{
    public class ContextBindingStrategy : IBindingStrategy
    {
        private readonly Func<UIContextControlBase> getControlCallback;

        private UIContextControlBase control;
        
        public event Action GotDirty;

        public ContextBindingStrategy(Func<UIContextControlBase> getControlCallback)
        {
            this.getControlCallback = getControlCallback;
        }

        public void Evaluate()
        {
            UnLinkFromEvents();

            control = getControlCallback.Invoke();
            
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
            GotDirty?.Invoke();
        }
    }
}
using System;
using System.ComponentModel;

namespace Rehawk.UIFramework
{
    internal class ConnectedProperty
    {
        private readonly Func<INotifyPropertyChanged> getContext;
        private readonly string propertyName;
        private readonly ConnectedPropertyDirection direction;

        private INotifyPropertyChanged context;
        
        public event EventHandler Changed;
            
        public ConnectedProperty(Func<INotifyPropertyChanged> getContext, string propertyName, ConnectedPropertyDirection direction)
        {
            this.getContext = getContext;
            this.propertyName = propertyName;
            this.direction = direction;
        }

        ~ConnectedProperty()
        {
            context.PropertyChanged -= OnContextPropertyChanged;
        }

        public ConnectedPropertyDirection Direction
        {
            get { return direction; }
        }

        public void Evaluate()
        {
            if (context != null)
            {
                context.PropertyChanged -= OnContextPropertyChanged;
            }
            
            context = getContext?.Invoke();

            if (context != null)
            {
                context.PropertyChanged += OnContextPropertyChanged;
            }
        }
        
        private void OnContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == propertyName)
            {
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
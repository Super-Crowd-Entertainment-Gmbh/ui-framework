using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Rehawk.UIFramework
{
    public class ContextPropertyBindingStrategy : IBindingStrategy
    {
        private readonly Func<object> getContext;
        private readonly string propertyName;

        private PropertyInfo propertyInfo;
        
        private object context;
        private object value;
        
        public event Action GotDirty;

        public ContextPropertyBindingStrategy(Func<object> getContext, string propertyName)
        {
            this.getContext = getContext;
            this.propertyName = propertyName;
        }

        public void Evaluate()
        {
            UnLinkINotifyPropertyChanged();
            
            context = getContext?.Invoke();
            
            if (context != null && !string.IsNullOrEmpty(propertyName))
            {
                propertyInfo = context.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            }

            value = Get();

            LinkINotifyPropertyChanged();
        }

        public object Get()
        {
            object result = context;
            
            if (context != null && propertyInfo != default)
            {
                result = propertyInfo.GetValue(context, null);
            }

            return result;
        }

        public void Set(object value)
        {
            if (context != null && propertyInfo != default)
            {
                propertyInfo.SetValue(context, value);
            }
            else if (context is UIContextControlBase contextNode)
            {
                contextNode.SetContext(value);
            }
        }
        
        private void LinkINotifyPropertyChanged()
        {
            if (value is INotifyCollectionChanged valueNotifyCollectionChanged)
            {
                valueNotifyCollectionChanged.CollectionChanged += OnValueCollectionChanged;
            }
            else if (value is INotifyPropertyChanged valueNotifyPropertyChanged)
            {
                valueNotifyPropertyChanged.PropertyChanged += OnValuePropertyChanged;
            }
            
            if (context is INotifyCollectionChanged contextNotifyCollectionChanged)
            {
                contextNotifyCollectionChanged.CollectionChanged += OnContextCollectionChanged;
            }
            else if (context is INotifyPropertyChanged contextNotifyPropertyChanged)
            {
                contextNotifyPropertyChanged.PropertyChanged += OnContextPropertyChanged;
            }
        }

        private void UnLinkINotifyPropertyChanged()
        {
            if (value is INotifyCollectionChanged valueNotifyCollectionChanged)
            {
                valueNotifyCollectionChanged.CollectionChanged -= OnValueCollectionChanged;
            }
            else if (value is INotifyPropertyChanged valueNotifyPropertyChanged)
            {
                valueNotifyPropertyChanged.PropertyChanged -= OnValuePropertyChanged;
            }

            if (context is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged -= OnContextCollectionChanged;
            }
            
            if (context is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged -= OnContextPropertyChanged;
            }
        }

        private void OnValueCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GotDirty?.Invoke();
        }

        private void OnValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GotDirty?.Invoke();
        }
        
        private void OnContextCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GotDirty?.Invoke();
        }

        private void OnContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(propertyName) || e.PropertyName == propertyName)
            {
                GotDirty?.Invoke();
            }
        }
    }
}
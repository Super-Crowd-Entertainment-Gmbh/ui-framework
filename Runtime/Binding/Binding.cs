using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Rehawk.UIFramework
{
    public class Binding
    {
        private readonly object parent;
        
        private IBindingStrategy sourceStrategy;
        private IBindingStrategy destinationStrategy;
        
        private IValueConverter converter;
        private BindingDirection direction;

        private readonly List<ConnectedProperty> connectedProperties = new List<ConnectedProperty>();

        public Binding() { }
        
        public Binding(object parent)
        {
            this.parent = parent;
        }

        public object Parent
        {
            get { return parent; }
        }

        public IBindingStrategy SourceStrategy
        {
            get { return sourceStrategy; }
        }
        
        public IBindingStrategy DestinationStrategy
        {
            get { return destinationStrategy; }
        }

        public void Release()
        {
            if (sourceStrategy != null)
            {
                sourceStrategy.Release();
            }    
            
            if (destinationStrategy != null)
            {
                destinationStrategy.Release();
            }    
        }
        
        internal void SetSource(IBindingStrategy strategy)
        {
            if (sourceStrategy != null)
            {
                sourceStrategy.GotDirty -= OnSourceGotDirty;
            }
            
            sourceStrategy = strategy;
            sourceStrategy.Evaluate();
            
            if (sourceStrategy != null)
            {
                sourceStrategy.GotDirty += OnSourceGotDirty;
            }
        }

        internal void SetDestination(IBindingStrategy strategy)
        {
            if (destinationStrategy != null)
            {
                destinationStrategy.GotDirty -= OnDestinationGotDirty;
            }
            
            destinationStrategy = strategy;
            destinationStrategy.Evaluate();
            
            if (destinationStrategy != null)
            {
                destinationStrategy.GotDirty += OnDestinationGotDirty;
            }
        }
        
        internal void SetConverter(IValueConverter converter)
        {
            this.converter = converter;
        }

        private void SetDirection(BindingDirection direction)
        {
            this.direction = direction;
        }

        internal void Evaluate()
        {
            sourceStrategy.Evaluate();
            destinationStrategy.Evaluate();

            for (int i = 0; i < connectedProperties.Count; i++)
            {
                connectedProperties[i].Evaluate();
            }
        }
        
        internal void SourceToDestination()
        {
            object value = sourceStrategy.Get();
            
            if (converter != default)
            {
                value = converter.Convert(value);
            }
            
            destinationStrategy.Set(value);
        }

        private void DestinationToSource()
        {
            object value = destinationStrategy.Get();
            
            if (converter != null)
            {
                value = converter.ConvertBack(value);
            }
            
            sourceStrategy.Set(value);
        }

        internal void ConnectTo(Func<INotifyPropertyChanged> getContext, string propertyName, ConnectedPropertyDirection direction = ConnectedPropertyDirection.SourceToDestination)
        {
            var connectedProperty = new ConnectedProperty(getContext, propertyName, direction);
            connectedProperty.Changed += OnConnectedPropertyChanged;
            
            connectedProperties.Add(connectedProperty);
        }

        private void OnSourceGotDirty()
        {
            SourceToDestination();
        }

        private void OnDestinationGotDirty()
        {
            if (direction == BindingDirection.TwoWay)
            {
                DestinationToSource();
            }
        }
        
        private void OnConnectedPropertyChanged(object sender, EventArgs eventArgs)
        {
            Evaluate();
            
            if (sender is ConnectedProperty connectedProperty)
            {
                switch (connectedProperty.Direction)
                {
                    case ConnectedPropertyDirection.SourceToDestination:
                        SourceToDestination();
                        break;
                    case ConnectedPropertyDirection.DestinationToSource:
                        DestinationToSource();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        
        public static Binding Bind<T>(object parent, Func<object> getContext, Expression<Func<T>> memberExpression, BindingDirection direction = BindingDirection.OneWay)
        {
            var binding = new Binding(parent);

            binding.SetSource(new ContextMemberPathBindingStrategy(getContext, MemberPath.Get(memberExpression)));
            binding.SetDirection(direction);
            
            return binding;
        }

        /// <summary>
        /// Creates a new binding.
        /// </summary>
        /// <param name="getContext">Should return the context object. If it's type implements <see cref="System.ComponentModel.INotifyPropertyChanged"/> or <see cref="System.Collections.Specialized.INotifyCollectionChanged"/> it will react when the context changes.</param>
        /// <param name="propertyName">Can be provided to distinguish context and data source from each other. Helpful in cases where the data source doesn't implements <see cref="System.ComponentModel.INotifyPropertyChanged"/> or <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>. If it's null or empty, the context is the data source.</param>
        /// <param name="direction">Can be provided to set the direction of the binding.</param>
        public static Binding BindProperty(object parent, Func<object> getContext, string propertyName, BindingDirection direction = BindingDirection.OneWay)
        {
            var binding = new Binding(parent);
            
            binding.SetSource(new ContextPropertyBindingStrategy(getContext, propertyName));
            binding.SetDirection(direction);
            
            return binding;
        }
        
        public static Binding BindContext(object parent, Func<UIContextControlBase> getControlCallback, BindingDirection direction = BindingDirection.OneWay)
        {
            var binding = new Binding(parent);

            binding.SetSource(new ContextBindingStrategy(getControlCallback));
            binding.SetDirection(direction);

            return binding;
        }
        
        public static Binding BindCallback<T>(object parent, Func<T> getCallback, Action<T> setCallback, BindingDirection direction = BindingDirection.OneWay)
        {
            var binding = new Binding(parent);
            
            binding.SetSource(new CallbackBindingStrategy<T>(getCallback, setCallback));
            binding.SetDirection(direction);
            
            return binding;
        }
        
        public static Binding BindCallback<T>(object parent, Func<T> getCallback)
        {
            return BindCallback<T>(parent, getCallback, _ => { });
        }
    }
}
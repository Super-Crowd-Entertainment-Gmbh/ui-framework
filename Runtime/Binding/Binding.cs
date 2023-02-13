using System;

// TODO: TwoWay binding would be a nice addition.

namespace Rehawk.UIFramework
{
    public class Binding
    {
        private IBindingStrategy sourceStrategy;
        private IBindingStrategy destinationStrategy;
        
        private IValueConverter converter;
        private BindingDirection direction;

        public void SetOrigin(IBindingStrategy strategy)
        {
            if (sourceStrategy != null)
            {
                sourceStrategy.GotDirty -= OnOriginGotDirty;
            }
            
            sourceStrategy = strategy;
            sourceStrategy.Evaluate();
            
            if (sourceStrategy != null)
            {
                sourceStrategy.GotDirty += OnOriginGotDirty;
            }
        }

        public void SetDestination(IBindingStrategy strategy)
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
        
        public void SetConverter(IValueConverter converter)
        {
            this.converter = converter;
        }

        public void SetDirection(BindingDirection direction)
        {
            this.direction = direction;
        }

        public void Evaluate()
        {
            sourceStrategy.Evaluate();
            destinationStrategy.Evaluate();
        }
        
        public void OriginToDestination()
        {
            object value = sourceStrategy.Get();
            
            if (converter != default)
            {
                value = converter.Convert(value);
            }
            
            destinationStrategy.Set(value);
        }

        public void DestinationToOrigin()
        {
            object value = destinationStrategy.Get();
            
            if (converter != null)
            {
                value = converter.ConvertBack(value);
            }
            
            sourceStrategy.Set(value);
        }

        private void OnOriginGotDirty()
        {
            OriginToDestination();
        }

        private void OnDestinationGotDirty()
        {
            if (direction == BindingDirection.TwoWay)
            {
                DestinationToOrigin();
            }
        }

        
        /// <summary>
        /// Creates a new binding.
        /// </summary>
        /// <param name="getContext">Should return the context object. If it's type implements <see cref="System.ComponentModel.INotifyPropertyChanged"/> or <see cref="System.Collections.Specialized.INotifyCollectionChanged"/> it will react when the context changes.</param>
        /// <param name="propertyName">Can be provided to distinguish context and data source from each other. Helpful in cases where the data source doesn't implements <see cref="System.ComponentModel.INotifyPropertyChanged"/> or <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>. If it's null or empty, the context is the data source.</param>
        /// <param name="direction">Can be provided to set the direction of the binding.</param>
        public static Binding Bind(Func<object> getContext, string propertyName = null, BindingDirection direction = BindingDirection.OneWay)
        {
            var binding = new Binding();
            
            binding.SetOrigin(new ContextPropertyBindingStrategy(getContext, propertyName));
            binding.SetDirection(direction);
            
            return binding;
        }
    }
}
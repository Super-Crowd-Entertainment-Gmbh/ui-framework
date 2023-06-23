using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Rehawk.UIFramework
{
    public static class BindingExtensions
    {
        /// <summary>
        /// Defines an <see cref="IValueConverter"/> by which the value of the data source can be converted into another type.
        /// </summary>
        public static Binding Converted(this Binding binding, IValueConverter converter) 
        {
            binding.SetConverter(converter);
            
            return binding;
        }
        
        /// <summary>
        /// Defines a <see cref="ValueConvertFunctionDelegate"/> by which the value of the data source can be converted into another type.
        /// </summary>
        public static Binding Converted(this Binding binding, ValueConvertFunctionDelegate converterFunction) 
        {
            binding.SetConverter(new FunctionConverter(converterFunction));
            
            return binding;
        }
        
        /// <summary>
        /// Defines a <see cref="ValueConvertFunctionDelegate{T}"/> by which the value of the data source can be converted into another type.
        /// </summary>
        public static Binding Converted<T>(this Binding binding, ValueConvertFunctionDelegate<T> converterFunction) 
        {
            binding.SetConverter(new FunctionConverter(value =>
            {
                if (value != null)
                {
                    return converterFunction.Invoke((T) value);
                }
                else
                {
                    return converterFunction.Invoke(default);
                }
            }));
            
            return binding;
        }
        
        public static Binding As<T>(this Binding binding)
        {
            binding.Converted(input =>
            {
                if (input != null)
                {
                    return (T) input;
                }

                return default(T);
            });
            
            return binding;
        }

        public static Binding AsBool(this Binding binding)
        {
            binding.As<bool>();
            
            return binding;
        }

        public static Binding AsInt(this Binding binding)
        {
            binding.Converted(input =>
            {
                string inputStr = input?.ToString();
                
                if (int.TryParse(inputStr, out int result))
                {
                    return result;
                }

                return 0;
            });
            
            return binding;
        }

        public static Binding AsFloat(this Binding binding)
        {
            binding.Converted(input =>
            {
                string inputStr = input?.ToString();
                
                if (float.TryParse(inputStr, out float result))
                {
                    return result;
                }

                return 0f;
            });
            
            return binding;
        }

        public static Binding AsString(this Binding binding)
        {
            binding.Converted(input => input?.ToString());
            
            return binding;
        }

        public static Binding Inverted(this Binding binding)
        {
            binding.Converted(input =>
            {
                if (input != null)
                {
                    return !((bool)input);
                }
                
                // Null is processed like false
                
                return true;
            });
            
            return binding;
        }

        public static Binding To<T>(this Binding binding, Expression<Func<T>> memberExpression) 
        {
            binding.SetSource(new MemberBindingStrategy(() => binding.Parent, MemberPath.Get(memberExpression)));
            
            return binding;
        }

        /// <summary>
        /// Defines the destination of the binding which will be reevaluated each time the binding gets dirty.
        /// </summary>
        /// <param name="getContextFunction">Should return the context object.</param>
        /// <param name="propertyName">Can be provided to distinguish context and data destination from each other.</param>
        public static Binding ToProperty(this Binding binding, Func<object> getContextFunction, string propertyName) 
        {
            binding.SetSource(new ContextPropertyBindingStrategy(getContextFunction, propertyName));
            
            return binding;
        }
        
        public static Binding ToContext(this Binding binding, Func<UIContextControlBase> getControlFunction)
        {
            binding.SetSource(new ContextBindingStrategy(getControlFunction));

            return binding;
        }

        public static Binding ToCallback<T>(this Binding binding, Func<T> getFunction)
        {
            return binding.ToCallback(getFunction, _ => {});
        }
        
        public static Binding ToCallback<T>(this Binding binding, Func<T> getFunction, Action<T> setCallback)
        {
            binding.SetSource(new CallbackBindingStrategy<T>(getFunction, setCallback));

            return binding;
        }
        
        public static Binding ToMultiple<T>(this Binding binding, Expression<Func<T>> memberExpression)
        {
            MultiBindingStrategy multiBindingStrategy = ReplaceSourceWithMultiBindingStrategy(binding);
            
            multiBindingStrategy.AddBindingStrategy(new MemberBindingStrategy(() => binding.Parent, MemberPath.Get(memberExpression)));

            return binding;
        }

        public static Binding ToMultipleProperty(this Binding binding, Func<object> getContextFunction, string propertyName)
        {
            MultiBindingStrategy multiBindingStrategy = ReplaceSourceWithMultiBindingStrategy(binding);
            
            multiBindingStrategy.AddBindingStrategy(new ContextPropertyBindingStrategy(getContextFunction, propertyName));

            return binding;
        }

        public static Binding ToMultipleCallback<T>(this Binding binding, Func<T> getFunction)
        {
            return binding.ToMultipleCallback(getFunction, _ => { });
        }

        public static Binding ToMultipleCallback<T>(this Binding binding, Func<T> getFunction, Action<T> setCallback)
        {
            MultiBindingStrategy multiBindingStrategy = ReplaceSourceWithMultiBindingStrategy(binding);
            
            multiBindingStrategy.AddBindingStrategy(new CallbackBindingStrategy<T>(getFunction, setCallback));

            return binding;
        }

        public static Binding Combined(this Binding binding, IMultiValueConverter valueConverter)
        {
            MultiBindingStrategy multiBindingStrategy = ReplaceSourceWithMultiBindingStrategy(binding);
            
            multiBindingStrategy.SetValueConverter(valueConverter);

            return binding;
        }

        public static Binding Combined(this Binding binding, MultiValueConvertFunctionDelegate valueConverter)
        {
            return binding.Combined(new FunctionMultiConverter(valueConverter));
        }

        public static Binding ListenTo<T>(this Binding binding, Expression<Func<T>> memberExpression, BindingConnectionDirection direction = BindingConnectionDirection.SourceToDestination) 
        {
            binding.ConnectTo(memberExpression, direction);
            
            return binding;
        }
        
        public static Binding ListenTo(this Binding binding, Func<INotifyPropertyChanged> getContextFunction, string propertyName, BindingConnectionDirection direction = BindingConnectionDirection.SourceToDestination) 
        {
            binding.ConnectTo(getContextFunction, propertyName, direction);
            
            return binding;
        }
        
        private static MultiBindingStrategy ReplaceSourceWithMultiBindingStrategy(Binding binding)
        {
            if (binding.SourceStrategy is MultiBindingStrategy multiBindingStrategy)
            {
                // Do nothing.
            }
            else if (binding.SourceStrategy != null)
            {
                IBindingStrategy previousSourceStrategy = binding.SourceStrategy;
                multiBindingStrategy = new MultiBindingStrategy();
                multiBindingStrategy.AddBindingStrategy(previousSourceStrategy);
                
                binding.SetSource(multiBindingStrategy);
            }
            else
            {
                multiBindingStrategy = new MultiBindingStrategy();
                
                binding.SetSource(multiBindingStrategy);
            }

            return multiBindingStrategy;
        }
    }
}
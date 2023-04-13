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
                return converterFunction.Invoke((T) value);
            }));
            
            return binding;
        }
        
        public static Binding As<T>(this Binding binding)
        {
            binding.Converted(input => (T) input);
            
            return binding;
        }

        public static Binding AsBool(this Binding binding)
        {
            binding.Converted(input => (bool) input);
            
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
            binding.Converted(input => !((bool) input));
            
            return binding;
        }

        public static Binding To<T>(this Binding binding, Expression<Func<T>> memberExpression) 
        {
            binding.SetDestination(new ContextMemberPathBindingStrategy(() => binding.Parent, MemberPath.Get(memberExpression)));
            
            return binding;
        }

        /// <summary>
        /// Defines the destination of the binding which will be reevaluated each time the binding gets dirty.
        /// </summary>
        /// <param name="getContext">Should return the context object.</param>
        /// <param name="propertyName">Can be provided to distinguish context and data destination from each other.</param>
        public static Binding ToProperty(this Binding binding, Func<object> getContext, string propertyName) 
        {
            binding.SetDestination(new ContextPropertyBindingStrategy(getContext, propertyName));
            
            return binding;
        }
        
        public static Binding ToContext(this Binding binding, Func<UIContextControlBase> getControlCallback)
        {
            binding.SetDestination(new ContextBindingStrategy(getControlCallback));

            return binding;
        }
        
        public static Binding ToList(this Binding binding, Func<UIList> getListCallback)
        {
            binding.ToContext(getListCallback);

            return binding;
        }


        public static Binding ToCallback<T>(this Binding binding, Action<T> setCallback)
        {
            binding.SetDestination(new CallbackBindingStrategy<T>(() => default, setCallback));

            return binding;
        }
        
        public static Binding ToCallback<T>(this Binding binding, Func<T> getCallback, Action<T> setCallback)
        {
            binding.SetDestination(new CallbackBindingStrategy<T>(getCallback, setCallback));

            return binding;
        }
        
        public static Binding ConnectedTo(this Binding binding, Func<INotifyPropertyChanged> getContext, string propertyName, ConnectedPropertyDirection direction = ConnectedPropertyDirection.SourceToDestination) 
        {
            binding.ConnectTo(getContext, propertyName, direction);
            
            return binding;
        }
    }
}
using System;

namespace Rehawk.UIFramework
{
    public static class BindingExtensions
    {
        /// <summary>
        /// Defines an <see cref="IValueConverter"/> by which the value of the data source can be converted into another type.
        /// </summary>
        public static Binding Through(this Binding binding, IValueConverter converter) 
        {
            binding.SetConverter(converter);
            
            return binding;
        }
        
        /// <summary>
        /// Defines a <see cref="ValueConvertFunctionDelegate"/> by which the value of the data source can be converted into another type.
        /// </summary>
        public static Binding Through(this Binding binding, ValueConvertFunctionDelegate converterFunction) 
        {
            binding.SetConverter(new FunctionConverter(converterFunction));
            
            return binding;
        }
        
        /// <summary>
        /// Defines the destination of the binding which will be reevaluated each time the binding gets dirty.
        /// </summary>
        /// <param name="getContext">Should return the context object.</param>
        /// <param name="propertyName">Can be provided to distinguish context and data destination from each other. If it's null or empty, the context is the data destination.</param>
        public static Binding To(this Binding binding, Func<object> getContext, string propertyName = null) 
        {
            binding.SetDestination(new ContextPropertyBindingStrategy(getContext, propertyName));
            
            return binding;
        }
        
        public static Binding To<T>(this Binding binding, Action<T> setCallback)
        {
            binding.SetDestination(new CallbackBindingStrategy(() =>
            {
                return default(T);
            }, value =>
            {
                setCallback.Invoke((T) value);
            }));

            return binding;
        }
        
        public static Binding To<T>(this Binding binding, Func<T> getCallback, Action<T> setCallback)
        {
            binding.SetDestination(new CallbackBindingStrategy(() =>
            {
                return getCallback.Invoke();
            }, value =>
            {
                setCallback.Invoke((T) value);
            }));

            return binding;
        }
        
        /// <summary>
        /// Completes the binding and executes <see cref="Binding.OriginToDestination"/> to initial run the binding.
        /// </summary>
        public static void Complete(this Binding binding) 
        {
            binding.OriginToDestination();
        }
    }
}
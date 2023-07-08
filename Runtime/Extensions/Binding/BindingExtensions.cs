using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Rehawk.UIFramework
{
    public static class BindingExtensions
    {
        public static Binding ToMember<T>(this Binding binding, Expression<Func<T>> memberExpression)
        {
            var bindingStrategy = new MemberBindingStrategy(() => binding.Parent, MemberPath.Get(memberExpression));

            MultiBindingHelper.AddSourceStrategy(binding, bindingStrategy);
            
            return binding;
        }

        public static Binding ToProperty(this Binding binding, Func<object> getContextFunction, string propertyName) 
        {
            var bindingStrategy = new ContextPropertyBindingStrategy(getContextFunction, propertyName);

            MultiBindingHelper.AddSourceStrategy(binding, bindingStrategy);

            return binding;
        }
        
        public static Binding ToContext(this Binding binding, Func<UIContextControlBase> getControlFunction)
        {
            var bindingStrategy = new ContextBindingStrategy(getControlFunction);

            MultiBindingHelper.AddSourceStrategy(binding, bindingStrategy);

            return binding;
        }

        public static Binding ToCallback<T>(this Binding binding, Func<T> getFunction)
        {
            return binding.ToCallback(getFunction, _ => {});
        }
        
        public static Binding ToCallback<T>(this Binding binding, Func<T> getFunction, Action<T> setCallback)
        {
            var bindingStrategy = new CallbackBindingStrategy<T>(getFunction, setCallback);

            MultiBindingHelper.AddSourceStrategy(binding, bindingStrategy);

            return binding;
        }
        
        public static Binding ToValue<T>(this Binding binding, T value)
        {
            var bindingStrategy = new StaticValueBindingStrategy(value);

            MultiBindingHelper.AddSourceStrategy(binding, bindingStrategy);

            return binding;
        }
        
        public static Binding ToCustom(this Binding binding, IBindingStrategy bindingStrategy)
        {
            MultiBindingHelper.AddSourceStrategy(binding, bindingStrategy);

            return binding;
        }
    }
}
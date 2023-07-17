using System;

namespace Rehawk.UIFramework
{
    public static class BindingDoExtensions
    {
        public static Binding Do(this Binding binding, Action callback)
        {
            binding.Evaluated += callback;
            
            return binding;
        }
    }
}
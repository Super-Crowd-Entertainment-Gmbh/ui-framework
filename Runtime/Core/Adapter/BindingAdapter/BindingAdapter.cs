using System;
using System.Reflection;
using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public abstract class BindingAdapter : Adapter<ContextControl>
    {
        [SerializeField] private string binding;
        
        private bool hadContext;

        public string Binding
        {
            get { return binding; }
        }
        
        protected override void OnRefresh()
        {
            base.OnRefresh();

            if (Control.HasContext)
            {
                if (!hadContext)
                {
                    OnGotContext();
                }
            }
            else
            {
                if (hadContext)
                {
                    OnLostContext();
                }
            }

            hadContext = Control.HasContext;
        }

        protected abstract void OnGotContext();
        protected abstract void OnLostContext();
        
        protected T GetValue<T>(string path, T fallback = default)
        {
            object context = Control.GetContext();
            Type contextType = context.GetType();

            object value = null;
            
            FieldInfo fieldInfo = contextType.GetField(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                value = fieldInfo.GetValue(context);
            }
            else
            {
                PropertyInfo propertyInfo = contextType.GetProperty(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (propertyInfo != null)
                {
                    value = propertyInfo.GetValue(context);
                }
            }

            return value != null ? (T) value : fallback;
        }
    }
}
using Rehawk.UIFramework.Utilities;

namespace Rehawk.UIFramework
{
    public abstract class BindingAdapter : Adapter<Control>
    {
        protected virtual T GetValue<T>(string path, T fallback = default)
        {
            if (HasControl)
            {
                object source = Control;
                
                if (source is ContextControl contextControl)
                {
                    source = contextControl.GetContext();
                }
                
                if (!ObjectUtility.IsNull(source))
                {
                    object value = BindingUtility.GetValueByPath(source, path);
                    if (value is T castedValue)
                    {
                        return castedValue;
                    }
                }
            }

            return fallback;
        }
    }
}
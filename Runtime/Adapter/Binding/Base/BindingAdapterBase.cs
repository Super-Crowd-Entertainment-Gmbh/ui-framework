using Rehawk.UIFramework.Utilities;

namespace Rehawk.UIFramework
{
    public abstract class BindingAdapterBase : ControlAdapter<ControlBase>
    {
        protected virtual T GetValue<T>(string path, T fallback = default)
        {
            if (HasControl)
            {
                object source = Control;

                bool isReferencingControl = path.StartsWith("_Control");

                if (isReferencingControl)
                {
                    path = path.Substring(8);
                }
                else if (source is ContextControlBase contextControl)
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
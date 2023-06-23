using System;

namespace Rehawk.UIFramework
{
    public interface IBindingStrategy
    {
        event EventHandler GotDirty;

        void Evaluate();
        void Release();
        
        object Get();
        void Set(object value);
    }
}
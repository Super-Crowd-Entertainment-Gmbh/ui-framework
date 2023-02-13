using System;

namespace Rehawk.UIFramework
{
    public interface IBindingStrategy
    {
        event Action GotDirty;

        void Evaluate();
        
        object Get();
        void Set(object value);
    }
}
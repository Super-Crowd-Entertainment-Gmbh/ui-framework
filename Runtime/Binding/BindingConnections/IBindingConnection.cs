using System;

namespace Rehawk.UIFramework
{
    public interface IBindingConnection
    {
        event EventHandler Changed;
        
        BindingConnectionDirection Direction { get; }
        
        void Evaluate();
        void Release();
    }
}
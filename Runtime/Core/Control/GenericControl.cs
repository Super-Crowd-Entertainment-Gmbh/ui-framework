using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Rehawk.UIFramework
{
    public class GenericControl : Control<object>
    {
        [PropertyOrder(-101)]
        [BoxGroup("ContextBox", false)]
        [LabelText("Context Type")]
        [HideInPlayMode]
        [HideIf("@this.UseStaticContext")]
        [OdinSerialize] private Type type;

        public override Type ContextType
        {
            get
            {
                if (UseStaticContext)
                {
                    if (StaticContext != null)
                    {
                        return StaticContext.GetType();
                    }
                    
                    return base.ContextType;
                }
                
                return type;
            }
        }
    }
}
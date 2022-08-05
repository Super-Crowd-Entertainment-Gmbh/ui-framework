using System;

namespace Rehawk.UIFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ControlAttribute : Attribute
    {
        public bool disallowContext;
        public Type[] contextTypes;

        public ControlAttribute(params Type[] contextTypes)
        {
            this.contextTypes = contextTypes;
        }
    }
}
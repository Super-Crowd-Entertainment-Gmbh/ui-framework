using System;

namespace Rehawk.UIFramework
{
    [Serializable]
    public abstract class FormatterBase
    {
        public abstract object ApplyFormat(object value);
    }
}
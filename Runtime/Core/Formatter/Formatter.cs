using System;

namespace Rehawk.UIFramework
{
    [Serializable]
    public abstract class Formatter
    {
        public abstract object ApplyFormat(object value);
    }
}
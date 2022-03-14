using System;
using UnityEngine;

namespace Rehawk.UIFramework
{
    [Serializable]
    public class DateTimeToStringFormatter : Formatter
    {
        [SerializeField] private string format;
        
        public override object ApplyFormat(object value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString(format);
            }

            return value;
        }
    }
}
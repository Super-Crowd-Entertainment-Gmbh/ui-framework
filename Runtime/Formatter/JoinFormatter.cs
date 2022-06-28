using System;
using System.Collections;
using UnityEngine;

namespace Rehawk.UIFramework
{
    [Serializable]
    public class JoinFormatter : FormatterBase
    {
        [SerializeField] private string separator;
        
        public override object ApplyFormat(object value)
        {
            if (value is IList list)
            {
                string result = string.Empty;
                
                for (int i = 0; i < list.Count; i++)
                {
                    result += list[i].ToString();

                    if (i < list.Count - 1)
                    {
                        result += separator;
                    }
                }

                return result;
            }

            return value;
        }
    }
}
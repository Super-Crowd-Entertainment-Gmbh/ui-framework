using System;

namespace Rehawk.UIFramework.Utilities
{
    public static class TypeExtensions
    {
        public static string GetFriendlyName(this Type type)
        {
            string name = type.Name;

            if (type == typeof(UnityEngine.Object)) 
            {
                return "UnityObject";
            }

            if (name == "Single")
            {
                return "Float";
            }

            if (name == "Single[]")
            {
                return "Float[]";
            }

            if (name == "Int32")
            {
                return "Int";
            }

            if (name == "Int32[]")
            {
                return "Int[]";
            }
            
            return name;
        }
    }
}
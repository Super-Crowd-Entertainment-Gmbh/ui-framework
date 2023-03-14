using System;
using System.Linq.Expressions;

namespace Rehawk.UIFramework
{
    public static class MemberPath
    {
        public static string Get<T>(Expression<Func<T>> memberExpression)
        {
            string path = string.Empty;
            
            var p = memberExpression.Body as MemberExpression;
            
            while (p != null)
            {             
                path = p.Member.Name + (path != string.Empty ? "." : "") + path;
                p = p.Expression as MemberExpression;
            }

            return path;           
        }
    }
}
namespace Rehawk.UIFramework.Utilities
{
    public static class ObjectUtility
    {
        public static bool IsNull(object instance)
        {
            return instance == null || instance.Equals(null);
        }
        
        public static bool IsEqual(object a, object b)
        {
            return a == b || Equals(a, b);
        }
    }
}
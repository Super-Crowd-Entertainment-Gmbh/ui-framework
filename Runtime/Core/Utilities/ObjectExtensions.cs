namespace Rehawk.UIFramework.Utilities
{
    public static class ObjectExtensions
    {
        public static bool IsNull(object instance)
        {
            return instance == null || instance.Equals(null);
        }
    }
}
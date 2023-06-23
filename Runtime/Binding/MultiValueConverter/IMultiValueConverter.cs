namespace Rehawk.UIFramework
{
    public interface IMultiValueConverter
    {
        object Convert(object[] values);
        object[] ConvertBack(object value);
    }
}
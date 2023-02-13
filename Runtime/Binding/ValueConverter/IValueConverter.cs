namespace Rehawk.UIFramework
{
    public interface IValueConverter
    {
        object Convert(object input);
        object ConvertBack(object input);
    }
}
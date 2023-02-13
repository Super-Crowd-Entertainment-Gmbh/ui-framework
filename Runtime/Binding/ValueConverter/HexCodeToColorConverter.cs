using UnityEngine;

namespace Rehawk.UIFramework
{
    public class HexCodeToColorConverter : IValueConverter
    {
        public object Convert(object input)
        {
            if (input != null && ColorUtility.TryParseHtmlString(input.ToString(), out Color color))
            {
                return color;
            }

            return Color.white;
        }

        public object ConvertBack(object input)
        {
            if (input is Color color)
            {
                return ColorUtility.ToHtmlStringRGBA(color);
            }
            
            return ColorUtility.ToHtmlStringRGBA(Color.white);
        }
    }
}
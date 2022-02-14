using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class TextBindingAdapter : BindingAdapter
    {
        [SerializeField] private TextMeshProUGUI label;
        
        protected override void OnGotContext()
        {
            label.text = string.Empty;
            
            if (Binding.Contains("{"))
            {
                string result = ProcessFormat(Binding);
                label.text = result;
            }
            else
            {
                label.text = GetValue<object>(Binding).ToString();
            }
        }

        protected override void OnLostContext()
        {
            label.text = string.Empty;
        }
        
        private string ProcessFormat(string format)
        {
            string result = format;
            
            Match match = Regex.Match(result, "{(.[^}]*)}");

            if (match.Success)
            {
                result = ProcessFormatRecursive(result, match);
            }
            else
            {
                result = string.Empty;
            }

            return result;
        }
        
        private string ProcessFormatRecursive(string format, Match match)
        {
            string result = format;

            string dataKey = match.Groups[1].ToString();

            object value = GetValue<object>(dataKey);
            string replacement = value != null ? value.ToString() : "";
                    
            result = result.Replace(match.Value, replacement);
            match = match.NextMatch();

            if (match.Success)
            {
                result = ProcessFormatRecursive(result, match);
            }
            
            return result;
        }
    }
}
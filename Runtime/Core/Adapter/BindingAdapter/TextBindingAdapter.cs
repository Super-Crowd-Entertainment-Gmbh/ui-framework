using System.Text.RegularExpressions;
using RSG;
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
                ProcessFormat(Binding)
                    .Then(result =>
                    {
                        label.text = result;
                    })
                    .Catch(Debug.LogError);
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
        
        private IPromise<string> ProcessFormat(string format)
        {
            var promise = new Promise<string>();
            
            Match match = Regex.Match(format, "{(.[^}]*)}");

            if (match.Success)
            {
                ProcessFormatRecursive(format, match)
                    .Then(promise.Resolve)
                    .Catch(promise.Reject);
            }
            else
            {
                promise.Resolve(string.Empty);
            }

            return promise;
        }
        
        private IPromise<string> ProcessFormatRecursive(string format, Match match)
        {
            var promise = new Promise<string>();
            
            string dataKey = match.Groups[1].ToString();

            object value = GetValue<object>(dataKey);
            string replacement = value != null ? value.ToString() : "";
                    
            format = format.Replace(match.Value, replacement);
            match = match.NextMatch();

            if (match.Success)
            {
                ProcessFormatRecursive(format, match)
                    .Then(promise.Resolve)
                    .Catch(promise.Reject);
            }
            else
            {
                promise.Resolve(format);
            }
            
            return promise;
        }
    }
}
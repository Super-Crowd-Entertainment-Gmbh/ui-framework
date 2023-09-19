using System;

namespace Rehawk.UIFramework
{
    public static class BindingConverterExtensions
    {
        public static Binding ConvertByFunction(this Binding binding, ValueConvertFunctionDelegate converterFunction)
        {
            var converter = new FunctionConverter(converterFunction);
            
            MultiConverterHelper.AddConverter(binding, converter);

            return binding;
        }

        public static Binding ConvertByFunction<T>(this Binding binding, ValueConvertFunctionDelegate<T> converterFunction) 
        {
            var converter = new FunctionConverter(value =>
            {
                if (value != null)
                {
                    return converterFunction.Invoke((T) value);
                }
                else
                {
                    return converterFunction.Invoke(default);
                }
            });
            
            MultiConverterHelper.AddConverter(binding, converter);

            return binding;
        }

        public static Binding ConvertTo<T>(this Binding binding)
        {
            return binding.ConvertByFunction(input =>
            {
                if (input != null)
                {
                    return (T) input;
                }

                return default(T);
            });
        }

        public static Binding ConvertToBool(this Binding binding)
        {
            return binding.ConvertByFunction(input =>
            {
                if (input is bool boolValue)
                {
                    return boolValue;
                }

                if (input is int intValue)
                {
                    return intValue > 0;
                }
                
                if (input is float floatValue)
                {
                    return floatValue > 0;
                }
                
                return input != null;
            });
        }

        public static Binding ConvertToInt(this Binding binding)
        {
            return binding.ConvertByFunction(input =>
            {
                string inputStr = input?.ToString();
                
                if (int.TryParse(inputStr, out int result))
                {
                    return result;
                }

                return 0;
            });
        }

        public static Binding ConvertToFloat(this Binding binding)
        {
            return binding.ConvertByFunction(input =>
            {
                string inputStr = input?.ToString();
                
                if (float.TryParse(inputStr, out float result))
                {
                    return result;
                }

                return 0f;
            });
        }

        public static Binding ConvertToDouble(this Binding binding)
        {
            return binding.ConvertByFunction(input =>
            {
                string inputStr = input?.ToString();
                
                if (double.TryParse(inputStr, out double result))
                {
                    return result;
                }

                return 0f;
            });
        }

        public static Binding ConvertToString(this Binding binding)
        {
            return binding.ConvertByFunction(input =>
            {
                return input?.ToString();
            });
        }

        public static Binding ConvertToString(this Binding binding, string format)
        {
            return binding.ConvertByFunction(input =>
            {
                return string.Format(format, input);
            });
        }

        public static Binding ConvertToDateTimeString(this Binding binding, string format)
        {
            return binding.ConvertByFunction(input =>
            {
                if (input is DateTime dateTime)
                {
                    return dateTime.ToString(format);
                }
                
                return input;
            });
        }

        public static Binding ConvertToInvertedBool(this Binding binding)
        {
            return binding.ConvertByFunction(input =>
            {
                if (input != null)
                {
                    return !((bool)input);
                }
                
                // Null is processed like false
                
                return true;
            });
        }
        
        public static Binding ConvertBy(this Binding binding, IValueConverter converter) 
        {
            MultiConverterHelper.AddConverter(binding, converter);
            
            return binding;
        }
    }
}
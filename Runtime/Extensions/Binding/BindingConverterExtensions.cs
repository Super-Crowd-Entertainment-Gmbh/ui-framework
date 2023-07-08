using System;

namespace Rehawk.UIFramework
{
    public static class BindingConverterExtensions
    {
        public static Binding ConvertByCustom(this Binding binding, IValueConverter converter) 
        {
            MultiConverterHelper.AddConverter(binding, converter);
            
            return binding;
        }

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
            binding.ConvertByFunction(input =>
            {
                if (input != null)
                {
                    return (T) input;
                }

                return default(T);
            });
            
            return binding;
        }

        public static Binding ConvertToBool(this Binding binding)
        {
            binding.ConvertTo<bool>();
            
            return binding;
        }

        public static Binding ConvertToInt(this Binding binding)
        {
            binding.ConvertByFunction(input =>
            {
                string inputStr = input?.ToString();
                
                if (int.TryParse(inputStr, out int result))
                {
                    return result;
                }

                return 0;
            });
            
            return binding;
        }

        public static Binding ConvertToFloat(this Binding binding)
        {
            binding.ConvertByFunction(input =>
            {
                string inputStr = input?.ToString();
                
                if (float.TryParse(inputStr, out float result))
                {
                    return result;
                }

                return 0f;
            });
            
            return binding;
        }

        public static Binding ConvertToDouble(this Binding binding)
        {
            binding.ConvertByFunction(input =>
            {
                string inputStr = input?.ToString();
                
                if (double.TryParse(inputStr, out double result))
                {
                    return result;
                }

                return 0f;
            });
            
            return binding;
        }

        public static Binding ConvertToString(this Binding binding)
        {
            binding.ConvertByFunction(input =>
            {
                return input?.ToString();
            });
            
            return binding;
        }

        public static Binding ConvertToString(this Binding binding, string format)
        {
            binding.ConvertByFunction(input =>
            {
                return string.Format(format, input);
            });
            
            return binding;
        }

        public static Binding ConvertToDateTimeString(this Binding binding, string format)
        {
            binding.ConvertByFunction(input =>
            {
                if (input is DateTime dateTime)
                {
                    return dateTime.ToString(format);
                }
                
                return input;
            });
            
            return binding;
        }

        public static Binding ConvertToInvertedBool(this Binding binding)
        {
            binding.ConvertByFunction(input =>
            {
                if (input != null)
                {
                    return !((bool)input);
                }
                
                // Null is processed like false
                
                return true;
            });
            
            return binding;
        }
    }
}
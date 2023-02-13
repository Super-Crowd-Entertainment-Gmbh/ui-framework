namespace Rehawk.UIFramework
{
    public delegate object ValueConvertFunctionDelegate(object input);
    
    public class FunctionConverter : IValueConverter
    {
        private readonly ValueConvertFunctionDelegate convertFunction;
        private readonly ValueConvertFunctionDelegate convertBackFunction;
        
        public FunctionConverter(ValueConvertFunctionDelegate convertFunction)
        {
            this.convertFunction = convertFunction;
        }
        
        public object Convert(object input)
        {
            return convertFunction.Invoke(input);
        }

        public object ConvertBack(object input)
        {
            return convertBackFunction.Invoke(input);
        }
    }
}
namespace Rehawk.UIFramework
{
    public static class BindingCombinerExtensions
    {
        public static Binding CombineByCustom(this Binding binding, IValueCombiner valueCombiner)
        {
            MultiBindingStrategy multiBindingStrategy = MultiBindingHelper.ReplaceSourceWithMultiBindingStrategy(binding);
            
            multiBindingStrategy.SetCombiner(valueCombiner);

            return binding;
        }

        public static Binding CombineByFunction(this Binding binding, MultiValueConvertFunctionDelegate valueConverter)
        {
            return binding.CombineByCustom(new FunctionCombiner(valueConverter));
        }

        public static Binding CombineByFormat(this Binding binding, string format)
        {
            return binding.CombineByCustom(new StringFormatCombiner(format));
        }
    }
}
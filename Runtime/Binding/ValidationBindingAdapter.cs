using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework
{
    public class ValidationBindingAdapter : SingleBindingAdapter
    {
        [LabelText("Operator")]
        [SerializeField] private Operator @operator;
        [OdinSerialize] private object comparedTo;
        
        [SerializeField] private UnityEvent onValid;
        [SerializeField] private UnityEvent onInvalid;

        private bool wasSetBefore = false;
        private bool wasValid;
        
        protected override void OnRefresh()
        {
            base.OnRefresh();

            object value = GetValue<object>(Binding);

            bool isValid = false;
            
            switch (@operator)
            {
                case Operator.Equals:
                    isValid = Equals(value, comparedTo);
                    break;
                case Operator.NotEquals:
                    isValid = !Equals(value, comparedTo);
                    break;
            }

            if (!isValid && value is int intValue && comparedTo is int intComparedTo)
            {
                switch (@operator)
                {
                    case Operator.Less:
                        isValid = intValue < intComparedTo;
                        break;
                    case Operator.LessOrEqual:
                        isValid = intValue <= intComparedTo;
                        break;
                    case Operator.Greater:
                        isValid = intValue > intComparedTo;
                        break;
                    case Operator.GreaterOrEqual:
                        isValid = intValue >= intComparedTo;
                        break;
                }
            }

            if (!isValid && value is float floatValue && comparedTo is float floatComparedTo)
            {
                switch (@operator)
                {
                    case Operator.Less:
                        isValid = floatValue < floatComparedTo;
                        break;
                    case Operator.LessOrEqual:
                        isValid = floatValue <= floatComparedTo;
                        break;
                    case Operator.Greater:
                        isValid = floatValue > floatComparedTo;
                        break;
                    case Operator.GreaterOrEqual:
                        isValid = floatValue >= floatComparedTo;
                        break;
                }
            }

            if (!isValid && value is double doubleValue && comparedTo is double doubleComparedTo)
            {
                switch (@operator)
                {
                    case Operator.Less:
                        isValid = doubleValue < doubleComparedTo;
                        break;
                    case Operator.LessOrEqual:
                        isValid = doubleValue <= doubleComparedTo;
                        break;
                    case Operator.Greater:
                        isValid = doubleValue > doubleComparedTo;
                        break;
                    case Operator.GreaterOrEqual:
                        isValid = doubleValue >= doubleComparedTo;
                        break;
                }
            }

            if (!wasSetBefore || wasValid != isValid)
            {
                if (isValid)
                {
                    onValid.Invoke();
                }
                else
                {
                    onInvalid.Invoke();
                }
            }

            wasValid = isValid;
            wasSetBefore = true;
        }

        private enum Operator
        {
            [InspectorName("==")]
            Equals,
            [InspectorName("!=")]
            NotEquals,
            [InspectorName("<")]
            Less,
            [InspectorName("<=")]
            LessOrEqual,
            [InspectorName(">")]
            Greater,
            [InspectorName(">=")]
            GreaterOrEqual
        }
    }
}
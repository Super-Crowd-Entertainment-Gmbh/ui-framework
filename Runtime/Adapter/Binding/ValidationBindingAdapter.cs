using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework
{
    public class ValidationBindingAdapter : SingleBindingAdapterBase
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

            // Hack for unity types which are not recognised as null by normal == check. (eg. Sprite)
            if (value != null && value.Equals(null))
            {
                value = null;
            }
            
            switch (@operator)
            {
                case Operator.Equals:
                    if (value != null)
                    {
                        isValid = value.Equals(comparedTo);
                    }
                    else
                    {
                        isValid = Equals(value, comparedTo);
                    }
                    break;
                case Operator.NotEquals:
                    if (value != null)
                    {
                        isValid = !value.Equals(comparedTo);
                    }
                    else
                    {
                        isValid = !Equals(value, comparedTo);
                    }
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

            if (!isValid && value is uint uintValue && comparedTo is uint uintComparedTo)
            {
                switch (@operator)
                {
                    case Operator.Less:
                        isValid = uintValue < uintComparedTo;
                        break;
                    case Operator.LessOrEqual:
                        isValid = uintValue <= uintComparedTo;
                        break;
                    case Operator.Greater:
                        isValid = uintValue > uintComparedTo;
                        break;
                    case Operator.GreaterOrEqual:
                        isValid = uintValue >= uintComparedTo;
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

        [ContextMenu("Compare To Int")]
        public void CompareToInt()
        {
            comparedTo = 0;
        }
        
        [ContextMenu("Compare To UInt")]
        public void CompareToUInt()
        {
            comparedTo = (uint) 0;
        }
        
        [ContextMenu("Compare To Float")]
        public void CompareToFloat()
        {
            comparedTo = 0f;
        }
        
        [ContextMenu("Compare To Double")]
        public void CompareToDouble()
        {
            comparedTo = (double) 0;
        }
        
        [ContextMenu("Compare To Bool")]
        public void CompareToBool()
        {
            comparedTo = false;
        }
        
        [ContextMenu("Compare To String")]
        public void CompareToString()
        {
            comparedTo = "";
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

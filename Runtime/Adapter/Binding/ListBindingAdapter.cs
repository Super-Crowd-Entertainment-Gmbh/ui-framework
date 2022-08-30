using System;
using System.Collections;
using Rehawk.UIFramework.Utilities;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ListBindingAdapter : SingleBindingAdapterBase
    {
        [SerializeField] private ListControlBase listControl;
        [SerializeField] private Mode mode;

        protected override void OnRefresh()
        {
            base.OnRefresh();

            listControl.Clear();

            switch (mode)
            {
                case Mode.Items:
                    HandleItems();
                    break;
                case Mode.Count:
                    HandleCount();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
        }

        private void HandleItems()
        {
            var value = GetValue<IEnumerable>(Binding);
            if (!ObjectUtility.IsNull(value))
            {
                if (value is IDictionary dictionary)
                {
                    listControl.SetCountByData(dictionary.Values);
                }
                else
                {
                    listControl.SetCountByData(value);
                }
            }
        }
        
        private void HandleCount()
        {
            var value = GetValue<int>(Binding);
            if (!ObjectUtility.IsNull(value) && value > 0)
            {
                listControl.SetCount(value);
            }
        }
        
        public enum Mode
        {
            Items,
            Count
        }
    }
}
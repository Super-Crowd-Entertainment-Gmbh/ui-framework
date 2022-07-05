using System.Collections;
using Rehawk.UIFramework.Utilities;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ListBindingAdapter : SingleBindingAdapterBase
    {
        [SerializeField] private ListControlBase listControl;

        protected override void OnRefresh()
        {
            base.OnRefresh();

            listControl.Clear();
            
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
    }
}
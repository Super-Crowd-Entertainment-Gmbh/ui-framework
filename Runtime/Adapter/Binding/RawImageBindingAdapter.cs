using Rehawk.UIFramework.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    public class RawImageBindingAdapter : SingleBindingAdapterBase
    {
        [SerializeField] private RawImage image;

        protected override void OnRefresh()
        {
            base.OnRefresh();
            
            image.texture = null;

            var value = GetValue<Texture>(Binding);
            if (!ObjectUtility.IsNull(value))
            {
                image.texture = value;
            }
        }
    }
}
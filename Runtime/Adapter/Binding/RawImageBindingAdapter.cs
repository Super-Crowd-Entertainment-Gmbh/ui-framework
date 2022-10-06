using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    public class RawImageBindingAdapter : SingleBindingAdapterBase
    {
        [SerializeField] 
        private RawImage[] images;
        
        [SerializeField, HideInInspector] 
        private RawImage image;

        protected override void OnRefresh()
        {
            base.OnRefresh();
            
            var value = GetValue<Texture>(Binding);
            
            for (int i = 0; i < images.Length; i++)
            {
                images[i].texture = value;
            }  
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (images == null || images.Length <= 0)
            {
                images = new[]
                {
                    image
                };
            }
        }
#endif
    }
}
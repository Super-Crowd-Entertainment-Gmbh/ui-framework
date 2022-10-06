using System;
using Rehawk.UIFramework.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    public class ImageBindingAdapter : SingleBindingAdapterBase
    {
        [SerializeField] 
        private Image[] images;
        [SerializeField] 
        private Mode mode;

        [SerializeField, HideInInspector] 
        private Image image;

        private Color[] defaultImageColor;

        protected override void Awake()
        {
            base.Awake();

            defaultImageColor = new Color[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                defaultImageColor[i] = images[i].color;
            }
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();
            
            switch (mode)
            {
                case Mode.Sprite:
                case Mode.OverrideSprite:
                    HandleSprite();
                    break;
                case Mode.Color:
                    HandleColor();
                    break;
                case Mode.FillAmount:
                    HandleFillAmount();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleSprite()
        {
            SetSprite(null);
            
            var value = GetValue<Sprite>(Binding);
            if (!ObjectUtility.IsNull(value))
            {
                SetSprite(value);
            }
        }

        private void HandleColor()
        {
            var value = GetValue<Color>(Binding);

            if (ObjectUtility.IsNull(value))
            {
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].color = defaultImageColor[i];
                }  
            }
            else
            {  
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].color = value;
                }  
            }
        }

        private void HandleFillAmount()
        {
            var value = GetValue<float>(Binding);
            
            if (ObjectUtility.IsNull(value))
            {
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].fillAmount = 0;
                }  
            }
            else
            {  
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].fillAmount = value;
                }  
            }
        }

        private void SetSprite(Sprite sprite)
        {
            switch (mode)
            {
                case Mode.Sprite:
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].sprite = sprite;
                    }  
                    break;
                case Mode.OverrideSprite:
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].overrideSprite = sprite;
                    } 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
        
        private enum Mode
        {
            Sprite,
            OverrideSprite,
            Color,
            FillAmount
        }
    }
}
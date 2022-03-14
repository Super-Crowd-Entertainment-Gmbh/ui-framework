using System;
using Rehawk.UIFramework.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework
{
    public class ImageBindingAdapter : SingleBindingAdapter
    {
        [SerializeField] private Image image;
        [SerializeField] private Mode mode;

        private Color defaultImageColor;

        protected override void Awake()
        {
            base.Awake();

            defaultImageColor = image.color;
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
            image.color = defaultImageColor;
            
            var value = GetValue<Color>(Binding);
            if (!ObjectUtility.IsNull(value))
            {
                image.color = value;
            }
        }

        private void SetSprite(Sprite sprite)
        {
            switch (mode)
            {
                case Mode.Sprite:
                    image.sprite = sprite;
                    break;
                case Mode.OverrideSprite:
                    image.overrideSprite = sprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum Mode
        {
            Sprite,
            OverrideSprite,
            Color
        }
    }
}
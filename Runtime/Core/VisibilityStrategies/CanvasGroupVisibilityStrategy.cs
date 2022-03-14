using System;
using UnityEngine;

namespace Rehawk.UIFramework
{
    [Serializable]
    public class CanvasGroupVisibilityStrategy : VisibilityStrategy
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool toggleInteractable = true;
        [SerializeField] private bool toggleBlocksRaycasts = true;

        public override bool IsVisible
        {
            get { return canvasGroup.alpha > 0; }
        }

        public override void SetVisible(bool visible, Action callback)
        {
            canvasGroup.alpha = visible ? 1 : 0;

            if (toggleInteractable)
            {
                canvasGroup.interactable = visible;
            }
            
            if (toggleBlocksRaycasts)
            {
                canvasGroup.blocksRaycasts = visible;
            }
            
            callback?.Invoke();
        }
    }
}
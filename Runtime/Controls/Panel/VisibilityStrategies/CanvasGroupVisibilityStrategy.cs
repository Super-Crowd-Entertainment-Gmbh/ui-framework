using System;
using UnityEngine;

namespace Rehawk.UIFramework
{
    [Serializable]
    public class CanvasGroupVisibilityStrategy : VisibilityStrategyBase
    {
        [SerializeField] 
        private CanvasGroup group;
        [SerializeField] 
        private float visibleAlpha = 1;
        [SerializeField] 
        private float inVisibleAlpha = 0;

        public override bool IsVisible
        {
            get { return group.alpha != inVisibleAlpha; }
        }

        public override void SetVisible(bool visible, Action callback)
        {
            group.alpha = visible ? visibleAlpha : inVisibleAlpha;
            callback?.Invoke();
        }
    }
}
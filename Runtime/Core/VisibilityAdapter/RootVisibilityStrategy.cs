using System;
using UnityEngine;

namespace Rehawk.UIFramework
{
    [Serializable]
    public class RootVisibilityStrategy : VisibilityStrategy
    {
        [SerializeField] private GameObject root;

        public override bool IsVisible
        {
            get { return root.activeSelf; }
        }

        public override void SetVisible(bool visible, Action callback)
        {
            root.SetActive(visible);
            callback?.Invoke();
        }
    }
}
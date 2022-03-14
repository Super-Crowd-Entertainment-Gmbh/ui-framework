using System;

namespace Rehawk.UIFramework
{
    [Serializable]
    public abstract class VisibilityStrategy
    {
        public abstract bool IsVisible { get; }
        public abstract void SetVisible(bool visible, Action callback);
    }
}
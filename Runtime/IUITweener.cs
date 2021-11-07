using System;

namespace Rehawk.UIFramework
{
    public interface IUITweener
    {
        bool IsVisible { get; }
        
        void Show(float duration = 1);
        void Hide(float duration = 1);
    }
}
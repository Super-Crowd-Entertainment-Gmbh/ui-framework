using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace Rehawk.UIFramework
{
    public interface IPanel : IElement
    {
        void AddSubElement(IElement element);
        void RemoveSubElement(IElement element);
    }
    
    public abstract class Panel : Element, IPanel
    {
        private readonly HashSet<IElement> elements = new HashSet<IElement>();

        public event EventHandler BecameVisible; 
        public event EventHandler BecameInvisible; 
            
        public abstract bool IsVisible { get; }

        public void Show()
        {
            Show(1);
        }
        
        public void Hide()
        {
            Hide(1);
        }
        
        public void Toggle()
        {
            Toggle(1);
        }
        
        public void Show(float duration)
        {
            OnShow(duration);

            foreach (IElement element in elements)
            {
                element.OnParentPanelGotVisible(this);
            }
            
            BecameVisible?.Invoke(this, EventArgs.Empty);
        }
        
        public void Hide(float duration)
        {
            OnHide(duration);
            
            foreach (IElement element in elements)
            {
                element.OnParentPanelGotInvisible(this);
            }
            
            BecameInvisible?.Invoke(this, EventArgs.Empty);
        }

        public void Toggle(float duration)
        {
            if (IsVisible)
            {
                Hide(duration);
            }
            else
            {
                Show(duration);
            }
        }
        
        public void AddSubElement(IElement element)
        {
            elements.Add(element);
        }
        
        public void RemoveSubElement(IElement element)
        {
            elements.Remove(element);
        }

        protected virtual void OnShow(float duration = 0) {}
        protected virtual void OnHide(float duration = 0) {}
    }
}
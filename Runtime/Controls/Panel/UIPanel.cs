using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework
{
    public class UIPanel : UIControlBase
    {
        [SerializeField] 
        private InitialVisibility visibility = InitialVisibility.None;

        [SerializeField] 
        private ParentConstraint parentConstraint = ParentConstraint.None;
        
        [Space]
        
        [SerializeReference, SubclassSelector] 
        private VisibilityStrategyBase visibilityStrategy;
        
        [Space]
        
        [SerializeField] 
        private UnityEvent becameVisible;
        [SerializeField]
        private UnityEvent becameInvisible;

        private bool isInitialized;
        
        private UIPanel parentUIPanel;

        public event Action<UIPanel> BecameVisible;
        public event Action<UIPanel> BecameInvisible;
        
        public bool IsVisible
        {
            get
            {
                if (visibilityStrategy != null)
                {
                    return visibilityStrategy.IsVisible;
                }
                
                return gameObject.activeSelf;
            }
        }

        public UIPanel Parent
        {
            get { return parentUIPanel; }
        }

        protected override void Awake()
        {
            base.Awake();

            parentUIPanel = GetComponentsInParent<UIPanel>()
                .FirstOrDefault(p => p != this);

            if (parentUIPanel)
            {
                parentUIPanel.BecameVisible += OnParentUIPanelBecameVisible;    
                parentUIPanel.BecameInvisible += OnParentUIPanelBecameInvisible;    
            }

            if (visibility != InitialVisibility.None)
            {
                SetVisible(true);
            }
            
            isInitialized = true;
            
            if (visibility != InitialVisibility.None)
            {
                StartCoroutine(SetInitialVisibilityDelayed());
            }
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (parentUIPanel)
            {
                parentUIPanel.BecameVisible -= OnParentUIPanelBecameVisible;    
                parentUIPanel.BecameInvisible -= OnParentUIPanelBecameInvisible;    
            }
        }

        public void SetVisible(bool visible)
        {
            if (IsVisible != visible)
            {
                if (visibilityStrategy != null)
                {
                    visibilityStrategy.SetVisible(visible, HandleVisibilityChange);
                }
                else
                {
                    gameObject.SetActive(visible);
                    HandleVisibilityChange();
                }
            }
        }

        [ContextMenu("Toggle")]
        public void ToggleVisible()
        {
            SetVisible(!IsVisible);
        }
        
        private void HandleVisibilityChange()
        {
            if (!isInitialized)
                return;

            if (IsVisible)
            {
                BecameVisible?.Invoke(this);
                becameVisible.Invoke();
            }
            else
            {
                BecameInvisible?.Invoke(this);
                becameInvisible.Invoke();
            }
        }

        private IEnumerator SetInitialVisibilityDelayed()
        {
            yield return null;
            
            // Do it one frame after Start to enable child controls Start too. 
            SetVisible(visibility == InitialVisibility.Visible);
        }
        
        private void OnParentUIPanelBecameVisible(UIPanel panel)
        {
            if (parentConstraint == ParentConstraint.ShowWith || parentConstraint == ParentConstraint.HideAndShowWith)
            {
                SetVisible(true);
            }
        }

        private void OnParentUIPanelBecameInvisible(UIPanel panel)
        {
            if (parentConstraint == ParentConstraint.HideWith || parentConstraint == ParentConstraint.HideAndShowWith)
            {
                SetVisible(false);
            }
        }

        public enum InitialVisibility
        {
            None,
            Visible,
            Hidden
        }

        public enum ParentConstraint
        {
            None,
            HideWith,
            ShowWith,
            HideAndShowWith,
        }
    }
}
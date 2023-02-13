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
        private bool hiddenByDefault;

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

        public event EventHandler<UIPanel> BecameVisible;
        public event EventHandler<UIPanel> BecameInvisible;
        
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

            SetVisible(true);

            isInitialized = true;
            
            StartCoroutine(SetInitialVisibilityDelayed());
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
                BecameVisible?.Invoke(this, this);
                becameVisible.Invoke();
            }
            else
            {
                BecameInvisible?.Invoke(this, this);
                becameInvisible.Invoke();
            }
        }

        private IEnumerator SetInitialVisibilityDelayed()
        {
            yield return null;
            
            // Do it after the start to enable the static context set of child controls.
            SetVisible(!hiddenByDefault);
        }
        
        private void OnParentUIPanelBecameVisible(object sender, UIPanel panel)
        {
            if (parentConstraint == ParentConstraint.ShowWith || parentConstraint == ParentConstraint.HideAndShowWith)
            {
                SetVisible(true);
            }
        }

        private void OnParentUIPanelBecameInvisible(object sender, UIPanel panel)
        {
            if (parentConstraint == ParentConstraint.HideWith || parentConstraint == ParentConstraint.HideAndShowWith)
            {
                SetVisible(false);
            }
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
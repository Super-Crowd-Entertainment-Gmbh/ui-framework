using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Rehawk.UIFramework
{
    public delegate void PanelVisibilityEvent(Panel panel);
    
    [ShowOdinSerializedPropertiesInInspector]
    public sealed class Panel : UIBehaviour, 
        ISerializationCallbackReceiver, 
        ISupportsPrefabSerialization
    {
        [BoxGroup("StrategyBox", false)]
        [DisableInPlayMode]
        [SerializeField] private bool hiddenByDefault;

        [BoxGroup("StrategyBox", false)]
        [DisableInPlayMode]
        [Tooltip("If TRUE, all other panels tagged with isMono are closed.")]
        [SerializeField] private bool isMono;
        
        [BoxGroup("StrategyBox", false)]
        [DisableInPlayMode]
        [SerializeField] private ParentConstraint parentConstraint = ParentConstraint.None;
        
        [Space]
        
        [BoxGroup("StrategyBox", false)]
        [LabelText("Strategy")]
        [SerializeField] private bool useStrategy;
        
        [BoxGroup("StrategyBox", false)]
        [HideLabel, ShowIf("@this.useStrategy")]
        [ShowInInspector] private VisibilityStrategyBase visibilityStrategy;
        
        [Space]
        
        [SerializeField] private UnityEvent becameVisible;
        [SerializeField] private UnityEvent becameInvisible;

        private bool isInitialized;
        
        private Panel parentPanel;

        public event PanelVisibilityEvent BecameVisible;
        public event PanelVisibilityEvent BecameInvisible;
        
        public bool IsVisible
        {
            get
            {
                if (useStrategy && visibilityStrategy != null)
                {
                    return visibilityStrategy.IsVisible;
                }
                
                return gameObject.activeSelf;
            }
        }

        public Panel ParentPanel
        {
            get { return parentPanel; }
        }

        protected override void Awake()
        {
            base.Awake();

            parentPanel = GetComponentsInParent<Panel>().FirstOrDefault(p => p != this);

            if (parentPanel)
            {
                parentPanel.BecameVisible += OnParentPanelBecameVisible;    
                parentPanel.BecameInvisible += OnParentPanelBecameInvisible;    
            }

            if (isMono)
            {
                MonoPanels.Register(this);
            }
            
            SetVisible(true);

            isInitialized = true;
            
            UIPoller.RunCoroutine(SetInitialVisibilityDelayed());
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (parentPanel)
            {
                parentPanel.BecameVisible -= OnParentPanelBecameVisible;    
                parentPanel.BecameInvisible -= OnParentPanelBecameInvisible;    
            }
            
            if (isMono)
            {
                MonoPanels.Unregister(this);
            }
        }

        public void SetVisible(bool visible)
        {
            if (IsVisible != visible)
            {
                if (useStrategy && visibilityStrategy != null)
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
            
            // Do it after the start to enable the static context set of child controls.
            SetVisible(!hiddenByDefault);
        }
        
        private void OnParentPanelBecameVisible(Panel panel)
        {
            if (parentConstraint == ParentConstraint.ShowWith || parentConstraint == ParentConstraint.HideAndShowWith)
            {
                SetVisible(true);
            }
        }

        private void OnParentPanelBecameInvisible(Panel panel)
        {
            if (parentConstraint == ParentConstraint.HideWith || parentConstraint == ParentConstraint.HideAndShowWith)
            {
                SetVisible(false);
            }
        }

        #region SERIALIZATION

        [SerializeField, HideInInspector] private SerializationData serializationData;
        
        [SerializeField, HideInInspector] private byte[] visibilityStrategyData;
        [SerializeField, HideInInspector] private List<Object> visibilityStrategyReferenceResolverData;

        SerializationData ISupportsPrefabSerialization.SerializationData
        {
            get { return this.serializationData; }
            set { this.serializationData = value; }
        }
        
#if UNITY_EDITOR
        [HideInTables]
        [OnInspectorGUI]
        [PropertyOrder(-2.147484E+09f)]
        private void InternalOnInspectorGUI()
        {
            EditorOnlyModeConfigUtility.InternalOnInspectorGUI(this);
        }
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            visibilityStrategyData = SerializationUtility.SerializeValue(visibilityStrategy, DataFormat.Binary, out visibilityStrategyReferenceResolverData);
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);    
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
            visibilityStrategy = SerializationUtility.DeserializeValue<VisibilityStrategyBase>(visibilityStrategyData, DataFormat.Binary, visibilityStrategyReferenceResolverData);
        }

        #endregion

        public enum ParentConstraint
        {
            None,
            HideWith,
            ShowWith,
            HideAndShowWith,
        }
    }
}
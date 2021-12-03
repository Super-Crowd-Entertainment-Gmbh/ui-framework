using System;
using System.Collections.Generic;
using Rehawk.UIFramework.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Rehawk.UIFramework
{
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class Control : UIBehaviour, 
        ISerializationCallbackReceiver, 
        ISupportsPrefabSerialization
    {
        private bool isStarted;
        private Panel parentPanel;

        public event EventHandler GotDirty;

        public Panel ParentPanel
        {
            get { return parentPanel; }
        }

        public virtual bool IsValid
        {
            get { return true; }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // If the control was already started and disabled at some point, set it dirty after enable.
            if (isStarted)
            {
                SetDirty();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            parentPanel = GetComponentInParent<Panel>();

            if (parentPanel)
            {
                parentPanel.BecameVisible += OnPanelBecameVisible;
                parentPanel.BecameInvisible += OnPanelBecameInvisible;
            }
        }

        protected override void Start()
        {
            base.Start();
            
            isStarted = true;
            SetDirty();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (parentPanel)
            {
                parentPanel.BecameVisible -= OnPanelBecameVisible;
                parentPanel.BecameInvisible -= OnPanelBecameInvisible;
            }
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            if (parentPanel)
            {
                parentPanel.BecameVisible -= OnPanelBecameVisible;
                parentPanel.BecameInvisible -= OnPanelBecameInvisible;
            }
            
            parentPanel = GetComponentInParent<Panel>();
            
            if (parentPanel)
            {
                parentPanel.BecameVisible += OnPanelBecameVisible;
                parentPanel.BecameInvisible += OnPanelBecameInvisible;
            }
        }

        [ContextMenu("Set Dirty")]
        public void SetDirty()
        {
            OnRefresh();
            GotDirty?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Is called when the displayed visuals should be refreshed.
        /// </summary>
        protected virtual void OnRefresh() {}

        protected virtual void OnPanelBecameVisible() {}
        protected virtual void OnPanelBecameInvisible() {}
        
        private void OnPanelBecameVisible(Panel panel)
        {
            SetDirty();
            OnPanelBecameVisible();
        }

        private void OnPanelBecameInvisible(Panel panel)
        {
            OnPanelBecameInvisible();
        }
        
        #region SERIALIZATION

        [SerializeField]
        [HideInInspector]
        private SerializationData serializationData;
        
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
            this.OnBeforeSerialize();
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
            this.OnAfterDeserialize();
        }

        protected virtual void OnBeforeSerialize() {}

        protected virtual void OnAfterDeserialize() {}

        #endregion
    }
    
    public abstract class ContextControl : Control
    {
        public abstract event EventHandler BeforeContextChanged;
        public abstract event EventHandler AfterContextChanged;
        
        public abstract bool HasContext { get; }
        
        public abstract void SetContext(object context);
        public abstract object GetContext();
    }

    public abstract class Control<TContext> : ContextControl
    {
        [PropertyOrder(-100)]
        [BoxGroup("ContextBox", false)]
        [HorizontalGroup("ContextBox/Context"), LabelText("Context")]
        [HideInPlayMode]
        [SerializeField] private bool useStaticContext;
        [PropertyOrder(-100)]
        [BoxGroup("ContextBox")]
        [HorizontalGroup("ContextBox/Context"), HideLabel, EnableIf("@this.useStaticContext")]
        [HideInPlayMode]
        [ShowInInspector] private TContext staticContext;

        public override event EventHandler BeforeContextChanged;
        public override event EventHandler AfterContextChanged;
        
        [PropertyOrder(-100)]
        [BoxGroup("ContextBox", false)]
        [HorizontalGroup("ContextBox/Context"), LabelText("Context")]
        [HideInEditorMode, ShowInInspector, ReadOnly]
        public TContext Context { get; private set; }

        public override bool HasContext
        {
            get { return !(Equals(Context, default(TContext)) || ObjectExtensions.IsNull(Context)); }
        }

        public override bool IsValid
        {
            get { return base.IsValid && HasContext; }
        }

        protected override void Start()
        {
            if (useStaticContext)
            {
                SetContextWithoutDirty(staticContext);
            }
            
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            ResetContext();
        }

        public void SetContext(TContext context)
        {
            SetContextWithoutDirty(context);
            SetDirty();
        }

        private void SetContextWithoutDirty(TContext context)
        {
            OnBeforeContextChanged();
            BeforeContextChanged?.Invoke(this, EventArgs.Empty);
            Context = context;
            OnAfterContextChanged();
            AfterContextChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ResetContext()
        {
            SetContext(default);
        }

        public override void SetContext(object context)
        {
            if (!(context is TContext castedContext))
            {
                Debug.LogError($"Given context '{context}' doesn't match the required type of '{typeof(TContext)}'", this);
                return;
            }
            
            SetContext(castedContext);
        }
        
        public override object GetContext()
        {
            return Context;
        }

        /// <summary>
        /// Is called right before the context is changed.
        /// Example: Use it to unsubscribe from events of the previous context.
        /// </summary>
        protected virtual void OnBeforeContextChanged() {}
        
        /// <summary>
        /// Is called right after the context has changed but before the view gets dirty.
        /// Example: Use it to subscribe to events of the new context.
        /// </summary>
        protected virtual void OnAfterContextChanged() {}
        
        #region SERIALIZATION
        
        [SerializeField, HideInInspector] private byte[] staticContextData;
        [SerializeField, HideInInspector] private List<Object> staticContextReferenceResolverData;
        
        protected override void OnBeforeSerialize()
        {
            staticContextData = SerializationUtility.SerializeValue(staticContext, DataFormat.Binary, out staticContextReferenceResolverData);
            
            base.OnBeforeSerialize();
        }

        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            
            staticContext = SerializationUtility.DeserializeValue<TContext>(staticContextData, DataFormat.Binary, staticContextReferenceResolverData);
        }

        #endregion
    }
}
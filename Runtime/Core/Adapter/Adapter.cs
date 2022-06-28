using System;
using System.Collections.Generic;
using Rehawk.UIFramework.Utilities;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Object = UnityEngine.Object;

namespace Rehawk.UIFramework
{
    public abstract class AdapterBase : UIBehaviour
    {
        public abstract Type ControlType { get; }
        
        public abstract ControlBase GetControl();
    }
    
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class ControlAdapter<TControl> : AdapterBase, 
        ISerializationCallbackReceiver, 
        ISupportsPrefabSerialization,
        IRefreshable
        where TControl : ControlBase
    {
        [LabelText("Control")]
        [DisableInPlayMode]
        [BoxGroup("ContextBox", false)]
        [PropertyOrder(-100)]
        [ShowInInspector] private TControl element;

        private bool isStarted;
        private bool willBeDestroyed;
        private bool gotDirtyBeforeStarted;
        
        public override Type ControlType
        {
            get { return typeof(TControl); }
        }

        public TControl Control
        {
            get { return element; }
        }

        public bool HasControl
        {
            get { return !(Equals(element, default(TControl)) || ObjectUtility.IsNull(element)); }
        }
        
        protected virtual bool UsePoller
        {
            get { return true; }
        }

        bool IRefreshable.IsDestroyed
        {
            get { return willBeDestroyed; }
        }

        protected override void Awake()
        {
            base.Awake();
            
            if (!HasControl)
            {
                element = GetComponent<TControl>();
            }

            Assert.IsTrue(element != null, $"Element of {name}:{GetType().Name} can't be null.");

            if (HasControl)
            {
                element.GotDirty += OnTargetGotDirty;
            }
        }

        protected override void Start()
        {
            base.Start();

            isStarted = true;

            if (gotDirtyBeforeStarted)
            {
                SetDirty();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            willBeDestroyed = true;
            
            if (HasControl)
            {
                element.GotDirty -= OnTargetGotDirty;
            }
        }

        public override ControlBase GetControl()
        {
            return Control;
        }

        [ContextMenu("Set Dirty")]
        public void SetDirty()
        {
            if (!isStarted)
            {
                gotDirtyBeforeStarted = true;
            }
            else if (UsePoller && !willBeDestroyed)
            {
                UIPoller.Tag(this);
            }
            else
            {
                OnRefresh();
            }
        }

        private void Refresh()
        {
            OnRefresh();
        }

        void IRefreshable.Refresh()
        {
            Refresh();
        }

        /// <summary>
        /// Is called when the displayed visuals should be refreshed.
        /// </summary>
        protected virtual void OnRefresh() {}

        private void OnTargetGotDirty(object sender, EventArgs e)
        {
            SetDirty();
        }
        
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            
            if (!HasControl)
            {
                element = GetComponent<TControl>();
            }
        }
#endif
        
        #region SERIALIZATION

        [SerializeField, HideInInspector] private SerializationData serializationData;
        
        [SerializeField, HideInInspector] private byte[] elementData;
        [SerializeField, HideInInspector] private List<Object> elementReferenceResolverData;

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
            
            elementData = SerializationUtility.SerializeValue(element, DataFormat.Binary, out elementReferenceResolverData);
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);    
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
            element = SerializationUtility.DeserializeValue<TControl>(elementData, DataFormat.Binary, elementReferenceResolverData);
            
            this.OnAfterDeserialize();
        }

        protected virtual void OnBeforeSerialize() {}

        protected virtual void OnAfterDeserialize() {}

        #endregion
    }

    public abstract class ControlAdapter<TControl, TContext> : ControlAdapter<TControl>
        where TControl : Control<TContext>
    {
        public bool HasContext
        {
            get { return HasControl && Control.HasContext; }
        }

        public TContext Context
        {
            get { return HasControl ? Control.Context : default; }
        }

        protected override void Awake()
        {
            base.Awake();
            
            if (HasControl)
            {
                Control.BeforeContextChanged += OnBeforeContextChanged;
                Control.AfterContextChanged += OnAfterContextChanged;
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (HasControl)
            {
                Control.BeforeContextChanged -= OnBeforeContextChanged;
                Control.AfterContextChanged -= OnAfterContextChanged;
            }
        }
        
        protected virtual void OnBeforeContextChanged() {}
        protected virtual void OnAfterContextChanged() {}

        private void OnBeforeContextChanged(object sender, EventArgs e)
        {
            OnBeforeContextChanged();
        }
        
        private void OnAfterContextChanged(object sender, EventArgs e)
        {
            OnAfterContextChanged();
        }
    }
    
    public abstract class ContextAdapter<TContext> : ControlAdapter<ContextControl>
    {
        public bool HasContext
        {
            get { return HasControl && Control.HasContext; }
        }
    
        public TContext Context
        {
            get { return HasControl ? (TContext) Control.GetContext() : default; }
        }
    
        protected override void Awake()
        {
            base.Awake();
            
            if (HasControl)
            {
                Control.BeforeContextChanged += OnBeforeContextChanged;
                Control.AfterContextChanged += OnAfterContextChanged;
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (HasControl)
            {
                Control.BeforeContextChanged -= OnBeforeContextChanged;
                Control.AfterContextChanged -= OnAfterContextChanged;
            }
        }
        
        protected virtual void OnBeforeContextChanged() {}
        protected virtual void OnAfterContextChanged() {}
    
        private void OnBeforeContextChanged(object sender, EventArgs e)
        {
            OnBeforeContextChanged();
        }
        
        private void OnAfterContextChanged(object sender, EventArgs e)
        {
            OnAfterContextChanged();
        }
    }
}
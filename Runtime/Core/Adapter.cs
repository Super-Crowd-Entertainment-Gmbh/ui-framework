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
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class Adapter<TControl> : UIBehaviour, 
        ISerializationCallbackReceiver, 
        ISupportsPrefabSerialization
        where TControl : Control
    {
        [LabelText("Control")]
        [DisableInPlayMode]
        [ShowInInspector] private TControl element;
        
        public TControl Control
        {
            get { return element; }
        }

        public bool HasControl
        {
            get { return !(Equals(element, default(TControl)) || ObjectExtensions.IsNull(element)); }
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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (HasControl)
            {
                element.GotDirty -= OnTargetGotDirty;
            }
        }

        [ContextMenu("Set Dirty")]
        public void SetDirty()
        {
            OnDirty();
            OnRefresh();
        }

        [ContextMenu("Refresh")]
        public void Refresh()
        {
            OnRefresh();
        }

        /// <summary>
        /// Is called when the linked control got dirty.
        /// </summary>
        protected virtual void OnDirty() {}
        
        /// <summary>
        /// Is called when the displayed visuals should be refreshed.
        /// </summary>
        protected virtual void OnRefresh() {}

        private void OnTargetGotDirty(object sender, EventArgs e)
        {
            SetDirty();
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (!HasControl)
            {
                element = GetComponent<TControl>();
            }
        }

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

    public abstract class Adapter<TControl, TContext> : Adapter<TControl>
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
}
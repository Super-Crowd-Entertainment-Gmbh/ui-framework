using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

#if ODIN_INSPECTOR

using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Rehawk.UIFramework
{
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class Adapter<TElement> : UIBehaviour, 
        ISerializationCallbackReceiver, 
        ISupportsPrefabSerialization
        where TElement : IElement
    {
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
        
#else

namespace Wayward.UI
{
    public abstract class Adapter<TElement> : UIBehaviour, 
        ISerializationCallbackReceiver, 
        where TElement : IElement
    {
        
#endif

        [OdinSerialize] private TElement element;

        public TElement Element
        {
            get { return element; }
        }

        protected override void Start()
        {
            base.Start();
            
            if (element == null)
            {
                element = GetComponentInParent<TElement>();
            }

            Assert.IsTrue(element != null, $"Element of {name}:{GetType().Name} can't be null.");
            
            element.GotDirty += OnTargetGotDirty;
            element.Refreshed += OnTargetRefreshed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            element.GotDirty -= OnTargetGotDirty;
            element.Refreshed -= OnTargetRefreshed;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (element == null)
            {
                element = GetComponentInParent<TElement>();
            }
        }

        protected override void Reset()
        {
            base.Reset();
            
            if (element == null)
            {
                element = GetComponentInParent<TElement>();
            }
        }
#endif
        
        /// <summary>
        /// Is called when the data behind the connected element has been changed.
        /// Example: Use it to get data from data sources and store them in fields of the adapter.
        /// </summary>
        protected virtual void OnGotDirty() {}

        /// <summary>
        /// Is called when the displayed visuals of the adapter should be updated.
        /// Example: Use it to set the text of a contained text field to data stored in <see cref="OnGotDirty"/>.
        /// </summary>
        protected virtual void OnRefresh() {}

        private void OnTargetGotDirty(object sender, EventArgs e)
        {
            OnGotDirty();
        }

        private void OnTargetRefreshed(object sender, EventArgs e)
        {
            OnRefresh();
        }
        
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
#if ODIN_INSPECTOR
            UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
#endif
            this.OnAfterDeserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.OnBeforeSerialize();
#if ODIN_INSPECTOR
            UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
#endif
        }

        protected virtual void OnAfterDeserialize() {}

        protected virtual void OnBeforeSerialize() {}

    }
}
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#if ODIN_INSPECTOR

using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Rehawk.UIFramework
{
    public interface IElement
    {
        event EventHandler GotDirty;
        event EventHandler Refreshed;
        
        void OnParentPanelGotVisible(IPanel panel);
        void OnParentPanelGotInvisible(IPanel panel);
    }
    
    public interface IElement<TContext> : IElement
    {
        public TContext Context { get; }
        bool HasContext { get; }

        void SetContext(TContext context);
    }
    
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class Element : UIBehaviour, 
        ISerializationCallbackReceiver, 
        ISupportsPrefabSerialization,
        IElement
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
    public abstract class Element<TContext> : UIBehaviour, 
        ISerializationCallbackReceiver, 
        IElement
    {
        
#endif

        public event EventHandler GotDirty;
        public event EventHandler Refreshed;
        
        /// <summary>
        /// The next panel in the hierarchy above of this element.
        /// </summary>
        public IPanel ParentPanel { get; private set; }

        // public bool GetDirtyWhenGetVisible { get; set; } = true;
        // public bool RefreshWhenGetVisible { get; set; } = true;
        
        protected override void Start()
        {
            base.Start();
            
            ResolveParentPanel();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ParentPanel?.RemoveSubElement(this);
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            ResolveParentPanel();
        }

        public void SetDirty()
        {
            OnGotDirty();
            GotDirty?.Invoke(this, EventArgs.Empty);
            Refresh();
        }

        public void Refresh()
        {
            OnRefresh();
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        private void ResolveParentPanel()
        {
            ParentPanel?.RemoveSubElement(this);

            if (this is IPanel thisAsPanel)
            {
                ParentPanel = GetComponentsInParent<IPanel>().FirstOrDefault(p => p != thisAsPanel);
            }
            else
            {
                ParentPanel = GetComponentInParent<IPanel>();
            }

            ParentPanel?.AddSubElement(this);  
        }

        void IElement.OnParentPanelGotVisible(IPanel panel)
        {
            OnGotVisible(panel);

            // if (GetDirtyWhenGetVisible)
            // {
            //     SetDirty();
            // }
            // else if (RefreshWhenGetVisible)
            // {
            //     Refresh();
            // }
        }

        void IElement.OnParentPanelGotInvisible(IPanel panel)
        {
            OnGotInvisible(panel);
        }
        
        /// <summary>
        /// Is called when the data behind the element has been changed.
        /// Example: Use it to get data from data sources and store them in fields of the element. 
        /// </summary>
        protected virtual void OnGotDirty() {}
        
        /// <summary>
        /// Is called when the displayed visuals of the element should be updated.
        /// Example: Use it to set the text of a contained text field to data stored in <see cref="OnGotDirty"/>.
        /// </summary>
        protected virtual void OnRefresh() {}
        
        /// <summary>
        /// Is called when the panel in the hierarchy above of this element got visible.
        /// </summary>
        protected virtual void OnGotVisible(IPanel panel) {}
        
        /// <summary>
        /// Is called when the panel in the hierarchy above of this element got invisible.
        /// </summary>
        protected virtual void OnGotInvisible(IPanel panel) {}
        
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Rehawk.UIFramework.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Rehawk.UIFramework
{
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class ControlBase : UIBehaviour, 
        ISerializationCallbackReceiver, 
        ISupportsPrefabSerialization,
        IRefreshable
    {
        private bool isStarted;
        private bool willBeDestroyed;

        private Panel parentPanel;

        private readonly Dictionary<string, Action<ControlBase>> commandHandlers = new Dictionary<string, Action<ControlBase>>();
        
        public event EventHandler GotDirty;

        public Panel ParentPanel
        {
            get { return parentPanel; }
        }

        protected virtual bool UsePoller
        {
            get { return UISettings.UsePoller; }
        }

        bool IRefreshable.IsDestroyed
        {
            get { return willBeDestroyed; }
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

            willBeDestroyed = true;
            
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
            if (UsePoller && !willBeDestroyed)
            {
                UIPoller.Tag(this);
            }
            else
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            OnRefresh();
            GotDirty?.Invoke(this, EventArgs.Empty);
        }

        void IRefreshable.Refresh()
        {
            Refresh();
        }

        public void SendCommand(string commandName)
        {
            if (commandHandlers.TryGetValue(commandName, out Action<ControlBase> handler))
            {
                handler.Invoke(this);
            }
            else
            {
                Debug.LogError($"Command '{commandName}': The control has no registered handlers for this command.");
            }
        }
        
        public void RegisterCommandHandler<T>(string commandName, Action<T> callback) where T : ControlBase
        {
            Assert.IsTrue(this is T, $"Command '{commandName}': The control type used in callback differs from the actual control type.");

            void CapsuledCallback(ControlBase control)
            {
                if (control is T castedControl)
                {
                    callback?.Invoke(castedControl);
                }
                else
                {
                    Debug.LogError($"Command '{commandName}': The control type used in callback differs from the actual control type.");
                }
            }

            if (commandHandlers.ContainsKey(commandName))
            {
                commandHandlers[commandName] += CapsuledCallback;
            }
            else
            {
                commandHandlers.Add(commandName, CapsuledCallback);
            }
        }

        public void RegisterCommandHandler(string commandName, Action callback)
        {
            void CapsuledCallback(ControlBase control)
            {
                callback?.Invoke();
            }

            if (commandHandlers.ContainsKey(commandName))
            {
                commandHandlers[commandName] += CapsuledCallback;
            }
            else
            {
                commandHandlers.Add(commandName, CapsuledCallback);
            }
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
    
    public abstract class ContextControlBase : ControlBase
    {
        public abstract event EventHandler BeforeContextChanged;
        public abstract event EventHandler AfterContextChanged;
        
        public abstract Type ContextType { get; }
        public abstract bool HasContext { get; }
        
        public abstract void SetContext(object context);
        public abstract void ResetContext();
        public abstract object GetContext();
        public abstract void CopyContext(ControlBase control);
    }

    public abstract class ContextControlBase<TContext> : ContextControlBase
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
        [HideInEditorMode, ShowInInspector, Sirenix.OdinInspector.ReadOnly]
        public TContext Context { get; private set; }
        
        public override Type ContextType
        {
            get { return typeof(TContext); }
        }

        public override bool HasContext
        {
            get { return !(Equals(Context, default(TContext)) || ObjectUtility.IsNull(Context)); }
        }

        protected internal bool UseStaticContext
        {
            get { return useStaticContext; }
        }

        protected internal TContext StaticContext
        {
            get { return staticContext; }
        }

        protected override void Awake()
        {
            base.Awake();
            
            if (useStaticContext)
            {
                SetContextWithoutDirty(staticContext);
            }
        }

        public void SetContext(TContext context)
        {
            SetContextWithoutDirty(context);
            SetDirty();
        }

        public override void CopyContext(ControlBase control)
        {
            if (control is ContextControlBase contextControl)
            {
                SetContext(contextControl.GetContext());
            }
            else
            {
                Debug.LogError($"Source control '{control.GetType().Name}' is not a context control like the target control '{GetType().Name}.", gameObject);
            }
        }

        private void SetContextWithoutDirty(TContext context)
        {
            OnBeforeContextChanged();
            BeforeContextChanged?.Invoke(this, EventArgs.Empty);
            
            if (Context is INotifyPropertyChanged previousNotifyPropertyChanged)
            {
                previousNotifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
            }

            Context = context;

            if (Context is INotifyPropertyChanged newNotifyPropertyChanged)
            {
                newNotifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }
            
            OnAfterContextChanged();
            AfterContextChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void ResetContext()
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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetDirty();
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
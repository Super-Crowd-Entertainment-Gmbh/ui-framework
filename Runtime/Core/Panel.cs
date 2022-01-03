<<<<<<< HEAD
﻿using System.Collections;
=======
﻿using System;
using System.Collections;
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
using System.Collections.Generic;
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
        [LabelText("Strategy")]
        [SerializeField] private bool useStrategy;
        
        [BoxGroup("StrategyBox", false)]
        [HideLabel, ShowIf("@this.useStrategy")]
        [ShowInInspector] private VisibilityStrategy visibilityStrategy;
        
        [Space]
        
        [SerializeField] public UnityEvent becameVisible;
        [SerializeField] public UnityEvent becameInvisible;

        private bool isStarted;

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

        protected override void Awake()
        {
            base.Awake();

            gameObject.SetActive(true);
            SetVisible(true);
        }

        protected override void Start()
        {
            base.Start();

            isStarted = true;
            
            StartCoroutine(SetInitialVisibilityDelayed());
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
        
        private void HandleVisibilityChange()
        {
            if (isStarted)
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
            visibilityStrategy = SerializationUtility.DeserializeValue<VisibilityStrategy>(visibilityStrategyData, DataFormat.Binary, visibilityStrategyReferenceResolverData);
        }

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: A proper implementation of INotifyCollectionChanged would be nice. So that not every time the list got dirty every item is "recreated".

namespace Rehawk.UIFramework
{
    public delegate void UIListItemCallbackDelegate(int index, GameObject item, object data);

    public class UIList : UIContextControlBase
    {
        private IUIListItemStrategy itemStrategy;
        private Type itemReceiverType;

        private readonly List<object> datasets = new List<object>();
        
        private UIListItemCallbackDelegate onInitialized;
        private UIListItemCallbackDelegate onActivated;
        private UIListItemCallbackDelegate onDeactivated;

        public IReadOnlyList<GameObject> Items
        {
            get { return itemStrategy.Items; }    
        }

        protected override void BeforeContextChanged()
        {
            base.BeforeContextChanged();

            ClearItems();
        }

        protected override void AfterContextChanged()
        {
            base.AfterContextChanged();

            RefreshItems();
        }

        public void SetItemStrategy(IUIListItemStrategy itemStrategy)
        {
            this.itemStrategy = itemStrategy;
        }

        public void SetItemReceiverType<T>() where T : IUIListItemReceiver
        {
            itemReceiverType = typeof(T);
        }

        public void SetItemCallback(UIListItemCallback type, UIListItemCallbackDelegate callback)
        {
            switch (type)
            {
                case UIListItemCallback.Initialized:
                    if (onInitialized != null)
                    {
                        Debug.LogError($"The callback was already set before. [callback={type}]", gameObject);
                    }
                    onInitialized = callback;
                    break;
                case UIListItemCallback.Activated:
                    if (onActivated != null)
                    {
                        Debug.LogError($"The callback was already set before. [callback={type}]", gameObject);
                    }
                    onActivated = callback;
                    break;
                case UIListItemCallback.Deactivated:
                    if (onDeactivated != null)
                    {
                        Debug.LogError($"The callback was already set before. [callback={type}]", gameObject);
                    }
                    onDeactivated = callback;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        private void InvokeCallback(UIListItemCallback type, int index, GameObject item, object data)
        {
            switch (type) 
            {
                case UIListItemCallback.Initialized:
                    onInitialized?.Invoke(index, item, data);
                    break;
                case UIListItemCallback.Activated:
                    onActivated?.Invoke(index, item, data);
                    break;
                case UIListItemCallback.Deactivated:
                    onDeactivated?.Invoke(index, item, data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void ClearItems()
        {
            int index = 0;

            foreach (GameObject item in itemStrategy.Items)
            {
                if (item != null)
                {
                    if (datasets.Count > index)
                    {
                        InvokeCallback(UIListItemCallback.Deactivated, index, item, datasets[index]);
                    }
                    else
                    {
                        InvokeCallback(UIListItemCallback.Deactivated, index, item, null);
                    }
                }

                index++;
            }

            itemStrategy.Clear();
        }
        
        private void RefreshItems()
        {
            if (RawContext is IEnumerable enumerable)
            {
                datasets.Clear();
                
                int index = 0;
                foreach (object data in enumerable)
                {
                    datasets.Insert(index, data);
                    
                    ItemAddReport addReport = itemStrategy.AddItem(index, data);

                    GameObject item = addReport.Object;
                    bool isNew = addReport.IsNew;

                    if (item != null)
                    {
                        Type itemReceiverType = this.itemReceiverType;

                        if (itemReceiverType == null)
                        {
                            itemReceiverType = typeof(IUIListItemReceiver);
                        }
                        
                        if (item.TryGetComponent(itemReceiverType, out Component itemReceiverComponent) && itemReceiverComponent is IUIListItemReceiver itemReceiver)
                        {
                            itemReceiver.SetListItem(new ListItem(index, data));
                        }
                        
                        if (isNew)
                        {
                            InvokeCallback(UIListItemCallback.Initialized, index, item, data);
                        }
                
                        InvokeCallback(UIListItemCallback.Activated, index, item, data);
                    }

                    index++;
                }
            }
        }
    }

    public enum UIListItemCallback
    {
        Initialized,
        Activated,
        Deactivated,
    }
}
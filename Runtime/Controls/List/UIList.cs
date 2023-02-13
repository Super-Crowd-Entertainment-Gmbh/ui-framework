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
        private Type itemReceiver;

        private object[] datasets;
        
        private UIListItemCallbackDelegate onInitialized;
        private UIListItemCallbackDelegate onActivated;
        private UIListItemCallbackDelegate onDeactivated;

        public IReadOnlyList<GameObject> Items
        {
            get { return itemStrategy.Items; }    
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

        public void SetItemReceiver<T>() where T : IUIListItemReceiver
        {
            itemReceiver = typeof(T);
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

        private void RefreshItems()
        {
            if (datasets != null)
            {
                for (int i = 0; i < datasets.Length; i++)
                {
                    GameObject item = itemStrategy.GetItem(i);

                    if (item != null)
                    {
                        itemStrategy.RemoveItem(i);
                
                        InvokeCallback(UIListItemCallback.Deactivated, i, item, datasets[i]);
                    }
                }
            }
            
            if (RawContext is IEnumerable enumerable)
            {
                datasets = enumerable.Cast<object>().ToArray();
                
                for (int i = 0; i < datasets.Length; i++)
                {
                    object data = datasets[i];
                    
                    ItemAddReport addReport = itemStrategy.AddItem(i, data);

                    GameObject item = addReport.Object;
                    bool isNew = addReport.IsNew;

                    if (item != null)
                    {
                        if (this.itemReceiver != null && item.TryGetComponent(this.itemReceiver, out Component itemReceiverComponent) && itemReceiverComponent is IUIListItemReceiver itemReceiver)
                        {
                            itemReceiver.SetListItem(new ListItem(i, data));
                        }
                
                        if (isNew)
                        {
                            InvokeCallback(UIListItemCallback.Initialized, i, item, data);
                        }
                
                        InvokeCallback(UIListItemCallback.Activated, i, item, data);
                    }
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
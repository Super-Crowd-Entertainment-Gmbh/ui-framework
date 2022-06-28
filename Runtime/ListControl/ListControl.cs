using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public abstract class ListControl : ControlBase
    {
        private Action<int, GameObject, object> onCreated;
        private Action<int, GameObject, object> onActivated;
        private Action<int, GameObject, object> onDeactivated;

        public abstract int Count { get; }
        public abstract IEnumerable<GameObject> Items { get; }

        public abstract void SetCount(int count);
        public abstract void SetCountByData(IEnumerable itemData);
        public abstract void Clear();
        public abstract GameObject GetItem(int index);
        public abstract int GetIndex(GameObject item);

        public void SetCallback(ListControlCallbacks target, Action<int, GameObject, object> callback)
        {
            switch (target)
            {
                case ListControlCallbacks.Created:
                    if (onCreated != null)
                    {
                        Debug.LogError("The callback was not empty before. You should check if this is right!", gameObject);
                    }
                    onCreated = callback;
                    break;
                case ListControlCallbacks.Activated:
                    if (onActivated != null)
                    {
                        Debug.LogError("The callback was not empty before. You should check if this is right!", gameObject);
                    }
                    onActivated = callback;
                    break;
                case ListControlCallbacks.Deactivated:
                    if (onDeactivated != null)
                    {
                        Debug.LogError("The callback was not empty before. You should check if this is right!", gameObject);
                    }
                    onDeactivated = callback;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }
        
        public void ClearCallback(ListControlCallbacks target)
        {
            switch (target)
            {
                case ListControlCallbacks.Created:
                    onCreated = null;
                    break;
                case ListControlCallbacks.Activated:
                    onActivated = null;
                    break;
                case ListControlCallbacks.Deactivated:
                    onDeactivated = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }
        
        protected void InvokeCallback(ListControlCallbacks target, int index, GameObject item, object data)
        {
            switch (target)
            {
                case ListControlCallbacks.Created:
                    onCreated?.Invoke(index, item, data);
                    break;
                case ListControlCallbacks.Activated:
                    if (onActivated != null)
                    {
                        onActivated?.Invoke(index, item, data);
                    }
                    else
                    {
                        var control = item.GetComponent<GenericControl>();
                        if (control)
                        {
                            control.SetContext(data);
                        }
                    }
                    break;
                case ListControlCallbacks.Deactivated:
                    onDeactivated?.Invoke(index, item, data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }
        
        protected void InformIndexReceiver(int index, GameObject itemObj)
        {
            foreach (ListIndexControl indexControl in itemObj.GetComponentsInChildren<ListIndexControl>())
            {
                indexControl.SetContext(index);
            }
        }
    }
}
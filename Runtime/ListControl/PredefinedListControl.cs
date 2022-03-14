using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class PredefinedListControl : ListControl
    {
        [SerializeField] private GameObject[] items;
        [SerializeField] private bool keepEmptyActive;
        
        private int count;
        private object[] itemData;
        
        public override int Count
        {
            get { return items.Length; }
        }

        public override IEnumerable<GameObject> Items
        {
            get { return items; }    
        }
        
        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < items.Length; i++)
            {
                InvokeCallback(ListControlCallbacks.Created, i, items[i], null);
                InformIndexReceiver(i, items[i]);
            }
            
            ClearItems();
        }

        public override void SetCount(int count)
        {
            this.count = count;
            itemData = new object[count];
            
            SetDirty();
        }

        public override void SetCountByData(IEnumerable itemData)
        {
            this.itemData = itemData.Cast<object>().ToArray();
            this.count = this.itemData.Length;
            
            RefreshItems();
            SetDirty();
        }

        public override void Clear()
        {
            this.itemData = Array.Empty<object>();
            this.count = 0;
            
            ClearItems();
            SetDirty();
        }

        public override GameObject GetItem(int index)
        {
            if (index >= 0 && index < items.Length)
            {
                return items[index];
            }

            return null;
        }

        public override int GetIndex(GameObject item)
        {
            return Array.IndexOf(items, item);
        }

        private void ClearItems()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].SetActive(keepEmptyActive);
            }
        }

        private void RefreshItems()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (i < count)
                {
                    items[i].SetActive(true);
                    InvokeCallback(ListControlCallbacks.Activated, i, items[i], i < itemData.Length ? itemData[i] : null);
                }
                else
                {
                    items[i].SetActive(keepEmptyActive);
                    InvokeCallback(ListControlCallbacks.Deactivated, i, items[i], i < itemData.Length ? itemData[i] : null);
                }
            }
        }
    }
}
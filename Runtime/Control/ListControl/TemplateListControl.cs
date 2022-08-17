using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class TemplateListControl : ListControlBase
    {
        [SerializeField] private RectTransform itemRoot;
        [OdinSerialize] private GameObject itemTemplate;

        private int count;
        private object[] itemData;
        
        private readonly List<GameObject> items = new List<GameObject>();
        private readonly Queue<GameObject> inactiveItems = new Queue<GameObject>();
        
        public override int Count
        {
            get { return count; }
        }
        
        public override IEnumerable<GameObject> Items
        {
            get { return items; }    
        }

        protected override void Awake()
        {
            base.Awake();
            
            itemTemplate.SetActive(false);
        }

        public override void SetCount(int count)
        {
            this.count = count;
            itemData = new object[count];
            
            RefreshItems();
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
            if (index >= 0 && index < items.Count)
            {
                return items[index];
            }

            return null;
        }

        public override int GetIndex(GameObject item)
        {
            return items.IndexOf(item);
        }

        private void ClearItems()
        {
            inactiveItems.Clear();
            
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetActive(false);
                inactiveItems.Enqueue(items[i]);
                
                IListPoolReturnHandler[] poolReturnHandlers = items[i].GetComponentsInChildren<IListPoolReturnHandler>();
                for (int j = 0; j < poolReturnHandlers.Length; j++)
                {
                    poolReturnHandlers[j].Returned();
                }
            }
        }

        private void RefreshItems()
        {
            ClearItems();

            for (int i = 0; i < count; i++)
            {
                GameObject itemObj;
                
                if (inactiveItems.Count > 0)
                {
                    itemObj = inactiveItems.Dequeue();
                    itemObj.gameObject.SetActive(true);
                }
                else
                {
                    itemObj = Instantiate(itemTemplate, itemRoot);
                    itemObj.gameObject.SetActive(true);
                    
                    items.Add(itemObj);
                    
                    InvokeCallback(ListControlCallbacks.Created, i, items[i], null);
                }
                
                InformIndexReceiver(i, items[i]);
                
                InvokeCallback(ListControlCallbacks.Activated, i, items[i], itemData[i]);
            }
        }
    }
}
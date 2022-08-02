using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ContextualTemplateListControl : ListControlBase
    {
        [SerializeField] private RectTransform itemRoot;
        [OdinSerialize] private Dictionary<Type, GameObject> itemTemplateMapping = new Dictionary<Type, GameObject>();

        private int capacity;
        private object[] itemData;
        
        private readonly List<GameObject> items = new List<GameObject>();

        public override int Count
        {
            get { return items.Count; }
        }
        
        public override IEnumerable<GameObject> Items
        {
            get { return items; }    
        }

        protected override void Awake()
        {
            base.Awake();

            foreach (GameObject itemTemplate in itemTemplateMapping.Values)
            {
                itemTemplate.SetActive(false);
            }
        }

        public override void SetCount(int count)
        {
            this.capacity = count;
            itemData = new object[count];
            
            RefreshItemsBalanced();
            SetDirty();
        }

        public override void SetCountByData(IEnumerable itemData)
        {
            this.itemData = itemData.Cast<object>().ToArray();
            this.capacity = this.itemData.Length;
            
            RefreshItemsTypeBased(this.itemData);
            SetDirty();
        }

        public override void Clear()
        {
            this.itemData = Array.Empty<object>();
            this.capacity = 0;
            
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
            for (int i = 0; i < items.Count; i++)
            {
                InvokeCallback(ListControlCallbacks.Deactivated, i, items[i], null);
                Destroy(items[i]);
            }
            items.Clear();
        }

        private void RefreshItemsBalanced()
        {
            ClearItems();

            int capacityPerType = capacity / itemTemplateMapping.Count;

            int baseIndex = 0;
            foreach (GameObject itemTemplate in itemTemplateMapping.Values)
            {
                for (int i = 0; i < capacityPerType; i++)
                {
                    GameObject itemObj = Instantiate(itemTemplate, itemRoot);
                    itemObj.gameObject.SetActive(true);
                    items.Add(itemObj);
                
                    InformIndexReceiver(i, itemObj);

                    InvokeCallback(ListControlCallbacks.Activated, baseIndex, items[baseIndex], itemData[baseIndex]);

                    baseIndex++;
                }
            }
        }
        
        private void RefreshItemsTypeBased(object[] items)
        {
            ClearItems();

            for (int i = 0; i < items.Length; i++)
            {
                Type itemType = items[i].GetType();

                if (itemTemplateMapping.TryGetValue(itemType, out GameObject itemTemplate))
                {
                    GameObject itemObj = Instantiate(itemTemplate, itemRoot);
                    itemObj.gameObject.SetActive(true);
                    this.items.Add(itemObj);
                    
                    InvokeCallback(ListControlCallbacks.Created, i, this.items[i], itemData[i]);
                    
                    InformIndexReceiver(i, itemObj);

                    InvokeCallback(ListControlCallbacks.Activated, i, this.items[i], itemData[i]);
                }
                else
                {
                    Debug.LogError($"No matching template found for '{itemType.Name}'.");
                }
            }
        }
    }
}
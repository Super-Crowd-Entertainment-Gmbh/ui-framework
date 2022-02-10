using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class TemplateListControl : ListControl
    {
        [SerializeField] private RectTransform itemRoot;
        [OdinSerialize] private GameObject itemTemplate;

        private int capacity;
        private object[] itemData;
        
        private readonly List<GameObject> activeItems = new List<GameObject>();
        private readonly Queue<GameObject> inactiveItems = new Queue<GameObject>();
        
        public override event Action<int, GameObject> CreatedItem;
        public override event Action<int, GameObject, object> ActivatedItem;
        public override event Action<int, GameObject, object> DeactivatedItem;

        public override int Count
        {
            get { return activeItems.Count; }
        }
        
        public override IEnumerable<GameObject> Items
        {
            get { return activeItems; }    
        }

        protected override void Awake()
        {
            base.Awake();
            
            itemTemplate.SetActive(false);
        }

        public override void SetCount(int count)
        {
            this.capacity = count;
            itemData = new object[count];
            
            RefreshItems();
            SetDirty();
        }

        public override void SetCountByData(IEnumerable<object> itemData)
        {
            this.itemData = itemData.ToArray();
            this.capacity = this.itemData.Length;
            
            RefreshItems();
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
            if (index >= 0 && index < activeItems.Count)
            {
                return activeItems[index];
            }

            return null;
        }

        public override int GetIndex(GameObject item)
        {
            return activeItems.IndexOf(item);
        }

        private void ClearItems()
        {
            for (int i = 0; i < activeItems.Count; i++)
            {
                activeItems[i].SetActive(false);
                inactiveItems.Enqueue(activeItems[i]);
            }
            
            activeItems.Clear();
        }

        private void RefreshItems()
        {
            ClearItems();

            for (int i = 0; i < capacity; i++)
            {
                GameObject itemObj;
                
                if (inactiveItems.Count > 0)
                {
                    itemObj = inactiveItems.Dequeue();
                    
                    itemObj.gameObject.SetActive(true);
                    activeItems.Add(itemObj);
                }
                else
                {
                    itemObj = Instantiate(itemTemplate, itemRoot);
                    
                    itemObj.gameObject.SetActive(true);
                    activeItems.Add(itemObj);
                    
                    CreatedItem?.Invoke(i, activeItems[i]);
                }
                
                InformIndexReceiver(i, activeItems[i]);
                
                ActivatedItem?.Invoke(i, activeItems[i], itemData[i]);
            }
        }
    }
}
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
        
        private readonly List<GameObject> items = new List<GameObject>();
        
        public override event Action<int, GameObject> CreatedItem;
        public override event Action<int, GameObject, object> ActivatedItem;
        public override event Action<int, GameObject, object> DeactivatedItem;

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
                Destroy(items[i]);
            }
            
            items.Clear();
        }

        private void RefreshItems()
        {
            ClearItems();

            for (int i = 0; i < capacity; i++)
            {
                GameObject itemObj = Instantiate(itemTemplate, itemRoot);
                itemObj.gameObject.SetActive(true);
                items.Add(itemObj);
                
                CreatedItem?.Invoke(i, items[i]);
                
                InformIndexReceiver(i, items[i]);
                
                ActivatedItem?.Invoke(i, items[i], itemData[i]);
            }
        }
    }
}
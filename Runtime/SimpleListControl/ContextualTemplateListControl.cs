using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ContextualTemplateListControl : ListControl
    {
        [SerializeField] private RectTransform itemRoot;
        [OdinSerialize] private Dictionary<Type, GameObject> itemTemplateMapping = new Dictionary<Type, GameObject>();

        private int capacity;
        private object[] itemData;
        
        private readonly List<GameObject> items = new List<GameObject>();

<<<<<<< HEAD
        public override event Action<int, GameObject> CreatedItem;
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
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

            foreach (GameObject itemTemplate in itemTemplateMapping.Values)
            {
                itemTemplate.SetActive(false);
            }
        }

        public override void SetCount(int count)
        {
            this.capacity = count;
            itemData = new object[count];
            
<<<<<<< HEAD
            RefreshItemsBalanced();
            SetDirty();
=======
            SetDirty();
            RefreshItemsBalanced();
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
        }

        public override void SetCount(IEnumerable<object> itemData)
        {
            this.itemData = itemData.ToArray();
            this.capacity = this.itemData.Length;
            
<<<<<<< HEAD
            RefreshItemsTypeBased(this.itemData);
            SetDirty();
        }

        public override void Clear()
        {
            this.itemData = Array.Empty<object>();
            this.capacity = 0;
            
            ClearItems();
            SetDirty();
=======
            SetDirty();
            RefreshItemsTypeBased(this.itemData);
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
        }

        public override GameObject GetItem(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                return items[index];
            }

            return null;
        }
        
        private void ClearItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                DeactivatedItem?.Invoke(i, items[i], itemData[i]);
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
                
                    ActivatedItem?.Invoke(baseIndex, items[baseIndex], itemData[baseIndex]);

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
            
<<<<<<< HEAD
                    CreatedItem?.Invoke(i, this.items[i]);
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
                    ActivatedItem?.Invoke(i, this.items[i], itemData[i]);
                }
                else
                {
                    Debug.LogError($"No matching template found for '{itemType.Name}'.");
                }
            }
        }
    }
}
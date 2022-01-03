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
        
<<<<<<< HEAD:Runtime/SimpleListControl/TemplateListControl.cs
        public override event Action<int, GameObject> CreatedItem;
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08:Runtime/ListControl/ListControl.cs
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
            
<<<<<<< HEAD:Runtime/SimpleListControl/TemplateListControl.cs
            RefreshItems();
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08:Runtime/ListControl/ListControl.cs
            SetDirty();
        }

        public override void SetCount(IEnumerable<object> itemData)
        {
            this.itemData = itemData.ToArray();
            this.capacity = this.itemData.Length;
            
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

        protected override void OnRefresh()
        {
            base.OnRefresh();
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
                
<<<<<<< HEAD:Runtime/SimpleListControl/TemplateListControl.cs
                CreatedItem?.Invoke(i, items[i]);
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08:Runtime/ListControl/ListControl.cs
                ActivatedItem?.Invoke(i, items[i], itemData[i]);
            }
        }
    }
}
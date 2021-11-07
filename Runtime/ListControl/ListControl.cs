using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ListControl : Control, IListControl
    {
        [SerializeField] private RectTransform itemRoot;
        [OdinSerialize] private GameObject itemTemplate;

        private int capacity;
        private readonly List<GameObject> items = new List<GameObject>();

        public event Action<int, GameObject> ActivatedItem;
        public event Action<int, GameObject> DeactivatedItem;

        public int Count
        {
            get { return items.Count; }
        }
        
        public IEnumerable<GameObject> Items
        {
            get { return items; }    
        }

        protected override void Awake()
        {
            base.Awake();
            
            itemTemplate.SetActive(false);
        }

        public void SetCount(int count)
        {
            this.capacity = count;
            SetDirty();
        }
        
        protected override void OnGotDirty()
        {
            base.OnGotDirty();
            
            RefreshItems();
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
                
                ActivatedItem?.Invoke(i, items[i]);
            }
        }
    }
}
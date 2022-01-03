using System;
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
        
<<<<<<< HEAD
        public override event Action<int, GameObject> CreatedItem;
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
        public override event Action<int, GameObject, object> ActivatedItem;
        public override event Action<int, GameObject, object> DeactivatedItem;

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

<<<<<<< HEAD
            for (int i = 0; i < items.Length; i++)
            {
                CreatedItem?.Invoke(i, items[i]);    
            }
            
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
            ClearItems();
        }

        public override void SetCount(int count)
        {
            this.count = count;
            itemData = new object[count];
            
            SetDirty();
        }

        public override void SetCount(IEnumerable<object> itemData)
        {
            this.itemData = itemData.ToArray();
            this.count = this.itemData.Length;
            
<<<<<<< HEAD
            RefreshItems();
            SetDirty();
        }

        public override void Clear()
        {
            this.itemData = Array.Empty<object>();
            this.count = 0;
            
            ClearItems();
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
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

<<<<<<< HEAD
=======
        protected override void OnRefresh()
        {
            base.OnRefresh();
            RefreshItems();
        }

>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
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
                    ActivatedItem?.Invoke(i, items[i], itemData[i]);
                }
                else
                {
                    items[i].SetActive(keepEmptyActive);
                    DeactivatedItem?.Invoke(i, items[i], itemData[i]);
                }
            }
        }
    }
}
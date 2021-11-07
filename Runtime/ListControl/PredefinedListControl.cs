using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class PredefinedListControl : Control, IListControl
    {
        [SerializeField] private GameObject[] items;

        private int count;
        
        public event Action<int, GameObject> ActivatedItem;
        public event Action<int, GameObject> DeactivatedItem;

        public int Count
        {
            get { return items.Length; }
        }

        public IEnumerable<GameObject> Items
        {
            get { return items; }    
        }
        
        protected override void Start()
        {
            base.Start();

            ClearItems();
        }

        public void SetCount(int count)
        {
            this.count = count;
            SetDirty();
        }

        protected override void OnGotDirty()
        {
            base.OnGotDirty();

            RefreshItems();
        }

        private void ClearItems()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].SetActive(false);
            }
        }

        private void RefreshItems()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (i < count)
                {
                    items[i].SetActive(true);
                    ActivatedItem?.Invoke(i, items[i]);
                }
                else
                {
                    items[i].SetActive(false);
                    DeactivatedItem?.Invoke(i, items[i]);
                }
            }
        }
    }
}
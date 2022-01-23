﻿using System;
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
        
        public override event Action<int, GameObject> CreatedItem;
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

            for (int i = 0; i < items.Length; i++)
            {
                CreatedItem?.Invoke(i, items[i]);  
                HandleListIndexControl(i, items[i]);
            }
            
            ClearItems();
        }

        public override void SetCount(int count)
        {
            this.count = count;
            itemData = new object[count];
            
            SetDirty();
        }

        public override void SetCountByData(IEnumerable<object> itemData)
        {
            this.itemData = itemData.ToArray();
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
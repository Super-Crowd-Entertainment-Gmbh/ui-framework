using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.UIFramework
{
    /// <summary>
    /// List strategy that creates new items for each data based on a single prefab.
    /// Support pooling of empty items.
    /// </summary>
    public class PrefabUIListStrategy : IUIListItemStrategy
    {
        private readonly GameObject root;
        private readonly GameObject itemPrefab;
        private readonly GetPrefabFunctionDelegate getItemPrefab;
        
        private readonly List<GameObject> items = new List<GameObject>();
        private readonly List<GameObject> emptyItems = new List<GameObject>();
        private readonly Queue<GameObject> emptyItemsQueue = new Queue<GameObject>();

        private bool keepEmptyActive;

        public PrefabUIListStrategy(GameObject root, GameObject itemPrefab)
        {
            this.root = root;
            this.itemPrefab = itemPrefab;

            if (!string.IsNullOrEmpty(itemPrefab.scene.name))
            {
                // Prefab is a scene object. Disable it.
                this.itemPrefab.SetActive(false);
            }
        }
        
        public IReadOnlyList<GameObject> Items
        {
            get { return items; }
        }

        public bool KeepEmptyActive
        {
            get { return keepEmptyActive; }
            set
            {
                keepEmptyActive = value;
                
                for (int i = this.emptyItems.Count - 1; i >= 0; i--)
                {
                    GameObject item = this.emptyItems[i];
                
                    item.SetActive(KeepEmptyActive);
                }
            }
        }

        public GameObject GetItem(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                return items[index];
            }

            return null;
        }
        
        public ItemAddReport AddItem(int index, object data)
        {
            ItemAddReport addReport;
            
            if (emptyItemsQueue.Count > 0)
            {
                GameObject item = emptyItemsQueue.Dequeue();
                
                emptyItems.Remove(item);
                
                item.transform.SetSiblingIndex(index);
                item.SetActive(true);
                
                addReport = new ItemAddReport(item, false);
            }
            else
            {
                GameObject item = Object.Instantiate(itemPrefab, root.transform);
                
                items.Add(item);

                item.transform.SetSiblingIndex(index);
                item.SetActive(true);
                    
                addReport = new ItemAddReport(item, true);
            }

            return addReport;
        }

        public void RemoveItem(int index)
        {
            items[index].SetActive(KeepEmptyActive);
            emptyItemsQueue.Enqueue(items[index]);
            emptyItems.Add(items[index]);
        }

        [Serializable]
        public class Dependencies
        {
            public GameObject itemRoot;
            public GameObject itemPrefab;
        }
    }
}
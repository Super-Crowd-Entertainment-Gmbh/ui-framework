using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.UIFramework
{
    public delegate GameObject GetPrefabFunctionDelegate(int index, object data);
    
    /// <summary>
    /// List strategy that allows to choose a different prefab dependent on index and data for each list item.
    /// Doesn't support pooling of empty items.
    /// </summary>
    public class ContextualPrefabUIListItemStrategy : IUIListItemStrategy
    {
        private readonly GameObject root;
        private readonly GetPrefabFunctionDelegate getItemPrefab;
        
        private readonly List<GameObject> items = new List<GameObject>();
        
        public ContextualPrefabUIListItemStrategy(GameObject root, GetPrefabFunctionDelegate getItemPrefab)
        {
            this.root = root;
            this.getItemPrefab = getItemPrefab;
        }
        
        public IReadOnlyList<GameObject> Items
        {
            get { return items; }
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
            GameObject itemPrefab = getItemPrefab.Invoke(index, data);
            
            GameObject item = Object.Instantiate(itemPrefab, root.transform);
            
            item.transform.SetSiblingIndex(index);
            item.SetActive(true);
                    
            items.Add(item);
                
            return new ItemAddReport(item, true);
        }

        public void RemoveItem(int index)
        {
            Object.Destroy(items[index]);
            items.RemoveAt(index);
        }

        [Serializable]
        public class Dependencies
        {
            public GameObject itemRoot;
        }
    }
}
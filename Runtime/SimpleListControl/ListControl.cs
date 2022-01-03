using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public abstract class ListControl : Control
    {
        public abstract event Action<int, GameObject> CreatedItem;
        public abstract event Action<int, GameObject, object> ActivatedItem;
        public abstract event Action<int, GameObject, object> DeactivatedItem;
        
        public abstract int Count { get; }
        public abstract IEnumerable<GameObject> Items { get; }

        public abstract void SetCount(int count);
        public abstract void SetCount(IEnumerable<object> itemData);
        public abstract void Clear();
        public abstract GameObject GetItem(int index);
    }
}
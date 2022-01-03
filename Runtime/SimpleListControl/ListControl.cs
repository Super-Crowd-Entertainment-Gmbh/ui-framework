using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public abstract class ListControl : Control
    {
<<<<<<< HEAD
        public abstract event Action<int, GameObject> CreatedItem;
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
        public abstract event Action<int, GameObject, object> ActivatedItem;
        public abstract event Action<int, GameObject, object> DeactivatedItem;
        
        public abstract int Count { get; }
        public abstract IEnumerable<GameObject> Items { get; }

        public abstract void SetCount(int count);
        public abstract void SetCount(IEnumerable<object> itemData);
<<<<<<< HEAD
        public abstract void Clear();
=======
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08
        public abstract GameObject GetItem(int index);
    }
}
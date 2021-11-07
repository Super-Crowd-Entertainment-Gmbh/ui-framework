using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public interface IListControl : IControl
    {
        event Action<int, GameObject> ActivatedItem;
        event Action<int, GameObject> DeactivatedItem;
        
        int Count { get; }
        IEnumerable<GameObject> Items { get; }

        void SetCount(int count);
    }
}
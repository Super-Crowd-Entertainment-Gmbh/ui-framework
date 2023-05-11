using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public interface IUIListItemStrategy
    {
        IReadOnlyList<GameObject> Items { get; }
        GameObject GetItem(int index);
        ItemAddReport AddItem(int index, object data);
        void RemoveItem(int index);
        void Clear();
    }
}
using UnityEngine;

namespace Rehawk.UIFramework
{
    public struct ItemAddReport
    {
        public GameObject Object { get; }
        public bool IsNew { get; }
        
        public ItemAddReport(GameObject obj, bool isNew)
        {
            Object = obj;
            IsNew = isNew;
        }
    }
}
using UnityEngine;

namespace Rehawk.UIFramework
{
    [DisallowMultipleComponent]
    public class ListIndexControl : ContextControlBase<int>
    {
        public int Index
        {
            get { return Context; }
        }
    }
}
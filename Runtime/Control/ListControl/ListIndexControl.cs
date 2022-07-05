using UnityEngine;

namespace Rehawk.UIFramework
{
    [DisallowMultipleComponent]
    public class ListIndexControl : Control<int>
    {
        public int Index
        {
            get { return Context; }
        }
    }
}
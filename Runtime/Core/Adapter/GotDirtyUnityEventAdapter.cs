using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework
{
    public class GotDirtyUnityEventAdapter : Adapter<Control>
    {
        [SerializeField] private UnityEvent gotDirty;
        
        protected override void OnRefresh()
        {
            base.OnRefresh();

            gotDirty.Invoke();
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework
{
    public class GotDirtyUnityEventAdapter : ControlAdapter<ControlBase>
    {
        [SerializeField] private UnityEvent gotDirty;
        
        protected override void OnRefresh()
        {
            base.OnRefresh();

            gotDirty.Invoke();
        }
    }
}
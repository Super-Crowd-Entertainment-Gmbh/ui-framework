using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework.Adapter
{
    public class GotDirtyUnityEventAdapter : Adapter<Control>
    {
        [SerializeField] private UnityEvent gotDirty;
        
        protected override void OnDirty()
        {
            base.OnDirty();

            gotDirty.Invoke();
        }
    }
}
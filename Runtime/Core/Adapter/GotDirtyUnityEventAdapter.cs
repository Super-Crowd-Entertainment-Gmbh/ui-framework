using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework.Adapter
{
    public class GotDirtyUnityEventAdapter : Adapter<Control>
    {
        [SerializeField] private UnityEvent gotDirty;
        
<<<<<<< HEAD
        protected override void OnRefresh()
        {
            base.OnRefresh();
=======
        protected override void OnDirty()
        {
            base.OnDirty();
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08

            gotDirty.Invoke();
        }
    }
}
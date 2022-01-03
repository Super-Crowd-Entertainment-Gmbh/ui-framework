using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetEnableByCountAdapter : Adapter<ListControl>
    {
        [SerializeField] private Behaviour[] behaviours;
        [SerializeField] private int count;
        [SerializeField] private bool state;

<<<<<<< HEAD
        protected override void OnRefresh()
        {
            base.OnRefresh();
=======
        protected override void OnDirty()
        {
            base.OnDirty();
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08

            bool isEnabled = Control.Count == count ? state : !state;
            
            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.enabled = isEnabled;
            }
        }
    }
}
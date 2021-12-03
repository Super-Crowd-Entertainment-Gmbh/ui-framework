using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetEnableByCountAdapter : Adapter<ListControl>
    {
        [SerializeField] private Behaviour[] behaviours;
        [SerializeField] private int count;
        [SerializeField] private bool state;

        protected override void OnDirty()
        {
            base.OnDirty();

            bool isEnabled = Control.Count == count ? state : !state;
            
            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.enabled = isEnabled;
            }
        }
    }
}
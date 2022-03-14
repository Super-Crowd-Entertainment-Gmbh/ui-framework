using UnityEngine;

namespace Rehawk.UIFramework
{
    public class SetEnableByCountAdapter : Adapter<ListControl>
    {
        [SerializeField] private Behaviour[] behaviours;
        [SerializeField] private int count;
        [SerializeField] private bool state;

        protected override void OnRefresh()
        {
            base.OnRefresh();

            bool isEnabled = Control.Count == count ? state : !state;
            
            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.enabled = isEnabled;
            }
        }
    }
}
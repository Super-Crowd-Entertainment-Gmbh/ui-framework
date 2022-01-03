using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetEnableByContextAdapter : Adapter<ContextControl>
    {
        [SerializeField] private Behaviour[] behaviours;
        [SerializeField] private bool state = true;
        
        protected override void OnRefresh()
        {
            base.OnRefresh();

            bool isEnabled = Control.HasContext ? state : !state;
            
            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.enabled = isEnabled;
            }
        }
    }
}
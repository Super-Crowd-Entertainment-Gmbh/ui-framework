using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetActiveByContextAdapter : Adapter<ContextControl>
    {
        [SerializeField] private GameObject[] objects;
        [SerializeField] private bool state = true;
        
        protected override void OnRefresh()
        {
            base.OnRefresh();

            bool isActive = Control.HasContext ? state : !state;
            
            foreach (GameObject obj in objects)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
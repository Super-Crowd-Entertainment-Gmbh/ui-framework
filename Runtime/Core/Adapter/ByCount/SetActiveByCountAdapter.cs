using UnityEngine;

namespace Rehawk.UIFramework
{
    public class SetActiveByCountAdapter : Adapter<ListControl>
    {
        [SerializeField] private GameObject[] objects;
        [SerializeField] private int count;
        [SerializeField] private bool state;

        protected override void OnRefresh()
        {
            base.OnRefresh();

            bool isActive = Control.Count == count ? state : !state;
            
            foreach (GameObject obj in objects)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
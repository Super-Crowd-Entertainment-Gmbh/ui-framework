using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetActiveByCountAdapter : Adapter<ListControl>
    {
        [SerializeField] private GameObject[] objects;
        [SerializeField] private int count;
        [SerializeField] private bool state;

        protected override void OnDirty()
        {
            base.OnDirty();

            bool isActive = Control.Count == count ? state : !state;
            
            foreach (GameObject obj in objects)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
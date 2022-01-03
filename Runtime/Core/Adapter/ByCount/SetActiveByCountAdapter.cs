using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetActiveByCountAdapter : Adapter<ListControl>
    {
        [SerializeField] private GameObject[] objects;
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

            bool isActive = Control.Count == count ? state : !state;
            
            foreach (GameObject obj in objects)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
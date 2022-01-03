using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetActiveByContextAdapter : Adapter<ContextControl>
    {
        [SerializeField] private GameObject[] objects;
        [SerializeField] private bool state = true;
        
<<<<<<< HEAD
        protected override void OnRefresh()
        {
            base.OnRefresh();
=======
        protected override void OnDirty()
        {
            base.OnDirty();
>>>>>>> ff995eef74f33e279537f12bcdb7a5a240041a08

            bool isActive = Control.HasContext ? state : !state;
            
            foreach (GameObject obj in objects)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
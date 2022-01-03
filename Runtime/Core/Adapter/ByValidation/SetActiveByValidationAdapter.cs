using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetActiveByValidationAdapter : Adapter<Control>
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

            bool isActive = Control.IsValid ? state : !state;
            
            foreach (GameObject obj in objects)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
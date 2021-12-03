using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetActiveByValidationAdapter : Adapter<Control>
    {
        [SerializeField] private GameObject[] objects;
        [SerializeField] private bool state = true;
        
        protected override void OnDirty()
        {
            base.OnDirty();

            bool isActive = Control.IsValid ? state : !state;
            
            foreach (GameObject obj in objects)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
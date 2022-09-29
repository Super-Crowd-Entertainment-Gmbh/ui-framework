using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ControlGroup : ControlBase
    {
        [SerializeField] private ControlBase[] controls;

        protected override void OnRefresh()
        {
            base.OnRefresh();

            for (int i = 0; i < controls.Length; i++)
            {
                controls[i].SetDirty();
            }
        }
    }
}
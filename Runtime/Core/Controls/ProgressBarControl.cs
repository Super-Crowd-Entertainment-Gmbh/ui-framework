using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.UIFramework.Controls
{
    public class ProgressBarControl : Control<float>
    {
        [SerializeField] private Image filler;

        public override bool HasContext
        {
            get { return Context > 0; }
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();

            filler.fillAmount = Context;
        }
    }
}
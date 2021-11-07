using System;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class SyncActiveWithVisibilityAdapter : Adapter<View>
    {
        [SerializeField] private GameObject[] targets;
        [SerializeField] private bool invert;
        
        protected override void Start()
        {
            base.Start();
            
            Element.BecameVisible += OnElementBecameVisible;
            Element.BecameInvisible += OnElementBecameInvisible;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Element.BecameVisible -= OnElementBecameVisible;
            Element.BecameInvisible -= OnElementBecameInvisible;
        }

        private void OnElementBecameVisible(object sender, EventArgs e)
        {
            foreach (GameObject target in targets)
            {
                target.SetActive(!invert);
            }
        }
        
        private void OnElementBecameInvisible(object sender, EventArgs e)
        {
            foreach (GameObject target in targets)
            {
                target.SetActive(invert);
            }
        }
    }
}
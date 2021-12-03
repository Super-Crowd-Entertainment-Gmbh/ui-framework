﻿using UnityEngine;

namespace Rehawk.UIFramework.Adapter
{
    public class SetEnableByValidationAdapter : Adapter<Control>
    {
        [SerializeField] private Behaviour[] behaviours;
        [SerializeField] private bool state = true;
        
        protected override void OnDirty()
        {
            base.OnDirty();

            bool isEnabled = Control.IsValid ? state : !state;
            
            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.enabled = isEnabled;
            }
        }
    }
}
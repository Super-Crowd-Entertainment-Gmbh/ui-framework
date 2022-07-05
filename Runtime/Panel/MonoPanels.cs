using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public static class MonoPanels
    {
        private static List<Panel> panels = new List<Panel>();

        public static void Register(Panel panel)
        {
            if (!panels.Contains(panel))
            {
                panels.Add(panel);
                
                panel.BecameVisible += OnPanelBecameVisible;
            }    
        }

        public static void Unregister(Panel panel)
        {
            if (panels.Contains(panel))
            {
                panels.Remove(panel); 
               
                panel.BecameVisible -= OnPanelBecameVisible;
            }
        }
        
        private static void OnPanelBecameVisible(Panel panel)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i] != panel)
                {
                    panels[i].SetVisible(false);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            panels = new List<Panel>();
        }
    }
}
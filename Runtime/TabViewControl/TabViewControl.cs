using System;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class TabViewControl : ControlBase
    {
        [SerializeField] private Panel defaultTab;
        [SerializeField] private Panel[] tabs;
        
        private int activeTabIndex;

        public void OpenTab(int index)
        {
            if (index < 0 || index > tabs.Length)
            {
                Debug.LogError($"Given index '{index}' is not inside of this tab control.", gameObject);
                return;
            }

            activeTabIndex = index;
            
            Panel tab = tabs[index];
            
            CloseAll();
            tab.SetVisible(true);
        }
        
        public void OpenTab(Panel tab)
        {
            int index = Array.IndexOf(tabs, tab);
            OpenTab(index);
        }

        private void CloseAll()
        {
            foreach (Panel panel in tabs)
            {
                panel.SetVisible(false);
            }
        }
        
        protected override void OnPanelBecameVisible()
        {
            base.OnPanelBecameVisible();
            
            OpenTab(defaultTab);
        }
    }
}
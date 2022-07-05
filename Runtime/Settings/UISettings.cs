using UnityEngine;

namespace Rehawk.UIFramework
{
    public class UISettings : ScriptableObject
    {
        private const string ASSET_PATH = "Assets/Resources/UISettings.asset";

        [SerializeField] private bool usePoller = true;
        
        private static UISettings instance;
        
        public static UISettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<UISettings>("UISettings");

#if UNITY_EDITOR
                    if (instance == null)
                    {
                        instance = CreateInstance<UISettings>();
                
                        UnityEditor.AssetDatabase.CreateAsset(instance, ASSET_PATH);
                        UnityEditor.AssetDatabase.SaveAssets();
                    }
#endif
                }

                return instance;
            }
        }
        
        public static bool UsePoller
        {
            get { return Instance.usePoller; }
        }
    }
}
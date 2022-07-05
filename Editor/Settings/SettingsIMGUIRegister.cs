using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rehawk.UIFramework
{
    internal static class SettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateKiteDialogueSettingsProvider()
        {
            var provider = new SettingsProvider("Project/UI Framework", SettingsScope.Project)
            {
                label = "UI Framework",
                
                guiHandler = searchContext =>
                {
                    SerializedObject settings = SettingsReceiver.GetSerializedSettings();
                    
                    EditorGUILayout.PropertyField(settings.FindProperty("usePoller"), new GUIContent("Use Poller", "If TRUE controls and adapters are refreshed async."));
                    
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },

                keywords = new HashSet<string>(new[] { "Use Poller", })
            };

            return provider;
        }
    }
}
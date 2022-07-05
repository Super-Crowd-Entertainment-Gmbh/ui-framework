using UnityEditor;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public static class SettingsReceiver
    {
        private static UISettings GetOrCreateSettings()
        {
            return UISettings.Instance;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}
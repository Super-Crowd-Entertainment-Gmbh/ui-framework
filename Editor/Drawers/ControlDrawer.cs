using System;
using System.Collections.Generic;
using System.Linq;
using Rehawk.UIFramework.Utilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ControlDrawer<TControl> : OdinValueDrawer<TControl> where TControl : ControlBase
    {
        private ControlBase[] controls;
        private string[] options;

        protected override void Initialize()
        {
            base.Initialize();

            Evaluate();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (label == null)
            {
                label = GUIContent.none;
            }
            
            Evaluate();
            
            EditorGUILayout.BeginHorizontal();
            {
                ControlBase control = this.ValueEntry.SmartValue;
                
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PrefixLabel(label);
                    
                    control = (ControlBase) EditorGUILayout.ObjectField(GUIContent.none, control, typeof(ControlBase), true);
                    
                    if (controls.Length > 0)
                    {
                        int newControlIndex = 0;

                        if (control)
                        {
                            newControlIndex = Array.IndexOf(controls, control);
                        }

                        EditorGUI.BeginChangeCheck();
                        {
                            newControlIndex = SirenixEditorFields.Dropdown(newControlIndex, options, GUILayout.Width(20));
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            control = controls[newControlIndex];
                        }
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    this.ValueEntry.WeakSmartValue = control;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Evaluate()
        {
            GameObject gameObject = null;
            
            TControl control = this.ValueEntry.SmartValue;

            if (control)
            {
                gameObject = control.gameObject;
            }
            else
            {
                object serializedObject = this.Property.SerializationRoot.ValueEntry.WeakSmartValue;

                if (serializedObject is Component component)
                {
                    gameObject = component.gameObject;
                }
            }

            if (gameObject)
            {
                controls = gameObject.GetComponents(typeof(ControlBase)).Cast<ControlBase>().ToArray();

                options = new string[controls.Length];

                var countByType = new Dictionary<Type, int>();
                for (int i = 0; i < controls.Length; i++)
                {
                    Type controlType = controls[i].GetType();

                    options[i] = controlType.Name;

                    if (countByType.TryGetValue(controlType, out int count))
                    {
                        options[i] += $" ({count})";
                        
                        countByType[controlType] += 1;
                    }
                    else
                    {
                        countByType.Add(controlType, 1);
                    }
                }
            }
            else
            {
                controls = new ControlBase[0];
                options = new string[0];
            }
        }
    }
}
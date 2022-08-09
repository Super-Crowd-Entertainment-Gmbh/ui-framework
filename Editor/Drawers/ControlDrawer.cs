using System;
using System.Collections.Generic;
using System.Linq;
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

        private bool hasError;
        private string errorText;

        protected override void Initialize()
        {
            base.Initialize();

            Evaluate();
            Validate();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (label == null)
            {
                label = GUIContent.none;
            }
            
            Evaluate();
            
            if (hasError && !string.IsNullOrEmpty(errorText))
            {
                EditorGUILayout.HelpBox(errorText, MessageType.Error);
            }

            EditorGUILayout.BeginHorizontal();
            {
                ControlBase control = this.ValueEntry.SmartValue;

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PrefixLabel(label);
                    
                    Color previousColor = GUI.color;

                    if (hasError)
                    {
                        GUI.color = Color.red;
                    }

                    control = (ControlBase) EditorGUILayout.ObjectField(GUIContent.none, control, this.ValueEntry.TypeOfValue, true);
                    
                    GUI.color = previousColor;

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
                    Validate();
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

        private void Validate()
        {
            hasError = false;
            errorText = string.Empty;
            
            var controlAttribute = this.Property.GetAttribute<ControlAttribute>();
            ControlBase control = this.ValueEntry.SmartValue;

            if (controlAttribute != null && control != null)
            {
                if (control is ContextControlBase contextControl)
                {
                    hasError = controlAttribute.disallowContext;

                    if (hasError)
                    {
                        errorText = "No context controls allowed.";
                        return;
                    }
                    
                    hasError = !controlAttribute.contextTypes.Contains(contextControl.ContextType);

                    if (hasError)
                    {
                        errorText = "Wrong context type.";
                        return;
                    }
                }
                else
                {
                    hasError = controlAttribute.contextTypes.Length > 0;
                    
                    if (hasError)
                    {
                        errorText = "No non context controls allowed.";
                        return;
                    }
                }
            }
        }
    }
}
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
    public class BindingAttributeDrawer : OdinAttributeDrawer<BindingAttribute, string>
    {
        private const int MAX_BINDING_DEPTH = 2;
        
        private BindingUtility.MemberPointer[] pointers;
        private string[] options;

        private BindingUtility.MemberPointer selectedPointer;
        private Type previousContextType;
        private Type previousControlType;

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
                string path = this.ValueEntry.SmartValue;

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PrefixLabel(label);
                    
                    Color previousColor = GUI.color;

                    if (hasError)
                    {
                        GUI.color = Color.red;
                    }

                    path = EditorGUILayout.TextField(GUIContent.none, path);
                    
                    GUI.color = previousColor;

                    if (pointers.Length > 0)
                    {
                        EditorGUI.BeginChangeCheck();
                        {
                            selectedPointer = SirenixEditorFields.Dropdown(selectedPointer, pointers, options, GUILayout.Width(20));
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            path = selectedPointer.Path;
                        }
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    this.ValueEntry.SmartValue = path;
                    Validate();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Evaluate()
        {
            object serializedObject = this.Property.SerializationRoot.ValueEntry.WeakSmartValue;

            Type contextType = null;
            Type controlType = null;
            
            if (serializedObject != null)
            {
                controlType = serializedObject.GetType();
            }
            
            if (serializedObject is AdapterBase adapter)
            {
                serializedObject = adapter.GetControl();
            }

            if (serializedObject is ControlBase control)
            {
                controlType = control.GetType();
            }

            if (serializedObject is ContextControlBase contextControl)
            {
                contextType = contextControl.ContextType;
            }

            if (previousContextType != contextType || previousControlType != controlType)
            {
                var pointerList = new List<BindingUtility.MemberPointer>(); 
                var optionList = new List<string>();

                if (contextType != null)
                {
                    BindingUtility.GetMemberPathsByDepth(contextType, MAX_BINDING_DEPTH, ref pointerList, "", "Context");
                }

                if (controlType != null)
                {
                    BindingUtility.GetMemberPathsByDepth(controlType, MAX_BINDING_DEPTH, ref pointerList, "_Control", "Control");
                }
                
                pointerList = pointerList.OrderBy(p => p.PrettifiedPath).ThenBy(p => p.Type).ToList();
                
                foreach (BindingUtility.MemberPointer pointer in pointerList)
                {
                    optionList.Add(pointer.PrettifiedPath.Replace('.', '/'));
                }

                pointers = pointerList.ToArray();
                options = optionList.ToArray();

                selectedPointer = pointerList.FirstOrDefault(p => p.Path == this.ValueEntry.SmartValue);    

                previousContextType = contextType;
                previousControlType = controlType;
            }
        }

        private void Validate()
        { 
            hasError = false;
            errorText = string.Empty;
            
            string path = this.ValueEntry.SmartValue;

            if (!string.IsNullOrEmpty(path) && !path.Contains("("))
            {
                hasError = pointers.Count(p => p.Path == path) == 0;
            }
        }
    }
}
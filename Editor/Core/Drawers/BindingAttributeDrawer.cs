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
        private Type previousType;
        
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
                string path = this.ValueEntry.SmartValue;
                
                EditorGUI.BeginChangeCheck();
                {
                    path = EditorGUILayout.TextField(label, path);

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
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void Evaluate()
        {
            object serializedObject = this.Property.SerializationRoot.ValueEntry.WeakSmartValue;

            Type baseType = null;

            if (serializedObject != null)
            {
                baseType = serializedObject.GetType();
            }
            
            if (serializedObject is Adapter adapter)
            {
                serializedObject = adapter.GetControl();
            }

            if (serializedObject is Control control)
            {
                baseType = control.GetType();
            }
            
            if (serializedObject is ContextControl contextControl)
            {
                baseType = contextControl.ContextType;
            }

            if (previousType != baseType)
            {
                var pointerList = new List<BindingUtility.MemberPointer>(); 
                var optionList = new List<string>();

                if (baseType != null)
                {
                    BindingUtility.GetMemberPathsByDepth(baseType, MAX_BINDING_DEPTH, ref pointerList);

                    pointerList = pointerList.OrderBy(p => p.PrettifiedPath).ThenBy(p => p.Type).ToList();
                
                    foreach (BindingUtility.MemberPointer pointer in pointerList)
                    {
                        optionList.Add(pointer.PrettifiedPath.Replace('.', '/'));
                    }

                    pointers = pointerList.ToArray();
                    options = optionList.ToArray();

                    selectedPointer = pointerList.FirstOrDefault(p => p.Path == this.ValueEntry.SmartValue);    
                }

                previousType = baseType;
            }
        }
    }
}
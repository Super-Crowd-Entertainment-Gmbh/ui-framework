﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.UIFramework.Utilities
{
    // TODO: Some caching would be nice!
    public static class BindingUtility
    {
        public static object GetValueByPath(object instance, string path)
        {
            if (instance == null)
                return null;
            
            Type currentType = instance.GetType();
            object value = instance;

            foreach (string member in path.Split('.'))
            {
                int bracketsStart = member.IndexOf("[");
                int bracketsEnd = member.IndexOf("]");

                string memberName = member;
                
                if (bracketsStart > 0)
                {
                    memberName = memberName.Substring(0, bracketsStart);
                }
                
                FieldInfo fieldInfo = currentType.GetField(memberName, BindingFlags.Instance | BindingFlags.Public);
                if (fieldInfo != null)
                {
                    value = fieldInfo.GetValue(value);

                    if (bracketsStart > 0)
                    {
                        string indexer = member.Substring(bracketsStart + 1, bracketsEnd - bracketsStart - 1);
                        indexer = indexer.Replace("\"", "");
                        indexer = indexer.Replace("'", "");

                        value = GetValueFromIndexer(value, indexer);
                    }
                    
                    currentType = value != null ? value.GetType() : null;
                }
                else
                {
                    PropertyInfo propertyInfo = currentType.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public);
                    if (propertyInfo != null)
                    {
                        value = propertyInfo.GetValue(value, null);
                        
                        if (bracketsStart > 0)
                        {
                            string indexer = member.Substring(bracketsStart + 1, bracketsEnd - bracketsStart - 1);
                            indexer = indexer.Replace("\"", "");
                            indexer = indexer.Replace("'", "");
                            
                            value = GetValueFromIndexer(value, indexer);
                        }
                    
                        currentType = value != null ? value.GetType() : null;
                    }
                    else
                    {
                        int braceStart = memberName.IndexOf("(");
                        int braceEnd = memberName.IndexOf(")");

                        string parameterValue = string.Empty;
                        
                        MethodInfo methodInfo;
                        
                        if (braceStart > 0)
                        {
                            parameterValue = memberName.Substring(braceStart + 1, braceEnd - braceStart - 1);
                            parameterValue = parameterValue.Replace("\"", "");
                            parameterValue = parameterValue.Replace("'", "");
                            
                            memberName = memberName.Substring(0, braceStart);
                            
                            methodInfo = currentType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                .FirstOrDefault(m =>
                                {
                                    if (m.Name != memberName)
                                    {
                                        return false;
                                    }

                                    ParameterInfo[] parameters = m.GetParameters();
                                    if (parameters.Length != 1)
                                    {
                                        return false;
                                    }

                                    return parameters[0].ParameterType == typeof(string);
                                });
                        }
                        else
                        {
                            methodInfo = currentType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                .FirstOrDefault(m => m.Name == memberName && m.GetParameters().Length == 0);
                        }

                        if (methodInfo != null)
                        {
                            if (string.IsNullOrEmpty(parameterValue))
                            {
                                value = methodInfo.Invoke(value, null);
                            }
                            else
                            {
                                value = methodInfo.Invoke(value, new object[]
                                {
                                    parameterValue
                                });
                            }
                            
                            currentType = value != null ? value.GetType() : null;
                        }
                    }
                }
                
                if (value == null)
                {
                    break;
                }
            }

            return value;
        }

        public static void GetMemberPathsByDepth(Type baseType, int depth, ref List<MemberPointer> pointers, string basePath = "", string prettifiedBasePath = "")
        {
            pointers.Add(new MemberPointer
            {
                Type = MemberPointer.MemberTypes.Property,
                Name = "Self",
                Path = basePath,
                PrettifiedPath = $"{prettifiedBasePath}.Self [{baseType.GetFriendlyName()}]"
            });
            
            GetMemberPathsByDepth(baseType, basePath, prettifiedBasePath, depth, ref pointers);
        }

        private static int GetMemberPathsByDepth(Type baseType, string basePath, string prettifiedBasePath, int depth, ref List<MemberPointer> pointers)
        {
            int previousCount = pointers.Count;
            
            // FIELDS
            
            FieldInfo[] fieldInfos = baseType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                // Skip delegates and baking fields
                if (fieldInfo.FieldType.IsSubclassOf(typeof(Delegate)) || fieldInfo.FieldType.IsSubclassOf(typeof(UnityEventBase)) || fieldInfo.Name.StartsWith("_") || fieldInfo.Name.StartsWith("m_") || fieldInfo.Name.StartsWith("<") || fieldInfo.Name.EndsWith("_"))
                    continue;
                
                string internalBasePath;
                string internalPrettifiedBasePath;
                
                if (string.IsNullOrEmpty(basePath))
                {
                    internalBasePath = $"{fieldInfo.Name}";
                }
                else
                {
                    internalBasePath = $"{basePath}.{fieldInfo.Name}";
                }

                if (string.IsNullOrEmpty(prettifiedBasePath))
                {
                    internalPrettifiedBasePath = $"Fields.{fieldInfo.Name}";
                }
                else
                {
                    internalPrettifiedBasePath = $"{prettifiedBasePath}.Fields.{fieldInfo.Name}";
                }

                string prettifiedPath = internalPrettifiedBasePath;

                if (depth > 0)
                {
                    if (GetMemberPathsByDepth(fieldInfo.FieldType, internalBasePath, internalPrettifiedBasePath, Mathf.Max(0, depth - 1), ref pointers) > 0)
                    {
                        prettifiedPath += $".Self [{fieldInfo.FieldType.GetFriendlyName()}]";
                    }
                }

                pointers.Add(new MemberPointer
                {
                    Type = MemberPointer.MemberTypes.Field,
                    Name = fieldInfo.Name,
                    Path = internalBasePath,
                    PrettifiedPath = prettifiedPath,
                });
            }

            // PROPERTIES
            
            PropertyInfo[] propertyInfos = baseType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                // Skip delegates and baking fields
                if (propertyInfo.PropertyType.IsSubclassOf(typeof(Delegate)) || propertyInfo.PropertyType.IsSubclassOf(typeof(UnityEventBase)) || propertyInfo.Name.StartsWith("_") || propertyInfo.Name.StartsWith("m_") || propertyInfo.Name.StartsWith("<") || propertyInfo.Name.EndsWith("_"))
                    continue;
                
                string internalBasePath;
                string internalPrettifiedBasePath;
                
                if (string.IsNullOrEmpty(basePath))
                {
                    internalBasePath = $"{propertyInfo.Name}";
                }
                else
                {
                    internalBasePath = $"{basePath}.{propertyInfo.Name}";
                }

                if (string.IsNullOrEmpty(prettifiedBasePath))
                {
                    internalPrettifiedBasePath = $"Properties.{propertyInfo.Name}";
                }
                else
                {
                    internalPrettifiedBasePath = $"{prettifiedBasePath}.Properties.{propertyInfo.Name}";
                }

                string prettifiedPath = internalPrettifiedBasePath;

                if (depth > 0)
                {
                    if (GetMemberPathsByDepth(propertyInfo.PropertyType, internalBasePath, internalPrettifiedBasePath, Mathf.Max(0, depth - 1), ref pointers) > 0)
                    {
                        prettifiedPath += $".Self [{propertyInfo.PropertyType.GetFriendlyName()}]";
                    }
                }
                
                pointers.Add(new MemberPointer
                {
                    Type = MemberPointer.MemberTypes.Property,
                    Name = propertyInfo.Name,
                    Path = internalBasePath,
                    PrettifiedPath = prettifiedPath,
                });
            }
            
            // METODS
            
            MethodInfo[] methodInfos = baseType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            
            foreach (MethodInfo methodInfo in methodInfos)
            {
                // Skip methods which are generic, without return type and getters/setters of properties
                if (methodInfo.IsGenericMethod || methodInfo.ReturnType == typeof(void) || methodInfo.Name.StartsWith("get_") || methodInfo.Name.StartsWith("set_"))
                    continue;
                
                ParameterInfo[] parameters = methodInfo.GetParameters();

                // Skip methods which have more then one parameter or non string parameters.
                if (parameters.Length > 1 || (parameters.Length == 1 && parameters[0].ParameterType != typeof(string))) 
                    continue;
            
                string internalBasePath;
                string internalPrettifiedBasePath;

                string methodName = $"{methodInfo.Name}(";

                if (parameters.Length > 0)
                {
                    methodName += $"\"{parameters[0].Name.ToLower()}\"";
                }

                methodName += ")";

                if (string.IsNullOrEmpty(basePath))
                {
                    internalBasePath = $"{methodName}";
                }
                else
                {
                    internalBasePath = $"{basePath}.{methodName}";
                }
                
                if (string.IsNullOrEmpty(prettifiedBasePath))
                {
                    internalPrettifiedBasePath = $"Methods.{methodName}";
                }
                else
                {
                    internalPrettifiedBasePath = $"{prettifiedBasePath}.Methods.{methodName} [{methodInfo.ReturnType.GetFriendlyName()}]";
                }

                string prettifiedPath = internalPrettifiedBasePath;

                pointers.Add(new MemberPointer
                {
                    Type = MemberPointer.MemberTypes.Method,
                    Name = methodInfo.Name,
                    Path = internalBasePath,
                    PrettifiedPath = prettifiedPath,
                });
            }

            return pointers.Count - previousCount;
        }

        private static object GetValueFromIndexer(object instance, object indexer)
        {
            object value = instance;

            foreach (Type type in value.GetType().GetInterfaces())
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    MethodInfo methodInfo = typeof(BindingUtility).GetMethod(nameof(GetDictionaryElement), BindingFlags.Static | BindingFlags.NonPublic);
                    value = methodInfo.MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new[] { value, indexer });
                    
                    break;
                }
                
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    MethodInfo methodInfo = typeof(BindingUtility).GetMethod(nameof(GetListElement), BindingFlags.Static | BindingFlags.NonPublic);
                    value = methodInfo.MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new[] { value, indexer });
                    
                    break;
                }
            }

            return value;
        }
        
        private static TValue GetDictionaryElement<TKey, TValue>(IDictionary<TKey, TValue> dict, object index)
        {
            TKey key = (TKey)Convert.ChangeType(index, typeof(TKey), null);
            return dict[key];
        }

        private static T GetListElement<T>(IList<T> list, object index)
        {
            return list[Convert.ToInt32(index)];
        }
        
        public struct MemberPointer
        {
            public MemberTypes Type { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public string PrettifiedPath { get; set; }
            
            public enum MemberTypes
            {
                Field,
                Property,
                Method
            }
        }
    }
}
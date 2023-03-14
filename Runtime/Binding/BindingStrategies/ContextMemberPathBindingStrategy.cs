using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Rehawk.UIFramework
{
    public class ContextMemberPathBindingStrategy : IBindingStrategy
    {
        private readonly Func<object> getOrigin;
        private readonly string memberPath;
        private MemberInfo[] memberInfos;
        private object[] memberValues;

        private string memberName;
        
        private object origin;
        private object context;
        private object value;
        
        public event Action GotDirty;

        public ContextMemberPathBindingStrategy(Func<object> getOrigin, string memberPath)
        {
            this.getOrigin = getOrigin;
            this.memberPath = memberPath;
        }

        public void Evaluate()
        {
            UnLinkFromEvents();
            
            origin = getOrigin?.Invoke();
            
            EvaluateMemberPath();
            EvaluateContext();
            
            value = Get();

            LinkToEvents();
        }
        
        public void Release()
        {
            UnLinkFromEvents();
        }

        private void EvaluateMemberPath()
        {
            memberInfos = null;
            memberValues = null;
            
            if (origin != null && !string.IsNullOrEmpty(memberPath))
            {
                string[] pathParts = memberPath.Split('.');

                memberInfos = new MemberInfo[pathParts.Length];
                memberValues = new object[pathParts.Length - 1];
                
                Type type = origin.GetType();
                
                for (int i = 0; i < pathParts.Length; i++)
                {
                    memberName = pathParts[i];
                    
                    MemberInfo memberInfo = type.GetMember(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                        .FirstOrDefault();
                    
                    if (memberInfo == null)
                    {
                        Debug.LogError($"Part of path to member couldn't be found. [memberName={memberName}, path={memberPath}]");
                        break;
                    }

                    memberInfos[i] = memberInfo;
                    
                    type = memberInfo.GetUnderlyingType();
                    
                    if (type == null)
                    {
                        Debug.LogError($"Type of member was null. [memberName={memberName}, path={memberPath}]");
                        break;
                    }
                }

                if (memberInfos[^1].MemberType == MemberTypes.Method)
                {
                    Debug.LogError($"Methods are not supported by {nameof(ContextMemberPathBindingStrategy)}. [memberName={memberName}, path={memberPath}]");
                }
            }
        }

        private void EvaluateContext()
        {
            context = origin;

            for (int i = 0; i < memberInfos.Length - 1; i++)
            {
                if (context != null)
                {
                    context = GetMemberInfoValueSimple(context, memberInfos[i]);
                    memberValues[i] = context;
                }
            }
        }

        private void ReevaluateContext()
        {
            UnLinkFromEvents();

            EvaluateContext();
            
            LinkToEvents();
            
            GotDirty?.Invoke();
        }

        public object Get()
        {
            object result = null;

            if (context != null)
            {
                MemberInfo memberInfo = memberInfos[^1];
                    
                if (memberInfo is PropertyInfo propertyInfo)
                {
                    result = propertyInfo.GetValue(context, null);
                }
                else if (memberInfo is FieldInfo fieldInfo)
                {
                    result = fieldInfo.GetValue(context);
                }
            }
            
            return result;
        }

        public void Set(object value)
        {
            if (context != null)
            {
                MemberInfo memberInfo = memberInfos[^1];

                if (memberInfo is PropertyInfo propertyInfo)
                {
                    propertyInfo.SetValue(context, value);
                }
                else if (memberInfo is FieldInfo fieldInfo)
                {
                    fieldInfo.SetValue(context, value);
                }
            }
        }
        
        private object GetMemberInfoValueSimple(object instance, MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.GetValue(instance, null);
            }
            else if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(instance);
            }

            return null;
        }

        private void LinkToEvents()
        {
            if (origin is INotifyPropertyChanged originNotifyPropertyChanged)
            {
                originNotifyPropertyChanged.PropertyChanged += OnOriginPropertyChanged;
            }
            
            if (memberValues != null)
            {
                for (int i = 0; i < memberValues.Length; i++)
                {
                    object memberValue = memberValues[i];
                
                    if (memberValue is INotifyPropertyChanged memberValueNotifyPropertyChanged)
                    {
                        memberValueNotifyPropertyChanged.PropertyChanged += OnMemberValuePropertyChanged;
                    }
                }
            }

            if (value is INotifyCollectionChanged valueNotifyCollectionChanged)
            {
                valueNotifyCollectionChanged.CollectionChanged += OnValueCollectionChanged;
            }
            else if (value is INotifyPropertyChanged valueNotifyPropertyChanged)
            {
                valueNotifyPropertyChanged.PropertyChanged += OnValuePropertyChanged;
            }
            
            if (context is INotifyCollectionChanged contextNotifyCollectionChanged)
            {
                contextNotifyCollectionChanged.CollectionChanged += OnContextCollectionChanged;
            }
            else if (context is INotifyPropertyChanged contextNotifyPropertyChanged)
            {
                contextNotifyPropertyChanged.PropertyChanged += OnContextPropertyChanged;
            }
        }

        private void UnLinkFromEvents()
        {
            if (origin is INotifyPropertyChanged originNotifyPropertyChanged)
            {
                originNotifyPropertyChanged.PropertyChanged -= OnOriginPropertyChanged;
            }

            if (memberValues != null)
            {
                for (int i = 0; i < memberValues.Length; i++)
                {
                    object memberValue = memberValues[i];
                
                    if (memberValue is INotifyPropertyChanged memberValueNotifyPropertyChanged)
                    {
                        memberValueNotifyPropertyChanged.PropertyChanged -= OnMemberValuePropertyChanged;
                    }
                }
            }
            
            if (value is INotifyCollectionChanged valueNotifyCollectionChanged)
            {
                valueNotifyCollectionChanged.CollectionChanged -= OnValueCollectionChanged;
            }
            else if (value is INotifyPropertyChanged valueNotifyPropertyChanged)
            {
                valueNotifyPropertyChanged.PropertyChanged -= OnValuePropertyChanged;
            }
            
            if (context is INotifyCollectionChanged contextNotifyCollectionChanged)
            {
                contextNotifyCollectionChanged.CollectionChanged -= OnContextCollectionChanged;
            }
            else if (context is INotifyPropertyChanged contextNotifyPropertyChanged)
            {
                contextNotifyPropertyChanged.PropertyChanged -= OnContextPropertyChanged;
            }
        }

        private void OnOriginPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (memberInfos[0].Name == e.PropertyName)
            {
                ReevaluateContext();
            }
        }
        
        private void OnMemberValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int memberIndex = Array.IndexOf(memberValues, sender);

            if (memberInfos[memberIndex + 1].Name == e.PropertyName)
            {
                ReevaluateContext();
            }
        }
        
        private void OnValueCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GotDirty?.Invoke();
        }

        private void OnValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GotDirty?.Invoke();
        }
        
        private void OnContextCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GotDirty?.Invoke();
        }

        private void OnContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(memberName) || e.PropertyName == memberName)
            {
                GotDirty?.Invoke();
            }
        }
    }
}
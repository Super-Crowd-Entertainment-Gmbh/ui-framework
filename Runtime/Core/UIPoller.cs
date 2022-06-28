using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rehawk.UIFramework
{
    public class UIPoller : MonoBehaviour
    {
        private void Update()
        {
            inRefresh = true;
            
            foreach (IRefreshable refreshable in refreshables)
            {
                if (!Equals(refreshable, null) && !refreshable.IsDestroyed)
                {
                    refreshable.Refresh();
                }
            }
            
            refreshables.Clear();

            if (nextRefreshables.Count > 0)
            {
                foreach (IRefreshable refreshable in nextRefreshables)
                {
                    if (!Equals(refreshable, null) && !refreshable.IsDestroyed)
                    {
                        refreshables.Add(refreshable);
                    }
                }
            }
            
            nextRefreshables.Clear();
            
            inRefresh = false;
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        private static readonly HashSet<IRefreshable> refreshables = new HashSet<IRefreshable>();
        private static readonly HashSet<IRefreshable> nextRefreshables = new HashSet<IRefreshable>();
        
        private static bool inRefresh;
        private static bool isQuitting;

        private static UIPoller instance;
        
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			inRefresh = false;
            isQuitting = false;
            refreshables.Clear();
            nextRefreshables.Clear();
		}
	
        public static void Tag(IRefreshable refreshable)
        {
            if (isQuitting)
                return;

            CheckPollerInstance();
            
            if (inRefresh)
            {
                nextRefreshables.Add(refreshable);
            }
            else
            {
                refreshables.Add(refreshable);
            }
        }

        public static void Untag(IRefreshable refreshable)
        {
            if (isQuitting)
                return;

            CheckPollerInstance();
            
            refreshables.Remove(refreshable);
            nextRefreshables.Remove(refreshable);
        }

        public static Coroutine RunCoroutine(IEnumerator routine)
        {
            if (isQuitting)
                return default;

            CheckPollerInstance();

            return instance.StartCoroutine(routine);
        }

        private static void CheckPollerInstance()
        {
            if (instance == null)
            {
                var obj = new GameObject("[UIPoller]");
                instance = obj.AddComponent<UIPoller>();
                DontDestroyOnLoad(obj);
            }
        }
    }
    
    public interface IRefreshable
    {
        bool IsDestroyed { get; }
        
        void Refresh();
    }
}
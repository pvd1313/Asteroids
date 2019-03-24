using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Asteroids
{
    public static class EditorLazyAction
    {
        private static List<Action> actions = new List<Action>();

        static EditorLazyAction()
        {
            #if UNITY_EDITOR
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
            #endif
        }

        private static void Update()
        {
            foreach (var action in actions)
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }   
            actions.Clear();
        }

        public static void Add(Action action)
        {
            #if UNITY_EDITOR
            if (action != null) actions.Add(action);
            #endif
        }
    }
}
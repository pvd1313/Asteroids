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
        private static List<Action> Actions = new List<Action>();

        static EditorLazyAction()
        {
            #if UNITY_EDITOR
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
            #endif
        }

        private static void Update()
        {
            foreach (var action in Actions)
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
            Actions.Clear();
        }

        public static void Add(Action action)
        {
            if (action != null)
                Actions.Add(action);
        }
    }
}
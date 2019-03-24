using UnityEngine;
using System.Collections.Generic;

namespace Asteroids
{
    public static class MonoBehaviourExtensions
    {
        public static void SendMessageToFirstChildren(this MonoBehaviour behaviour, string methodName)
        {
            behaviour.transform.ForeachFirstChild(x => x.SendMessage(methodName));
        }

        public static void SendMessageToFirstChildren(this MonoBehaviour behaviour, string methodName, object value)
        { 
            behaviour.transform.ForeachFirstChild(x => x.SendMessage(methodName, value));
        }

        public static void SendMessageToFirstChildren(this MonoBehaviour behaviour, string methodName, SendMessageOptions options)
        {
            behaviour.transform.ForeachFirstChild(x => x.SendMessage(methodName, options));
        }

        public static void SendMessageToFirstChildren(this MonoBehaviour behaviour, string methodName, object value, SendMessageOptions options)
        {
            behaviour.transform.ForeachFirstChild(x => x.SendMessage(methodName, value, options));
        }

        public static void GetComponentsInFirstChildren<T>(this MonoBehaviour behaviour, bool includeInactive, List<T> results)
        {
            results.Clear(); T t;
            behaviour.transform.ForeachFirstChild(x => { t = x.GetComponent<T>(); if (t != null) results.Add(t); });
        }
    }
}
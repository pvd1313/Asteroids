using System.Collections.Generic;
using UnityEngine;
using System;

namespace Asteroids
{
    public static class TransformExtensions
    {
        private static readonly IExtension defaultNullWrapper = new NullWrapper();

        public interface IExtension
        {
            Vector3 Position { get; }
            Vector3 PlanarX { get; }
            Vector3 PlanarY { get; }
            Vector3 PlanarZ { get; }

            Vector3 TransformPoint(Vector3 position);
            Vector3 TransformDirection(Vector3 direction);
            Vector3 InverseTransformPoint(Vector3 position);
            Vector3 InverseTransformDirection(Vector3 direction);
            void TransformPoints(IList<Vector3> points, List<Vector3> result);
            Vector3 TransformPoint(PlanarVector position);
            T GetComponent<T>();
        }

        private struct Wrapper : IExtension
        {
            private readonly Transform _transform;

            public Vector3 Position { get { return _transform.position; } }
            public Vector3 PlanarX { get { return _transform.rotation * VectorConverter.planarX; } }
            public Vector3 PlanarY { get { return _transform.rotation * VectorConverter.planarY; } }
            public Vector3 PlanarZ { get { return _transform.rotation * VectorConverter.planarZ; } }

            public Wrapper(Transform transform) { _transform = transform; }
            public Vector3 TransformPoint(Vector3 position) { return _transform.TransformPoint(position); }
            public Vector3 TransformDirection(Vector3 direction) { return _transform.TransformDirection(direction); }
            public Vector3 InverseTransformPoint(Vector3 position) { return _transform.InverseTransformPoint(position); }
            public Vector3 InverseTransformDirection(Vector3 direction) { return _transform.InverseTransformDirection(direction); }
            public void TransformPoints(IList<Vector3> points, List<Vector3> result)
            {
                result.Clear();
                result.AddRange(points);
                for (var i = 0; i < result.Count; i++)
                    result[i] = _transform.TransformPoint(result[i]);
            }
            public Vector3 TransformPoint(PlanarVector position) { return _transform.TransformPoint(VectorConverter.Convert(position)); }
            public T GetComponent<T>() { return _transform.GetComponent<T>(); }
        }

        private struct NullWrapper : IExtension
        {
            public Vector3 Position { get { return Vector3.zero; } }
            public Vector3 PlanarX { get { return VectorConverter.planarX; } }
            public Vector3 PlanarY { get { return VectorConverter.planarY; } }
            public Vector3 PlanarZ { get { return VectorConverter.planarZ; } }

            public Vector3 TransformPoint(Vector3 position) { return position; }
            public Vector3 TransformDirection(Vector3 direction) { return direction; }
            public Vector3 InverseTransformPoint(Vector3 position) { return position; }
            public Vector3 InverseTransformDirection(Vector3 direction) { return direction; }
            public void TransformPoints(IList<Vector3> points, List<Vector3> result)
            {
                result.Clear();
                result.AddRange(points);
            }
            public Vector3 TransformPoint(PlanarVector position) { return VectorConverter.Convert(position); }
            public T GetComponent<T>() { return default(T); }
        }

        public static IExtension Wrap(this Transform transform)
        {
            return new Wrapper(transform);
        }

        public static IExtension WrapParent(this Transform transform)
        {
            var parent = transform.parent;
            if (parent == null) return defaultNullWrapper;
            return new Wrapper(parent);
        }

        public static void ForeachFirstChild(this Transform transform, Action<Transform> action)
        {
            var childCount = transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
                action.Invoke(transform.GetChild(i));
        }        
    }
}
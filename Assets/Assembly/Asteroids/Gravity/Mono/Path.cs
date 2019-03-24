using UnityEngine;
using System;
using System.Collections.ObjectModel;

namespace Asteroids.Gravity.Mono
{
    public struct Path
    {
        public readonly ReadOnlyCollection<Vector3> Curve;
        public readonly bool Enclosed;

        public Path(Vector3[] curve, bool enclosed)
        {
            Curve = Array.AsReadOnly(curve);
            Enclosed = enclosed;
        }
    }   
}
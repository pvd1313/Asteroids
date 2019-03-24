using UnityEngine;
using System;
using System.Collections.ObjectModel;

namespace Asteroids.Gravity.Mono
{
    public struct Path
    {
        public ReadOnlyCollection<Vector3> Curve { get; private set; }
        public bool Enclosed { get; private set; }

        public Path(Vector3[] curve, bool enclosed)
        {
            Curve = Array.AsReadOnly(curve);
            Enclosed = enclosed;
        }
    }   
}
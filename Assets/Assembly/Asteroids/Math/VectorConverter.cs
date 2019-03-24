using System;
using UnityEngine;
using System.Collections.Generic;

namespace Asteroids
{
    public static class VectorConverter
    {
        public static readonly Vector3 planarX = Convert(PlanarVector.xAxis);
        public static readonly Vector3 planarY = Convert(PlanarVector.yAxis);
        public static readonly Vector3 planarZ;

        static VectorConverter()
        {
            var vec = planarX + planarY;
            planarZ = new Vector3(vec.x == 0 ? 1 : 0, vec.y == 0 ? 1 : 0, vec.z == 0 ? 1 : 0);
        }

        public static void Convert(IList<PlanarVector> source, IList<Vector3> result)
        {
            if (source.Count != result.Count)
                throw new ArgumentException("Source count does not equal result count");

            for (var i = 0; i < source.Count; i++)
                result[i] = Convert(source[i]);
        }

        public static Vector3 Convert(PlanarVector source)
        {
            return new Vector3((float)source.x, 0, (float)source.y);
        }

        public static PlanarVector Convert(Vector3 source)
        {
            return new PlanarVector(source.x, source.z);
        }
    }
}
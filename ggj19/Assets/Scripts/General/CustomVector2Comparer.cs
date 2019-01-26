using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrutalHack.ggj19.General
{
    sealed class CustomVector2Comparer : IEqualityComparer<Vector2>
    {
        public bool Equals(Vector2 x, Vector2 y)
        {
            return Equals(x.x, y.x) && Equals(x.y, y.y);
        }

        public int GetHashCode(Vector2 obj)
        {
            return (Mathf.FloorToInt(obj.x) + Mathf.FloorToInt(obj.y)).GetHashCode();
        }

        private bool Equals(float x, float y)
        {
            return Math.Abs(x) - Math.Abs(y) < 0.001;
        }
    }
}
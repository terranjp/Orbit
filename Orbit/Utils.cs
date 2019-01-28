using Microsoft.Xna.Framework;
using System;

namespace Orbit
{
    internal static class Utils
    {
        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(x: vector.X, y: vector.Y);
        }

        // in Extensions
        public static float NextFloat(this Random rand, float minValue, float maxValue)
        {
            return (float)rand.NextDouble() * (maxValue - minValue) + minValue;
        }

        // in MathUtil
        public static Vector2 FromPolar(float angle, float magnitude)
        {
            return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float DiameterFromCircleArea(float area)
        {
            return (float)Math.Sqrt(4 * area / (float)Math.PI);
        }


    }
}
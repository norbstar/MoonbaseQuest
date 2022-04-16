using System;

using UnityEngine;

namespace Chess
{
    public class ChessMath
    {
        public static Vector3 RoundPosition(Vector3 localPosition)
        {
            return new Vector3(RoundFloat(localPosition.x), RoundFloat(localPosition.y), RoundFloat(localPosition.z));
        }

        public static float RoundFloat(float value)
        {
            return (float) Math.Round((value * 100f) / 100f, 2);
        }

        public static Vector3 RoundVector3(Vector3 value)
        {
            return new Vector3
            {
                x = (float) Math.Round((value.x * 100f) / 100f, 2),
                y = (float) Math.Round((value.y * 100f) / 100f, 2),
                z = (float) Math.Round((value.z * 100f) / 100f, 2)
            };
        }

        public static double Normalize(double value, double min, double max)
        {
            return (value - min) / (max - min);
        }
    }
}
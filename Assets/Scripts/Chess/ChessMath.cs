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
            return Mathf.Round(value * 100f) / 100f;
        }

        public static double Normalize(double value, double min, double max) {
            return (value - min) / (max - min);
        }
    }
}
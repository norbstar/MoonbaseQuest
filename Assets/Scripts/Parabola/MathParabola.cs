using System;

using UnityEngine;

namespace Parabola
{
    public class MathParabola
    {
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float timeframe)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
            var mid = Vector3.Lerp(start, end, timeframe);

            return new Vector3(mid.x, f(timeframe) + Mathf.Lerp(start.y, end.y, timeframe), mid.z);
        }

        public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float timeframe)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
            var mid = Vector2.Lerp(start, end, timeframe);

            return new Vector2(mid.x, f(timeframe) + Mathf.Lerp(start.y, end.y, timeframe));
        }
    }
}
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public class HelicopterStabiliseElevationCurveFunction : CurveFunction
    {
        public const float MIN_CLAMPED_VELOCITY = -1f;
        public const float MAX_CLAMPED_VELOCITY = 1f;

        public override float Get()
        {
            // Prep data
            var properties = ((IHelicopterCurveFunctionInput) input).GetProperties();
            var clampedVelocityY = Mathf.Clamp(properties.velocity.y, MIN_CLAMPED_VELOCITY, MAX_CLAMPED_VELOCITY);
            var normal = clampedVelocityY.Remap(MIN_CLAMPED_VELOCITY, MAX_CLAMPED_VELOCITY, -1f, 1f);

            // Interrogate curve
            var value = curve.Evaluate(normal);

            // Incorporate the return value when calculating the result
            return Mathf.Clamp(value, 0f, HelicopterController.CAPPED_ROTAR_SPEED);
        }
    }
}
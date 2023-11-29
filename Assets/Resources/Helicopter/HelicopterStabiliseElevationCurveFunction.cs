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
            var velocityY = ((IHelicopterCurveFunctionInput) input).GetVelocity().y;
            var clampedVelocityY = Mathf.Clamp(velocityY, MIN_CLAMPED_VELOCITY, MAX_CLAMPED_VELOCITY);
            var normalisedValue = clampedVelocityY.Remap(MIN_CLAMPED_VELOCITY, MAX_CLAMPED_VELOCITY, -1f, 1f);
            return Mathf.Clamp(curve.Evaluate(normalisedValue) * HelicopterController.MAX_ROTAR_SPEED, 0f, HelicopterController.CAPPED_ROTAR_SPEED);
        }
    }
}
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public class HelicopterStabiliseElevationCurveFunction : CurveFunction
    {
        public const float MIN_CLAMPED_VELOCITY = -10f;
        public const float MAX_CLAMPED_VELOCITY = 10f;

        public override float Get()
        {
            var yVelocity = ((IHelicopterCurveFunctionInput) input).GetVelocity().y;

            var clampedYVelocity = Mathf.Clamp(yVelocity, MIN_CLAMPED_VELOCITY, MAX_CLAMPED_VELOCITY);
            var normalisedValue = clampedYVelocity.Remap(MIN_CLAMPED_VELOCITY, MAX_CLAMPED_VELOCITY, -1f, 1f);
            return Mathf.Clamp(curve.Evaluate(normalisedValue), 0f, HelicopterController.CAPPED_ROTAR_SPEED);
        }
    }
}
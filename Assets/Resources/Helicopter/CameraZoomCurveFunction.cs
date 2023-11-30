using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public class CameraZoomCurveFunction : CurveFunction
    {
        public override float Get()
        {
            // Prep data
            var properties = ((IHelicopterCurveFunctionInput) input).GetProperties();
            var normalisedValue = properties.position.y.Remap(HelicopterController.MIN_ALTITUDE, HelicopterController.MAX_ALTITUDE, 0f, 1f);

            // Interrogate curve
            return curve.Evaluate(normalisedValue);
        }
    }
}
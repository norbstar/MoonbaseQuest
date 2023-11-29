using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public class CameraZoomCurveFunction : CurveFunction
    {
        public override float Get()
        {
            var positionY = ((IHelicopterCurveFunctionInput) input).GetPosition().y;
            var normalisedValue = positionY.Remap(HelicopterController.MIN_ALTITUDE, HelicopterController.MAX_ALTITUDE, 0f, 1f);
            return curve.Evaluate(normalisedValue);
        }
    }
}
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public class HelicopterEngagePowerCurveFunction : CurveFunction
    {
        public override float Get()
        {
            // Interrogate curve
            return curve.Evaluate(Time.time - startSec.Value) * HelicopterController.ROTAR_SPEED_LEVEL_THRESHOLD;
    }
}
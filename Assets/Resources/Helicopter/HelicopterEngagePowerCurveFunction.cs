using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public class HelicopterEngagePowerCurveFunction : CurveFunction
    {
        public override float Get()
        {
            var value = curve.Evaluate(Time.time - startSec.Value);
            return value * HelicopterController.ROTAR_SPEED_LEVEL_THRESHOLD;
        }
    }
}
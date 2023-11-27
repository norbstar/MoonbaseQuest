using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public class HelicopterStabiliseElevationCurveFunction : CurveFunction
    {       
        public override float Get()
        {
            // var elevation = ((IHelicopterCurveFunctionInput) input).GetElevation();
            // Debug.Log($"Elevation: {elevation}");

            var value = curve.Evaluate(Time.time - startSec.Value);
            return value;
        }
    }
}
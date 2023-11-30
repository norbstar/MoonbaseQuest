using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public class HelicopterCutPowerCurveFunction : CurveFunction
    {
        // public const float PERCENTAGE_DROPOFF_NORMAL = 0.1f;

        // private float altitude;

        // public void SetAltitude(float altitude) => this.altitude = altitude - HelicopterController.MIN_ALTITUDE;

        public override float Get()
        {
            // var properties = ((IHelicopterCurveFunctionInput) input).GetProperties();
            // var distance = properties.position.y - HelicopterController.MIN_ALTITUDE;
            // var normal = distance / altitude;
            // Debug.Log($"Altitude: {altitude} Distance: {distance} Normal: {normal}");
            // return curve.Evaluate(normal) * HelicopterController.ROTAR_SPEED_LEVEL_THRESHOLD - (HelicopterController.ROTAR_SPEED_LEVEL_THRESHOLD * PERCENTAGE_DROPOFF_NORMAL);

            // return HelicopterController.ROTAR_SPEED_LEVEL_THRESHOLD - (HelicopterController.ROTAR_SPEED_LEVEL_THRESHOLD * PERCENTAGE_DROPOFF_NORMAL);

            // Interrogate curve
            return curve.Evaluate(Time.time - startSec.Value);
        }
    }
}
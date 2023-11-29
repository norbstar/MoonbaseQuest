using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public class HelicopterCutPowerCurveFunction : CurveFunction
    {
        private float rotarSpeed;

        public void SetRotarSpeed(float rotarSpeed) => this.rotarSpeed = rotarSpeed;

        public override float Get()
        {
            var value = curve.Evaluate(Time.time - startSec.Value);
            return value * rotarSpeed;
        }
    }
}
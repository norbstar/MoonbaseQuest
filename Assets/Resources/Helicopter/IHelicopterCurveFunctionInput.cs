using UnityEngine;

namespace Helicopter
{
    public interface IHelicopterCurveFunctionInput : ICurveFunctionInput
    {
        float GetRotarSpeed();
        float GetElevation();
        Vector3 GetVelocity();
    }
}
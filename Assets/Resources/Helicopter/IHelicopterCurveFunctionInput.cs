using UnityEngine;

namespace Helicopter
{
    public interface IHelicopterCurveFunctionInput : ICurveFunctionInput
    {
        float GetRotarSpeed();
        float GetAltitude();
        Vector3 GetVelocity();
        Vector3 GetPosition();        
    }
}
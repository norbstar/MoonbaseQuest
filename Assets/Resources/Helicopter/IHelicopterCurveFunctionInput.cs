using UnityEngine;

namespace Helicopter
{
    public class HelicopterProperties
    {
        public float rotarSpeed;
        public float altitude;
        public Vector3 velocity;
        public Vector3 position;
    }
    
    public interface IHelicopterCurveFunctionInput : ICurveFunctionInput
    {
        HelicopterProperties GetProperties();
    }
}
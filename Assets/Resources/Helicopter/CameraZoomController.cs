using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    [RequireComponent(typeof(CameraZoomCurveFunction))]
    public class CameraZoomController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] HelicopterController helicopterController;

        private CameraZoomCurveFunction cameraZoomCurveFn;

        private void ResolveComponents()
        {
            cameraZoomCurveFn = GetComponent<CameraZoomCurveFunction>();
        }

        void Awake()
        {
            ResolveComponents();
            cameraZoomCurveFn.Init(helicopterController);
        }

        // Update is called once per frame
        void Update() => transform.localPosition = new Vector3(0f, 35f, -35f) * cameraZoomCurveFn.Get();
    }
}
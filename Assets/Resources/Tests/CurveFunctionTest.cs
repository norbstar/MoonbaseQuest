using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Tests
{
    [RequireComponent(typeof(CurveFunction))]
    public class CurveFunctionTest : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] float value;

        private CurveFunction curveFunction;

        void Awake() => curveFunction = GetComponent<CurveFunction>();

        void OnEnable()  => curveFunction.Exec();

        void OnDisable() => curveFunction.Cancel();

        // Update is called once per frame
        void Update() => value = curveFunction.Get();
    }
}
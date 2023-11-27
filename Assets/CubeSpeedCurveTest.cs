using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class CubeSpeedCurveTest : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] AnimationCurve curve;

    [Header("Bindings")]
    [SerializeField] InputAction action;

    [Header("Stats")]
    [SerializeField] float timelineSec = 0f;
    [SerializeField] float speed = 0f;

    private float startSec, elapsedTimeSec;
    // private float lastValue;

    // Start is called before the first frame update
    // void Start() => startSec = Time.time;

    void OnEnable() => action.Enable();

    void OnDisable() => action.Disable();

    // Update is called once per frame
    void Update()
    {
        // timelineSec = elapsedTimeSec += Time.deltaTime;
        // value = curve.Evaluate(elapsedTimeSec);

        // transform.position += -transform.right * value * Time.deltaTime;

        // Move the object forward along its x axis at -1 unit/second.
        // transform.Translate(-Vector3.right * value * Time.deltaTime);

        // Move the object forward in world space -1 unit/second.
        // transform.Translate(-Vector3.right * value * Time.deltaTime, Space.World);

        if (action.triggered)
        {
            // Debug.Log($"Triggered: {action.triggered}"); 
            startSec = Time.time;
            elapsedTimeSec = 0f;
        }

        var isPressed = action.IsPressed();
        // Debug.Log($"IsPressed: {isPressed}");

        if (isPressed)
        {
            var value = action.ReadValue<float>();

            // if (value != lastValue)
            // {
            //     startSec = Time.time;
            //     elapsedTimeSec = 0f;
            // }

            // lastValue = value;

            elapsedTimeSec += Time.deltaTime;
            speed = curve.Evaluate(elapsedTimeSec);
            transform.Translate(Vector3.forward * value * speed * Time.deltaTime);
        }
    }
}

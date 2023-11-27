using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CustomCubeBounceViaAnimationCurve : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] AnimationCurve curve;

    [Tooltip("Range")]
    [Range(0f, 1f)]
    [SerializeField] float range = 0f;

    [Tooltip("Timing")]
    [SerializeField] float durationSec = 2f;

#if false
    private float startSec;
#endif
    private float lastKeyTimeSec;
    private float elapsedTimeSec;

    // Start is called before the first frame update
    void Start()
    {
#if false
        startSec = Time.time;
#endif
        lastKeyTimeSec = curve.keys[curve.length - 1].time;
    }

    // Update is called once per frame
    void Update()
    {
#if false        
        var nowSec = Time.time;
        var diffSec = nowSec - startSec;

        var clampedDuration = Mathf.Clamp(diffSec, 0f, durationSec);
        // Debug.Log($"ClampedDuration: {clampedDuration}");
#endif
        elapsedTimeSec += Time.deltaTime;

        var clampedDuration = Mathf.Clamp(elapsedTimeSec, 0f, durationSec);
        // Debug.Log($"ClampedDuration: {clampedDuration}");

        range = clampedDuration.Remap(0f, durationSec, 0f, /*1f*/lastKeyTimeSec);
        // Debug.Log($"Range: {range}");

        var value = curve.Evaluate(range);

        // var xPos = CubeExtensionMethods.Remap(value, 0f, 1f, -5f, 5f);
        var xPos = value.Remap(0f, 1f, -5f, 5f);
        transform.position = new Vector3(xPos, 0f, 0f);
    }
}

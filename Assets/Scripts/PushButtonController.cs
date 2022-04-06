using System;

using UnityEngine;
using UnityEngine.Events;

public class PushButtonController : MonoBehaviour
{
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float deadZone = 0.025f;

    [Header("Events")]
    [SerializeField] UnityEvent onPressed;
    [SerializeField] UnityEvent onReleased;

    private bool isPressed;
    private Vector3 startPosition;
    private ConfigurableJoint joint;

    void Awake() => ResolveDependencies();

    private void ResolveDependencies()
    {
        joint = GetComponent<ConfigurableJoint>() as ConfigurableJoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"PushButtonController OnStart");
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        var value = GetValue();

        if (!isPressed && (value + threshold >= 1f))
        {
            OnPressed();
        }
        else if (isPressed && (value - threshold <= 0f))
        {
            OnReleased();
        }
    }

    private float GetValue()
    {
        var value = Vector3.Distance(startPosition, transform.localPosition) / joint.linearLimit.limit;

        if (Math.Abs(value) < deadZone)
        {
            value = 0;
        }

        return Mathf.Clamp(value, -1f, 1f);
    }
    
    private void OnPressed()
    {
        isPressed = true;
        onPressed.Invoke();
    }

    private void OnReleased()
    {
        isPressed = false;
        onReleased.Invoke();
    }
}
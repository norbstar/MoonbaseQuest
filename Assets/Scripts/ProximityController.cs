// using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ProximityController : MonoBehaviour
{
    [SerializeField] bool enableHaptics = false;
    
    private InputDevice leftDevice, rightDevice;
    private XRController[] controllers;
    private XRController leftController, rightController;
    private ProximityCanvas[] canvases;
    private ProximityCanvas leftDeviceCanvas, rightDeviceCanvas;
    private Light directionalLight;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();

        foreach (XRController controller in controllers)
        {
            if (controller.name.Equals("Left Hand Controller"))
            {
                leftController = controller;
                leftDevice = controller.inputDevice;
            }
            else if (controller.name.Equals("Right Hand Controller"))
            {
                rightController = controller;
                rightDevice = controller.inputDevice;
            }
        }

        foreach (ProximityCanvas canvas in canvases)
        {
            if (canvas.name.Equals("Left Hand Canvas"))
            {
                leftDeviceCanvas = canvas;
            }
            else if (canvas.name.Equals("Right Hand Canvas"))
            {
                rightDeviceCanvas = canvas;
            }
        }
    }

    private void ResolveDependencies()
    {
        controllers = FindObjectsOfType<XRController>();
        canvases = FindObjectsOfType<ProximityCanvas>();
        directionalLight = FindObjectOfType<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        float leftDeviceProximity = Vector3.Distance(leftController.gameObject.transform.position, transform.position);
        leftDeviceCanvas.SetProximity(leftDeviceProximity);

        if (enableHaptics)
        {
            SetImpulse(leftDevice, leftDeviceProximity);
        }

        float rightDeviceProximity = Vector3.Distance(rightController.gameObject.transform.position, transform.position);
        rightDeviceCanvas.SetProximity(rightDeviceProximity);

        if (enableHaptics)
        {
            SetImpulse(rightDevice, rightDeviceProximity);
        }
        
        SetLighting(leftDeviceProximity + rightDeviceProximity);
    }

    private void SetImpulse(InputDevice device, float proximity)
    {
        if (proximity > 1.0f) return;

        UnityEngine.XR.HapticCapabilities capabilities;

        if (device.TryGetHapticCapabilities(out capabilities))
        {
            if (capabilities.supportsImpulse)
            {
                uint channel = 0;
                float amplitude = (1f - proximity) * 5f;
                float duration = 0.1f;
                device.SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }

    private void SetLighting(float jointProximity)
    {
        directionalLight.intensity = (2.0f - jointProximity) * 5f;
    }
}
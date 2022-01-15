using UnityEngine;
using UnityEngine.XR;

public class SceneManager : MonoBehaviour
{
    [SerializeField] bool enableLogging = false;

    // private DebugCanvas debugCanvas;

    private static InputDeviceCharacteristics RightHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right);
    private static InputDeviceCharacteristics LeftHand = (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left);

    // Start is called before the first frame update
    // void Start()
    // {
    //     ResolveDependencies();
    // }

    // private void ResolveDependencies()
    // {
    //     var obj = FindObjectOfType<DebugCanvas>();
    //     debugCanvas = obj.GetComponent<DebugCanvas>() as DebugCanvas;
    // }

    void OnEnable()
    {
        InputDevices.deviceConnected += OnDeviceConnected;
        InputDevices.deviceDisconnected += OnDeviceDisconnected;
    }

    void OnDisable()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
        InputDevices.deviceDisconnected -= OnDeviceDisconnected;
    }

    private void OnDeviceConnected(InputDevice device)
    {
        Log($"{device.name}.Connected");

        if (((int) device.characteristics) == ((int) LeftHand))
        {
            Log($"{device.name}.Identified as Left Hand");
        }
        else if (((int) device.characteristics) == ((int) RightHand))
        {
            Log($"{device.name}.Identified as Right Hand");
        }
    }

    private void OnDeviceDisconnected(InputDevice device)
    {
        Log($"{device.name}.Disconnected");

        if (((int) device.characteristics) == ((int) LeftHand))
        {
            Log($"{device.name}.Identified as Left Hand");
        }
        else if (((int) device.characteristics) == ((int) RightHand))
        {
            Log($"{device.name}.Identified as Right Hand");
        }
    }

    private void Log(string message)
    {
        if (!enableLogging) return;
        Debug.Log(message);
    }
}
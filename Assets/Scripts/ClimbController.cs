using System.Reflection;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbController : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Config")]
    [SerializeField] DeviceBasedContinuousMoveProvider continuousMoveProvider;
    
    private CharacterController characterController;
    private XRController climbingHand;

    public XRController ClimbingHand
    {
        get
        {
            return climbingHand;
        }
        
        set
        {
            climbingHand = value;
            Log($"{Time.time} {gameObject.name} {className} ClimbingHand:{climbingHand}");
        }
    }

    protected void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        characterController = GetComponent<CharacterController>() as CharacterController;
    }

    void FixedUpdate()
    {
        continuousMoveProvider.enabled = !climbingHand;

        if (!climbingHand) return;
        Climb();
    }

    private void Climb()
    {
        InputDevices.GetDeviceAtXRNode(climbingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
        characterController.Move(transform.rotation * -velocity * Time.fixedDeltaTime);
    }
}
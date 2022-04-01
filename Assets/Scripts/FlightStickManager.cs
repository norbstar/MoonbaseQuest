using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlightStickManager : FocusableInteractableManager
{
    [Header("Device")]
    [SerializeField] private LocomotionProvider locomotionProvider;

    [Header("Config")]
    [SerializeField] GameObject stick;
    [SerializeField] HandAnimationController hand;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();

        hand?.SetFloat("Grip", 1f);
    }

    private void ResolveDependencies()
    {
        GameObject xrOrigin = GameObject.Find("XR Origin");

        if (xrOrigin == null) return;

        var driver = xrOrigin.GetComponent<CharacterControllerDriver>() as CharacterControllerDriver;
        locomotionProvider = driver?.locomotionProvider;
    }

    void FixedUpdate()
    {
        Log($"{gameObject.name}.FixedUpdate:Rotation : {transform.rotation.eulerAngles}");
    }
}
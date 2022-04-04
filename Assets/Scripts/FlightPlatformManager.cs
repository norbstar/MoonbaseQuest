using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(AudioSource))]
public class FlightPlatformManager  : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    
    [Header("Config")]
    [SerializeField] TurrentManager turret;
    [SerializeField] TextMeshProCanvasManager zStatsCanvas;
    [SerializeField] TextMeshProCanvasManager xStatsCanvas;

    private GameObject xrOrigin;
    private LocomotionProvider locomotionProvider;
    private DeviceBasedContinuousMoveProvider continuousMoveProvider;
    private DeviceBasedContinuousTurnProvider continuousTurnProvider;
    private AudioSource audioSource;
    private int interactorCount;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        xrOrigin = GameObject.Find("XR Origin");

        if (xrOrigin == null) return;

        if (xrOrigin.TryGetComponent<CharacterControllerDriver>(out CharacterControllerDriver driver))
        {
            locomotionProvider = driver.locomotionProvider;

            if (locomotionProvider.TryGetComponent<DeviceBasedContinuousMoveProvider>(out DeviceBasedContinuousMoveProvider moveProvider))
            {
                continuousMoveProvider = moveProvider;
            }

            if (locomotionProvider.TryGetComponent<DeviceBasedContinuousTurnProvider>(out DeviceBasedContinuousTurnProvider turnProvider))
            {
                continuousTurnProvider = turnProvider;
            }
        }

        audioSource = GetComponent<AudioSource>() as AudioSource;
    }

    void OnEnable()
    {
        StickInteractableManager.StickEventReceived += OnEvent;
        ZStickInteractableManager.MessageEventReceived += OnZMessageEvent;
        XStickInteractableManager.MessageEventReceived += OnXMessageEvent;
    }

    void OnDisable()
    {
        StickInteractableManager.StickEventReceived -= OnEvent;
        ZStickInteractableManager.MessageEventReceived -= OnZMessageEvent;
        XStickInteractableManager.MessageEventReceived -= OnXMessageEvent;
    }

    private void EnablePlatform(bool enabled)
    {
        EnableDeviceBasedProviders(!enabled);

        if (audioSource != null)
        {
            audioSource.volume = 0;
            audioSource.pitch = 1;
            audioSource.enabled = enabled;

            zStatsCanvas.Reset();
            xStatsCanvas.Reset();
        }
    }

    private void EnableDeviceBasedProviders(bool enabled)
    {
        if (continuousMoveProvider != null)
        {
            continuousMoveProvider.enabled = enabled;
        }

        if (continuousTurnProvider != null)
        {
            continuousTurnProvider.enabled = enabled;
        }
    }

    public void Rotate(NavId controllerId, float rotationForce, float normalized)
    {
        Log($"{gameObject.name} {className} Rotate:Nav ID : {controllerId} Rotational Force : {rotationForce} Normalized : {normalized}");
        
        if (controllerId == NavId.Left)
        {
            transform.Rotate(0f, rotationForce, 0f);

            if (audioSource != null)
            {
                audioSource.volume = normalized;
                audioSource.pitch = (1 + (0.25f * normalized));
            }
        }
        else if (controllerId == NavId.Right)
        {
            turret.Rotate(rotationForce, normalized);
        }
    }

    // public void OnTrigger(NavId controllerId)
    // {
    //     if (controllerId == NavId.Right)
    //     {
    //         turret.OnTrigger();
    //     }
    // }

    private void OnZMessageEvent(string message) => zStatsCanvas.Text = message;

    private void OnXMessageEvent(string message) => xStatsCanvas.Text = message;

    private void OnEvent(NavId controllerId, InteractableManager.EventType type)
    {
        Log($"{gameObject.name} {className} OnEvent:Type : {type}");

        switch (type)
        {
            case InteractableManager.EventType.OnSelectEntered:
                if (interactorCount == 0)
                {
                    EnablePlatform(true);
                    xrOrigin.transform.parent = transform;
                }

                if (controllerId == NavId.Right)
                {
                    turret.Activate();
                }

                ++interactorCount;
                break;

            case InteractableManager.EventType.OnActivated:
                if (controllerId == NavId.Right)
                {
                    turret.OnActivate();
                }
                break;

            case InteractableManager.EventType.OnDeactivated:
                if (controllerId == NavId.Right)
                {
                    turret.OnDeactivate();
                }
                break;

            case InteractableManager.EventType.OnSelectExited:
                --interactorCount;

                if (interactorCount == 0)
                {
                    xrOrigin.transform.parent = null;
                    EnablePlatform(false);
                }

                if (controllerId == NavId.Right)
                {
                    turret.Deactivate();
                }
                break;
        }
    }
}
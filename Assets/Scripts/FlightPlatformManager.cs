using System.Reflection;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(AudioSource))]
public class FlightPlatformManager  : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    
    [Header("Config")]
    [SerializeField] TextMeshProCanvasManager statsCanvas;

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
        XStickInteractableManager.MessageEventReceived += OnMessageEvent;
    }

    void OnDisable()
    {
        StickInteractableManager.StickEventReceived -= OnEvent;
        XStickInteractableManager.MessageEventReceived -= OnMessageEvent;
    }

    private void EnablePlatform(bool enabled)
    {
        EnableDeviceBasedProviders(!enabled);

        if (audioSource != null)
        {
            audioSource.volume = 0;
            audioSource.pitch = 1;
            audioSource.enabled = enabled;
            statsCanvas.Reset();
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

    public void Rotate(float rotationForce, float normalized)
    {
        Log($"{gameObject.name} {className} Rotate:Rotational Force : {rotationForce} Normalized : {normalized}");
        
        transform.Rotate(0f, rotationForce, 0f);

        if (audioSource != null)
        {
            audioSource.volume = normalized;
            audioSource.pitch = (1 + (0.25f * normalized));
        }
    }

    private void OnMessageEvent(string message)
    {
        statsCanvas.Text = message;
    }

    private void OnEvent(/*NavId controllerId, */InteractableManager.EventType type)
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

                ++interactorCount;
                break;

            case InteractableManager.EventType.OnSelectExited:
                --interactorCount;

                if (interactorCount == 0)
                {
                    xrOrigin.transform.parent = null;
                    EnablePlatform(false);
                }
                break;
        }
    }
}
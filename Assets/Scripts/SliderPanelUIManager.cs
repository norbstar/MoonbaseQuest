using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class SliderPanelUIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Slider slider;
    public float Value { get { return slider.value; } set { slider.value = value; } }
    [SerializeField] Toggle toggle;
    [SerializeField] PointerEventHandler eventHandler;

    [Header("Audio")]
    [SerializeField] protected AudioClip onHoverClip;

    [Header("Config")]
    [SerializeField] bool enableHaptics = false;
    public bool EnableHaptics { get { return enableHaptics; } }

    public delegate void OnModifyEvent(float value);
    public static event OnModifyEvent EventReceived;

    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(delegate {
            OnValueChanged(slider.value);
        });

        toggle.onValueChanged.AddListener(delegate {
            OnToggleChanged(toggle.isOn);
        });

        toggle.isOn = (slider.value == slider.minValue);
    }

    void OnEnable() => eventHandler.EventReceived += OnPointerEvent;

    void OnDisable() => eventHandler.EventReceived -= OnPointerEvent;

    private XRUIInputModule GetXRInputModule() => EventSystem.current.currentInputModule as XRUIInputModule;

    private bool TryGetXRRayInteractor(int pointerID, out XRRayInteractor rayInteractor)
    {
        var inputModule = GetXRInputModule();

        if (inputModule == null)
        {
            rayInteractor = null;
            return false;
        }

        rayInteractor = inputModule.GetInteractor(pointerID) as XRRayInteractor;
        return (rayInteractor != null);
    }

    private void OnPointerEvent(GameObject gameObject, PointerEventHandler.Event evt, PointerEventData eventData)
    {
        switch (evt)
        {
            case PointerEventHandler.Event.Enter:
                OnPointerEnter(eventData);
                break;

            case PointerEventHandler.Event.Exit:
                OnPointerExit(eventData);
                break;
        }
    }

    private void OnPointerEnter(PointerEventData eventData)
    {
        XRRayInteractor rayInteractor = null;
        
        if (TryGetXRRayInteractor(eventData.pointerId, out var interactor))
        {
            rayInteractor = interactor;
        }

        StartCoroutine(OnPointerEnterCoroutine(eventData, rayInteractor));
    }

    private IEnumerator OnPointerEnterCoroutine(PointerEventData eventData, XRRayInteractor rayInteractor)
    {
        var manager = slider.handleRect.gameObject.GetComponent<ScaleFXManager>() as ScaleFXManager;

        if (enableHaptics)
        {
            rayInteractor?.SendHapticImpulse(0.25f, 0.1f);
        }

        yield return manager.ScaleUp(1f, 1.1f);

        if (onHoverClip != null)
        {
            AudioSource.PlayClipAtPoint(onHoverClip, Vector3.zero, 1.0f);
        }

        yield return null;
    }

    private void OnPointerExit(PointerEventData eventData) => StartCoroutine(OnPointerExitCoroutine(eventData));

    private IEnumerator OnPointerExitCoroutine(PointerEventData eventData)
    {
        var manager = slider.handleRect.gameObject.GetComponent<ScaleFXManager>() as ScaleFXManager;
        yield return manager.ScaleDown(1.1f, 1f);
    }

    public void OnValueChanged(float value)
    {
        toggle.isOn = (value == slider.minValue);

        if (!toggle.isOn && !toggle.interactable)
        {
            toggle.interactable = true;
        }

        EventReceived?.Invoke(slider.value);
    }

    public void OnToggleChanged(bool isOn)
    {
        if (!isOn) return;

        toggle.interactable = false;
        slider.value = slider.minValue;
    }
}
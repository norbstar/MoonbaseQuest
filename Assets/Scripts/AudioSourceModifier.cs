using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceModifier : MonoBehaviour
{
    [SerializeField] SliderPanelUIManager sliderPanelUIManager;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();

        sliderPanelUIManager.Value = audioSource.volume;
    }

    private void ResolveDependencies() => audioSource = GetComponent<AudioSource>() as AudioSource;

    void OnEnable() => sliderPanelUIManager.EventReceived += OnModifyEvent;

    void OnDisable() => sliderPanelUIManager.EventReceived -= OnModifyEvent;

    private void OnModifyEvent(float value) => audioSource.volume = value;
}
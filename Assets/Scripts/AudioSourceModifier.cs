using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceModifier : MonoBehaviour
{
    [SerializeField] SliderPanelUIManager source;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();

        source.Value = audioSource.volume;
    }

    private void ResolveDependencies() => audioSource = GetComponent<AudioSource>() as AudioSource;

    void OnEnable() => SliderPanelUIManager.EventReceived += OnSliderEvent;

    void OnDisable() => SliderPanelUIManager.EventReceived -= OnSliderEvent;

    private void OnSliderEvent(float value) => audioSource.volume = value;
}
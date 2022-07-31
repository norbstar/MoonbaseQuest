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

    void OnEnable() => SliderPanelUIManager.EventReceived += OnModifyEvent;

    void OnDisable() => SliderPanelUIManager.EventReceived -= OnModifyEvent;

    private void OnModifyEvent(float value) => audioSource.volume = value;
}
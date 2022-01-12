using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public enum Event
    {
        OnTrigger
    }
    
    public delegate void OnTriggerEvent(Event evt);
    public OnTriggerEvent EventReceived;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        audioSource = GetComponent<AudioSource>() as AudioSource;
    }

    public void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;

        if (trigger.gameObject.CompareTag("Laser"))
        {
            // Debug.Log($"{gameObject.name}.{trigger.name} OnTriggerEnter");
            audioSource.Play();
            Invoke("PostTriggerEvent", 1f);
        }
    }

    private void PostTriggerEvent() => EventReceived(Event.OnTrigger);
}
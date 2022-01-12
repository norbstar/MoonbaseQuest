using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TestButton : MonoBehaviour
{
    private AudioSource audioSource;
    // private bool triggered;
    private int lastTriggerID;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        audioSource = GetComponent<AudioSource>() as AudioSource;
    }

    public void OnTriggerEnter(Collider collider) => HandleOnTrigger(collider.gameObject);

    public void OnTriggerStay(Collider collider) => HandleOnTrigger(collider.gameObject);

    private void HandleOnTrigger(GameObject trigger)
    {
        // if (triggered) return;

        Debug.Log($"{Time.time} {gameObject.name}.HandleTrigger:GameObject : {trigger.name}");

        if (trigger.gameObject.CompareTag("Laser") && trigger.GetInstanceID() != lastTriggerID)
        {
            lastTriggerID = trigger.GetInstanceID();
            Debug.Log($"{gameObject.name}.{trigger.name} HandleOnTrigger:ID : {lastTriggerID}");
            audioSource.Play();
        }

        // triggered = true;
    }
}
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AutomatedDoorManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioClip doorClip;

    private Animator animator;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        animator = GetComponent<Animator>() as Animator;
    }

    public void OnTriggerEnter(Collider collider)
    {
        var obj = collider.gameObject;

        if (obj.CompareTag("Player"))
        {
            animator.SetBool("character_nearby", true);
            AudioSource.PlayClipAtPoint(doorClip, transform.position, 1.0f);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        var obj = collider.gameObject;

        if (obj.CompareTag("Player"))
        {
            animator.SetBool("character_nearby", false);
            AudioSource.PlayClipAtPoint(doorClip, transform.position, 1.0f);
        }
    }
}
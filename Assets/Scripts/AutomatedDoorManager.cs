using System.Reflection;

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AutomatedDoorManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    
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
        Log($"{Time.time} {gameObject.name} {className} OnTriggerEnter:Collider : {collider.gameObject.name}");

        var trigger = collider.gameObject;

        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            if (rootGameObject.CompareTag("Player"))
            {
                animator.SetBool("character_nearby", true);
                AudioSource.PlayClipAtPoint(doorClip, transform.position, 1.0f);
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        Log($"{Time.time} {gameObject.name} {className} OnTriggerExit:Collider : {collider.gameObject.name}");

        var trigger = collider.gameObject;

        if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
        {
            if (rootGameObject.CompareTag("Player"))
            {
                animator.SetBool("character_nearby", false);
                AudioSource.PlayClipAtPoint(doorClip, transform.position, 1.0f);
            }
        }
    }
}
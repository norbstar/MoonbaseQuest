using System.Collections;
using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Animator))]
public class IncendiaryGrenadeInteractableManager : FocusableInteractableManager
{
    public enum State
    {
        Inactive,
        Activated,
        Deployed
    }

    [Header("Timings")]
    [SerializeField] float delay = 3f;

    [Header("Audio")]
    [SerializeField] AudioClip releaseClip;
    [SerializeField] AudioClip detinationClip;

    [Header("Config")]
    [SerializeField] float actuationRadius = 1.5f;
    [SerializeField] float actuationUPS = 50f;
    [SerializeField] float actuationForce = 250f;

    private Animator animator;
    private State state;

    protected override void Awake()
    {
        base.Awake();

        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        animator = GetComponent<Animator>() as Animator;
    }

    public void OnDeactivated(DeactivateEventArgs args)
    {
        if (state != State.Inactive) return;
        
        animator.SetTrigger("release");
        AudioSource.PlayClipAtPoint(releaseClip, transform.position, 1.0f);
        state = State.Activated;
    }

    protected override void OnSelectExited(SelectExitEventArgs args, HandController controller)
    {
        if (state == State.Activated)
        {
            StartCoroutine(ActuateCoroutine());
        }
    }

    private IEnumerator ActuateCoroutine()
    {
        yield return new WaitForSeconds(delay);

        AudioSource.PlayClipAtPoint(detinationClip, transform.position, 1.0f);

        Collider[] hits = Physics.OverlapSphere(transform.position, actuationRadius);

        foreach (var hit in hits)
        {
            StartCoroutine(ApplyImpactForceCoroutine(hit));
        }

        state = State.Deployed;
    }

    private IEnumerator ApplyImpactForceCoroutine(Collider hit)
    {
        var objectHit = hit.transform.gameObject;
        var point = hit.transform.position;

        Vector3 direction = point - transform.position;
        float distance = Vector3.Distance(transform.position, point);
        float duration = distance / actuationUPS;

        yield return new WaitForSeconds(duration);

        if (objectHit.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.AddForceAtPosition(direction.normalized * actuationForce, point);
        }
    }
}
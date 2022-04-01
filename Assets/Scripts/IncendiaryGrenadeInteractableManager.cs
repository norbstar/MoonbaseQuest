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
            // Force force = new Force();
            // StartCoroutine(ApplyImpactForceCoroutine(hit, force));

            StartCoroutine(ProcessImpactCoroutine(hit));
        }

        state = State.Deployed;
    }

    private IEnumerator ProcessImpactCoroutine(Collider hit)
    {
        CollisionData collisionData = new CollisionData();
        yield return StartCoroutine(ApplyImpactForceCoroutine(hit, collisionData));

        if ((collisionData.GameObject != null) && (collisionData.GameObject.TryGetComponent<IInteractableEvent>(out IInteractableEvent interactableEvent)))
        {
            interactableEvent.OnActivate(interactable, transform, collisionData.Point, collisionData.Force);
        }
    }

    private IEnumerator ApplyImpactForceCoroutine(Collider hit, CollisionData collisionData)
    {
        var objectHit = hit.gameObject;
        var point = hit.transform.position;

        Vector3 direction = point - transform.position;
        float distance = Vector3.Distance(transform.position, point);
        float duration = distance / actuationUPS;

        yield return new WaitForSeconds(duration);

        if (objectHit.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            float normForce = actuationForce - (distance / actuationRadius * actuationForce);
            Vector3 force = direction.normalized * normForce;
            rigidbody.AddForceAtPosition(force, point);

            collisionData.GameObject = objectHit;
            collisionData.Force = force;
            collisionData.Point = point;
        }
    }
}
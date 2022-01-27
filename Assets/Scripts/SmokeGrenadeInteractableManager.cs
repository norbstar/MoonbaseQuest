using System.Collections;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Animator))]
public class SmokeGrenadeInteractableManager : DockableFocusableInteractableManager
{
    public enum State
    {
        Inactive,
        Activated,
        Deployed
    }

    [Header("Timings")]
    [SerializeField] float smokeDelay = 3f;

    [Header("Signals")]
    [SerializeField] RendererColorSequencer band;

    [Header("Hits")]
    [SerializeField] bool showHits;
    [SerializeField] GameObject hitPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip releaseClip;
    [SerializeField] AudioClip detinationClip;

    [Header("Particles")]
    [SerializeField] new ParticleSystem particleSystem;

    private Animator animator;
    private State state;

    protected override void Awake()
    {
        base.Awake();
        ResolveDependencies();
        particleSystem.Stop();
    }

    private void ResolveDependencies()
    {
        animator = GetComponent<Animator>() as Animator;
    }

    public void OnActivated(ActivateEventArgs args)
    {
        if (state != State.Inactive) return;
        
        animator.SetTrigger("release");
        AudioSource.PlayClipAtPoint(releaseClip, transform.position, 1.0f);
        band.StartSequence();
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
        yield return new WaitForSeconds(smokeDelay);

        band.gameObject.SetActive(false);
        AudioSource.PlayClipAtPoint(detinationClip, transform.position, 1.0f);
        particleSystem.transform.parent = null;
        particleSystem.Play();

        state = State.Deployed;
    }
}
using System.Collections;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Animator))]
public class CustomSmokeGrenadeInteractableManager : FocusableInteractableManager
{
    public enum State
    {
        Inactive,
        Activated,
        Deployed
    }

    public enum Type
    {
        White,
        Red,
        Green,
        Violet,
        Yellow
    }

    [Header("Timings")]
    [SerializeField] float smokeDelay = 3f;

    [Header("Audio")]
    [SerializeField] AudioClip releaseClip;
    [SerializeField] AudioClip detinationClip;

    [Header("Particles")]
    [SerializeField] new ParticleSystem particleSystem;

    [Header("Materials")]
    [SerializeField] Material whiteMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] Material greenMaterial;
    [SerializeField] Material violetMaterial;
    [SerializeField] Material yellowMaterial;

    [Header("Config")]
    [SerializeField] GameObject smokeBody;
    [SerializeField] Type type;

    private Animator animator;
    private State state;

    protected override void Awake()
    {
        base.Awake();

        ResolveDependencies();
        particleSystem.Stop();

        Configure();
    }

    private void ResolveDependencies()
    {
        animator = GetComponent<Animator>() as Animator;
    }

    private void Configure()
    {
        ConfigureMaterial();
        ConfigureSmokeColor();
    }

    private void ConfigureMaterial()
    {
        var renderer = smokeBody.GetComponent<Renderer>() as Renderer;
        
        Material material = null;

        switch (type)
        {
            case Type.White:
                material = whiteMaterial;
                break;

            case Type.Red:
                material = redMaterial;
                break;

            case Type.Green:
                material = greenMaterial;
                break;

            case Type.Violet:
                material = violetMaterial;
                break;

            case Type.Yellow:
                material = yellowMaterial;
                break;
        }
        
        renderer.material = material;
    }

    private void ConfigureSmokeColor()
    {
        Color color = default(Color);

        switch (type)
        {
            case Type.White:
                ColorUtility.TryParseHtmlString("#ffffff80", out color);
                break;

            case Type.Red:
                ColorUtility.TryParseHtmlString("#d0372b80", out color);
                break;

            case Type.Green:
                ColorUtility.TryParseHtmlString("#419e0080", out color);
                break;

            case Type.Violet:
                ColorUtility.TryParseHtmlString("#c740d580", out color);
                break;

            case Type.Yellow:
                ColorUtility.TryParseHtmlString("#ffe10080", out color);
                break;
        }

        var main = particleSystem.main;
        main.startColor = color;
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
        yield return new WaitForSeconds(smokeDelay);

        AudioSource.PlayClipAtPoint(detinationClip, transform.position, 1.0f);
        particleSystem.transform.parent = null;
        particleSystem.Play();

        state = State.Deployed;
    }
}
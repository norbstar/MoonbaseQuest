using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Animator))]
public class RotateTargetManager : TargetManager, IDamage
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        animator = GetComponent<Animator>() as Animator;
    }

    public override void Activate(XRGrabInteractable interactable)
    {
        animator.SetTrigger("rotate");
    }

    public void ApplyDamage(float damage = 0)
    {
        // TODO
    }
}
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

    public override void Activate(XRGrabInteractable interactable, Vector3 hitPoint)
    {
        // Debug.Log($"{gameObject.name}.OnActivate:{interactable.name} {hitPoint}");

        animator.SetTrigger("rotate");
    }

    public void ApplyDamage(float damage = 0)
    {
        // TODO
    }
}
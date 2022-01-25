using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DroneTargetManager : TargetManager, IDamage
{
    // Start is called before the first frame update
    void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        // TODO
    }

    public override void Activate(XRGrabInteractable interactable, Vector3 hitPoint)
    {
        // Debug.Log($"{gameObject.name}.OnActivate:{interactable.name} {hitPoint}");

        Destroy(gameObject.transform.parent.gameObject);
    }

    public void ApplyDamage(float damage = 0)
    {
        // TODO
    }
}
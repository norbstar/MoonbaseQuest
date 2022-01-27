using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class MinimalInteractableManager : MonoBehaviour, IInteractable
{
    private Rigidbody rigidBody;

    private Transform objects;

    protected virtual void Awake()
    {
        ResolveDependencies();
        objects = GameObject.Find("Objects").transform;
    }

    private void ResolveDependencies()
    {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            this.rigidBody = rigidBody;
        }
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"{Time.time} {gameObject.name}.OnSelectEntered");
        Debug.Log($"{Time.time} {gameObject.name}:Is Kinemetic : {rigidBody.isKinematic} Use Gravity : {rigidBody.useGravity}");

        transform.parent = objects;
    }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log($"{Time.time} {gameObject.name}.OnSelectExited");
        Debug.Log($"{Time.time} {gameObject.name}:Is Kinemetic : {rigidBody.isKinematic} Use Gravity : {rigidBody.useGravity}");

        transform.parent = objects;
    }

    public void EnableTracking(bool enable)
    {
        Debug.Log($"{Time.time} {gameObject.name} EnableTracking:{enable}");
    }
}
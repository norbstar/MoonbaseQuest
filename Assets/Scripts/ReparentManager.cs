using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class ReparentManager : MonoBehaviour
{
    private Transform objects;

    void Awake()
    {
        objects = GameObject.Find("Objects").transform;
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        transform.parent = objects;
    }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        transform.parent = objects;
    }
}
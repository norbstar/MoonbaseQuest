using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleXRBaseInteractable : XRBaseInteractable
{
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        var interactor = args.interactorObject.transform.gameObject;
        Debug.Log($"OnHoverEntered:GameObject :{interactor.name}");
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        var interactor = args.interactorObject.transform.gameObject;
        Debug.Log($"OnHoverExited:GameObject :{interactor.name}");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        var interactor = args.interactorObject.transform.gameObject;
        Debug.Log($"OnSelectEntered:GameObject : {interactor.name}");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        var interactor = args.interactorObject.transform.gameObject;
        Debug.Log($"OnSelectExited:GameObject : {interactor.name}");
    }
}
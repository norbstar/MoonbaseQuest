using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbInteractable : XRBaseInteractable
{
    [Header("Materials")]
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material interactedMaterial;

    [Header("Config")]
    [SerializeField] ClimbController climbController;

    private new Renderer renderer;

    protected override void Awake()
    {
        base.Awake();
        
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        renderer = GetComponent<Renderer>() as Renderer;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        
        renderer.material = interactedMaterial;

        var interactorGameObject = args.interactorObject.transform.gameObject;

        if (TryGet.TryGetXRController(interactorGameObject, out XRController xrController))
        {
            climbController.ClimbingHand = xrController;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        renderer.material = defaultMaterial;

        var interactorGameObject = args.interactorObject.transform.gameObject;

        if (TryGet.TryGetXRController(interactorGameObject, out XRController xrController))
        {
            XRController climbingHand = climbController.ClimbingHand;

            if (climbingHand && climbingHand.name.Equals(xrController.name))
            {
                climbController.ClimbingHand = null;
            }
        }
    }
}
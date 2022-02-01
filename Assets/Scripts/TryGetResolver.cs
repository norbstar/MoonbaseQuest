using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;

public class TryGetResolver : MonoBehaviour
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    public bool TryGetInteractable<IInteractable>(GameObject trigger, out IInteractable interactable)
    {
        if (trigger.TryGetComponent<IInteractable>(out IInteractable interactableManager))
        {
            interactable = interactableManager;
            return true;
        }

        var component = trigger.GetComponentInParent<IInteractable>();

        if (component != null)
        {
            interactable = component;
            return true;
        }

        interactable = default(IInteractable);
        return false;
    }

    public bool TryGetControllers<HandController>(out List<HandController> controllers)
    {
        controllers = (FindObjectsOfType(typeof(HandController)) as HandController[]).ToList<HandController>();
        return (controllers.Count > 0);
    }

    public bool TryGetController<HandController>(GameObject interactor, out HandController controller)
    {
        if (interactor != null && interactor.CompareTag("Hand"))
        {
            if (interactor.TryGetComponent<HandController>(out HandController handController))
            {
                controller = handController;
                return true;
            }
        }

        controller = default(HandController);
        return false;
    }

    public bool TryGetOppositeController(HandController controller, out HandController opposingController)
    {
        opposingController = null;

        if (TryGetControllers<HandController>(out List<HandController> controllers))
        {
            var device = controller.GetInputDevice();

            if ((int) device.characteristics == (int) DockableGunInteractableManager.LeftHand)
            {
                var rightController = (HandController) controllers.First(hc => (int) hc.GetInputDevice().characteristics == (int) DockableGunInteractableManager.RightHand);
                opposingController = (rightController != null) ? rightController : null;
            }
            else if ((int) device.characteristics == (int) DockableGunInteractableManager.RightHand)
            {
                var leftController = (HandController) controllers.First(hc => (int) hc.GetInputDevice().characteristics == (int) DockableGunInteractableManager.LeftHand);
                opposingController = (leftController != null) ? leftController : null;
            }
        }
    
        return (opposingController != null);
    }
}
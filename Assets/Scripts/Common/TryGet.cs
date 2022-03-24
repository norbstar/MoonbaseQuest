using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

public class TryGet
{
    public static bool TryGetInteractable(GameObject trigger, out IInteractable interactable)
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

    public static bool TryGetControllers(out List<HandController> controllers)
    {
        controllers = (GameObject.FindObjectsOfType(typeof(HandController)) as HandController[]).ToList<HandController>();
        return (controllers.Count > 0);
    }

    public static bool TryGetIdentifyController(GameObject interactor, out HandController controller)
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

    public static bool TryGetControllerWithCharacteristics(InputDeviceCharacteristics characteristics, out HandController controller)
    {
        if (TryGetControllers(out List<HandController> controllers))
        {
            foreach (HandController thisController in controllers)
            {
                if (((int) characteristics == (int) HandController.LeftHand) || ((int) characteristics == (int) HandController.RightHand))
                {
                    controller = thisController;
                    return true;
                }
            }
        }

        controller = default(HandController);
        return false;
    }

    public static bool TryGetOpposingController(HandController controller, out HandController opposingController)
    {
        opposingController = null;

        if (TryGetControllers(out List<HandController> controllers))
        {
            var device = controller.InputDevice;

            if ((int) device.characteristics == (int) HandController.LeftHand)
            {
                var rightController = (HandController) controllers.FirstOrDefault(hc => (int) hc.InputDevice.characteristics == (int) HandController.RightHand);
                opposingController = (rightController != null) ? rightController : null;
            }
            else if ((int) device.characteristics == (int) HandController.RightHand)
            {
                var leftController = (HandController) controllers.FirstOrDefault(hc => (int) hc.InputDevice.characteristics == (int) HandController.LeftHand);
                opposingController = (leftController != null) ? leftController : null;
            }
        }
    
        return (opposingController != null);
    }

    public static bool TryGetRootResolver(GameObject gameObject, out GameObject rootGameObject)
    {
        if (gameObject.TryGetComponent<RootResolver>(out RootResolver rootResolver))
        {
            rootGameObject = rootResolver.Root;
            return true;
        }

        rootGameObject = default(GameObject);
        return false;
    }

    public static bool TryGetSocketInteractorManager(SocketCompatibilityLayerManager manager, out SocketInteractorManager socketInteractorManager)
    {
        socketInteractorManager = manager.GetComponentInChildren<SocketInteractorManager>() as SocketInteractorManager;
        return (socketInteractorManager != null);
    }
}
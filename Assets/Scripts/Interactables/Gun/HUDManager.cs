using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public abstract class HUDManager : BaseManager, IInputChange
    {
        public enum Identity
        {
            Home,
            Flashlight,
            LaserSight
        }

        [Header("Config")]
        [SerializeField] Identity id;
        [SerializeField] GunInteractableManager gunInteractableManager;

        protected GunInteractableManager GunInteractableManager { get { return gunInteractableManager; } }

        public Identity Id { get { return id; } }

        public abstract HUDCanvasManager GetCanvas();
        
        public abstract void ShowHUD();

        public abstract void HideHUD();
        
        public abstract void OnInputChange(Enum.ControllerEnums.Input input, InputDeviceCharacteristics characteristics, object value = null);
    }
}
using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public abstract class HUDManager : BaseManager, IActuation
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
        
        public abstract void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null);
    }
}
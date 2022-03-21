using UnityEngine;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public abstract class HUDManager : BaseManager, IActuation
    {
        public enum Identity
        {
            Primary,
            Flashlight,
            LaserSight
        }

        [Header("Config")]
        [SerializeField] Identity id;
        [SerializeField] GunInteractableManager gunInteractableManager;

        protected GunInteractableManager GunInteractableManager { get { return gunInteractableManager; } }

        void OnEnable()
        {
            ResolveDependencies();
        }

        private void ResolveDependencies()
        {
            gunInteractableManager = gameObject.transform.parent.parent.gameObject.GetComponent<GunInteractableManager>() as GunInteractableManager;
        }

        public Identity Id { get { return id; } }

        public abstract void ShowHUD();

        public abstract void HideHUD();

        public abstract void OnActuation(Actuation actuation, object value = null);
    }
}
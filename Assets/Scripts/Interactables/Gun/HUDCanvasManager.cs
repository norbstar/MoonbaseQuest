using System.Reflection;

using UnityEngine;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public abstract class HUDCanvasManager : BaseManager, IActuation
    {
        public enum Identity
        {
            Primary,
            Flashlight,
            LaserSight
        }

        [SerializeField] Identity id;

        public Identity Id { get { return id; } }

        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        public virtual void SetState(Enum.GunInteractableEnums.State state) { }

        public abstract void OnActuation(Actuation actuation, object value = null);
    }
}
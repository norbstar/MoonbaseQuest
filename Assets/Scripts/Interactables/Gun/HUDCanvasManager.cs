using System.Reflection;

using UnityEngine;

namespace Interactables.Gun
{
    public abstract class HUDCanvasManager : BaseManager
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
    }
}
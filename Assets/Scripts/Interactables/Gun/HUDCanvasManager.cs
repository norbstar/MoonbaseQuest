using System.Reflection;

using UnityEngine;

namespace Interactables.Gun
{
    public abstract class HUDCanvasManager : BaseManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        public virtual void SetState(Enum.GunInteractableEnums.State state) { }
    }
}
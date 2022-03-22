using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public class LaserSightHUDManager : HUDManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [SerializeField] LaserSightHUDCanvasManager canvasManager;
        
        public override HUDCanvasManager GetCanvas()
        {
            return canvasManager;
        }
        
        public override void ShowHUD()
        {
            canvasManager.gameObject.SetActive(false);
        }

        public override void HideHUD()
        {
            canvasManager.gameObject.SetActive(true);
        }

        public override void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null)
        {
            Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation}");
            
            // TODO
        }
    }
}
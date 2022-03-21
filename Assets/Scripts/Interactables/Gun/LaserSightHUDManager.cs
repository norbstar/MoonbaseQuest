using UnityEngine;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public class LaserSightHUDManager : HUDManager
    {
        [SerializeField] LaserSightHUDCanvasManager canvasManager;
        
        public override void ShowHUD()
        {
            canvasManager.gameObject.SetActive(false);
        }

        public override void HideHUD()
        {
            canvasManager.gameObject.SetActive(true);
        }

        public override void OnActuation(Actuation actuation, object value = null)
        {
            // TODO
        }
    }
}
using UnityEngine;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public class FlashlightHUDManager : HUDManager
    {
        [SerializeField] FlashlightHUDCanvasManager canvasManager;
        
        public override void ShowHUD()
        {
            canvasManager.gameObject.SetActive(true);
        }

        public override void HideHUD()
        {
            canvasManager.gameObject.SetActive(false);
        }

        public override void OnActuation(Actuation actuation, object value = null)
        {
            // TODO
        }
    }
}
using System.Reflection;

using UnityEngine;

using static Enum.GunInteractableEnums;
using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public class PrimaryHUDManager : HUDManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [SerializeField] PrimaryHUDCanvasManager canvasManager;

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
            Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation}");

            if (actuation.HasFlag(Actuation.Button_AX))
            {
                AlternateMode();
            }
        }

        private void AlternateMode()
        {
            Log($"{Time.time} {gameObject.name} {className} AlternateMode");

            var altMode = (GunInteractableManager.Mode == Enum.GunInteractableEnums.Mode.Manual) ? Enum.GunInteractableEnums.Mode.Auto : Enum.GunInteractableEnums.Mode.Manual;
            SetMode(altMode);
        }

        private void SetMode(Enum.GunInteractableEnums.Mode mode)
        {
            Log($"{Time.time} {gameObject.name} {className} SetMode: {mode}");

            switch (mode)
            {
                case Enum.GunInteractableEnums.Mode.Manual:
                    if (GunInteractableManager.Mode != Enum.GunInteractableEnums.Mode.Manual)
                    {
                        AudioSource.PlayClipAtPoint(GunInteractableManager.ManualClip, transform.position, 1.0f);
                    }
                    break;

                case Enum.GunInteractableEnums.Mode.Auto:
                    if (GunInteractableManager.Mode != Enum.GunInteractableEnums.Mode.Auto)
                    {
                        AudioSource.PlayClipAtPoint(GunInteractableManager.AutoClip, transform.position, 1.0f);
                    }
                    break;
            }
            
            canvasManager.SetMode(mode);
            GunInteractableManager.Mode = mode;
        }
    }
}
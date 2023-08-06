using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public class HomeHUDManager : HUDManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [SerializeField] HomeHUDCanvasManager canvasManager;
        [SerializeField] int defaultLoadout = 16;

        public int AmmoCount {
            get
            {
                return ammoCount;
            }
            
            set
            {
                canvasManager.AmmoTextUI.text = String.Format("{0:00}", value);
                canvasManager.AmmoTextUI.color = (value > 0) ? Color.white : Color.red;
            }
        }

        public bool HasAmmo { get { return ammoCount > 0; } }

        private int ammoCount;

        // Start is called before the first frame update
        void Start()
        {
            ammoCount = defaultLoadout;
            AmmoCount = ammoCount;
        }

        public void RestoreAmmoCount()
        {
            ammoCount = defaultLoadout;
            AmmoCount = ammoCount;
        }

        public void DecrementAmmoCount()
        {
            ammoCount -= 1;
            AmmoCount = ammoCount;
        }

        public override HUDCanvasManager GetCanvas()
        {
            return canvasManager;
        }

        public override void ShowHUD()
        {
            canvasManager.gameObject.SetActive(true);
        }

        public override void HideHUD()
        {
            canvasManager.gameObject.SetActive(false);
        }

        public override void OnInputChange(Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics, object value = null)
        {
            Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation}");

            if (actuation.HasFlag(Enum.ControllerEnums.Input.Button_AX))
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
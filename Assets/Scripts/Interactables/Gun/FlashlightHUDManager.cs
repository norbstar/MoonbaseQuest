using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    public class FlashlightHUDManager : HUDManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [SerializeField] FlashlightHUDCanvasManager canvasManager;
        
        private Enum.GunInteractableEnums.State state;

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

        public override void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics, object value = null)
        {
            Log($"{Time.time} {gameObject.name} {className} OnActuation:Actuation : {actuation}");

            if (actuation.HasFlag(Actuation.Button_AX))
            {
                AlternateIntent();
            }
        }

        private void AlternateIntent()
        {
            Log($"{Time.time} {gameObject.name} {className} AlternateIntent");

            var altState = (state == Enum.GunInteractableEnums.State.Active) ? Enum.GunInteractableEnums.State.Inactive : Enum.GunInteractableEnums.State.Active;
            SetState(altState);
        }

        public void SetStateNoAudio(Enum.GunInteractableEnums.State state)
        {
            SetState(state, false);
        }

        public void SetState(Enum.GunInteractableEnums.State state, bool audible = true)
        {
            Log($"{Time.time} {gameObject.name} {className} SetState: {state}");
            
            if (GunInteractableManager.TryGetCompatibleLayer("Compact Flashlight", out SocketCompatibilityLayerManager socketCompatibilityLayerManager))
            {
                if (TryGet.TryGetSocketInteractorManager(socketCompatibilityLayerManager, out SocketInteractorManager socketInteractorManager))
                {
                    if (!socketInteractorManager.IsOccupied) return;

                    var dockedObject = socketInteractorManager.Data.gameObject;

                    if (dockedObject.TryGetComponent<FlashlightInteractableManager>(out var manager))
                    {
                        switch (state)
                        {
                            case Enum.GunInteractableEnums.State.Active:
                                if (this.state != Enum.GunInteractableEnums.State.Active)
                                {
                                    manager.State = FlashlightInteractableManager.ActiveState.On;

                                    if (audible)
                                    {
                                        AudioSource.PlayClipAtPoint(GunInteractableManager.EngagedClip, transform.position, 1.0f);
                                    }
                                }
                                break;

                            case Enum.GunInteractableEnums.State.Inactive:
                                if (this.state != Enum.GunInteractableEnums.State.Inactive)
                                {
                                    manager.State = FlashlightInteractableManager.ActiveState.Off;

                                    if (audible)
                                    {
                                        AudioSource.PlayClipAtPoint(GunInteractableManager.DisengagedClip, transform.position, 1.0f);
                                    }
                                }
                                break;
                        }

                        canvasManager.SetState(state);
                        this.state = state;
                    }
                }
            }
        }
    }
}
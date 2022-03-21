using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Interactables.Gun
{
    [RequireComponent(typeof(Canvas))]
    public class FlashlightHUDCanvasManager : BaseManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        private static string ON = "ON";
        private static string OFF = "OFF";

        [SerializeField] TextMeshProUGUI textUI;
        [SerializeField] Image modeUI;

        [Header("Sprites")]
        [SerializeField] Sprite on;
        [SerializeField] Sprite off;

        private Enum.GunInteractableEnums.State state;

        public void SetState(Enum.GunInteractableEnums.State state)
        {
            Log($"{this.gameObject.name}.SetState:State : {state}");

            this.state = state;

            switch (state)
            {
                case Enum.GunInteractableEnums.State.Active:
                    modeUI.sprite = on;
                    textUI.text = ON;
                    break;

                case Enum.GunInteractableEnums.State.Inactive:
                    modeUI.sprite = off;
                    textUI.text = OFF;
                    break;
            }
        }
    }
}
using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using static Enum.ControllerEnums;

namespace Interactables.Gun
{
    [RequireComponent(typeof(Canvas))]
    public class PrimaryHUDCanvasManager : HUDCanvasManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [SerializeField] int defaultLoadout = 16;
        [SerializeField] TextMeshProUGUI ammoTextUI;
        [SerializeField] Image modeUI;

        [Header("Sprites")]
        [SerializeField] Sprite singleShot;
        [SerializeField] Sprite multiShot;
        
        public int AmmoCount {
        get
            {
                return ammoCount;
            }
            
            set
            {
                ammoTextUI.text = String.Format("{0:00}", value);
                ammoTextUI.color = (value > 0) ? Color.white : Color.red;
            }
        }

        private Enum.GunInteractableEnums.Mode mode;
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

        public override void OnActuation(Actuation actuation, object value = null)
        {
            // TODO
        }

        public void SetMode(Enum.GunInteractableEnums.Mode mode)
        {
            Log($"{this.gameObject.name}.SetMode:Mode : {mode}");

            this.mode = mode;

            switch (mode)
            {
                case Enum.GunInteractableEnums.Mode.Manual:
                    modeUI.sprite = singleShot;
                    break;

                case Enum.GunInteractableEnums.Mode.Auto:
                    modeUI.sprite = multiShot;
                    break;
            }
        }
    }
}
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

namespace Interactables.Gun
{
    public class HUDContainerManager : BaseManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("UI")]
        [SerializeField] List<Interactables.Gun.HUDManager> hudManagers;

        [SerializeField] HUDIndicatorManager hudIndicatorManager;

        private Interactables.Gun.HUDsManager hudsManager;

        public Interactables.Gun.HUDsManager HUDsManager { get { return hudsManager; } }

        public Interactables.Gun.HUDManager HUDManager
        {
            get
            {
                var activeIdx = hudsManager.ActiveIdx;
                return hudManagers[activeIdx];
            }
        }

        public void Awake()
        {
            hudsManager = new Interactables.Gun.HUDsManager(hudManagers);
        }

        public bool TryGetHUDManagerById(Interactables.Gun.HUDManager.Identity id, out Interactables.Gun.HUDManager hudManager)
        {
            Log($"{Time.time} {this.gameObject.name} {className}.TryGetHUDManagerById:Id : {id}");

            foreach (var thisHUDManager in hudManagers)
            {
                if (thisHUDManager.Id == id)
                {
                    hudManager = thisHUDManager;
                    return true;
                }
            }

            hudManager = null;
            return false;
        }

        public bool IsHUDShown(int id)
        {
            return (id == hudsManager.ActiveIdx);
        }

        public void ShowHomeHUD()
        {
            Log($"{Time.time} {this.gameObject.name}.ShowHomeHUD");

            ShowHUD((int) Interactables.Gun.HUDManager.Identity.Home);
        }

        public void ShowHUD(int id)
        {
            Log($"{Time.time} {this.gameObject.name}.ShowHUD:Id : {id}");

            Interactables.Gun.HUDManager hudManager;

            hudManager = hudManagers[hudsManager.ActiveIdx];
            hudManager.HideHUD();

            hudManager = hudManagers[id];
            hudManager.ShowHUD();

            hudsManager.ActiveIdx = id;
        }
    }
}
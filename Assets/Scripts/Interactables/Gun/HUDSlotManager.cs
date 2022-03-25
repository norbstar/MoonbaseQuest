using UnityEngine;
using UnityEngine.UI;

namespace Interactables.Gun
{
    public class HUDSlotManager : MonoBehaviour
    {
        private Image image;
        private HUDManager manager;
        private bool isActiveSlot;

        void Awake()
        {
            ResolveDependencies();
        }

        private void ResolveDependencies()
        {
            image = GetComponent<Image>() as Image;
        }
        
        public void Bind(HUDManager manager)
        {
            this.manager = manager;
            image.color = new Color(1f, 1f, 1f, 0.15f);
            gameObject.SetActive(true);
        }
        
        public bool IsBound { get { return manager != null; } }

        public bool IsActiveSlot { get { return isActiveSlot; } set { isActiveSlot = value; } }

        public void Reset()
        {
            this.manager = null;
            gameObject.SetActive(false);
        }
    }
}
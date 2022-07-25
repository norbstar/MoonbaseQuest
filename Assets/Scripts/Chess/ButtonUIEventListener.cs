using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(ButtonUIManager))]
    public class ButtonUIEventListener : MonoBehaviour
    {
        private ButtonUIManager buttonUIManager;

        // Start is called before the first frame update
        void Start() => ResolveDependencies();   

        private void ResolveDependencies() => buttonUIManager = GetComponent<ButtonUIManager>() as ButtonUIManager;
    }
}
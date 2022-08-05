using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(ButtonUI))]
    public class ButtonUIEventListener : MonoBehaviour
    {
        private ButtonUI buttonUI;

        // Start is called before the first frame update
        void Start() => ResolveDependencies();   

        private void ResolveDependencies() => buttonUI = GetComponent<ButtonUI>() as ButtonUI;
    }
}
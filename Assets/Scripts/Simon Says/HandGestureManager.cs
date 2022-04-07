using UnityEngine;

namespace SimonSays
{
    [RequireComponent(typeof(Animator))]
    public class HandGestureManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] new MeshRenderer renderer;

        private Animator animator;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies()
        {
            animator = GetComponent<Animator>() as Animator;
        }

        public void OnTriggerEnter(Collider collider)
        {
            var trigger =  collider.gameObject;
            
            if (trigger.CompareTag("Hand"))
            {
                renderer.enabled = false;
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            var trigger =  collider.gameObject;

            if (trigger.CompareTag("Hand"))
            {
                renderer.enabled = true;
            }
        }
    }
}
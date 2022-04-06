using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace SimonSays
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SimpleHandButtonManager : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] Material onMaterial;
        [SerializeField] Material offMaterial;

        [Header("Audio")]
        [SerializeField] AudioClip clickClip;

        [Header("Compatibility")]
        [SerializeField] List<XRDirectInteractor> interactables;

        [Header("Events")]
        [SerializeField] UnityEvent onPressed;
        [SerializeField] UnityEvent onReleased;

        private new MeshRenderer renderer;
        private IList<string> tags;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies()
        {
            renderer = GetComponent<MeshRenderer>() as MeshRenderer;
        }

        // Start is called before the first frame update
        void Start()
        {
            tags = new List<string>();

            foreach (XRDirectInteractor interactable in interactables)
            {
                if (!tags.Contains(interactable.tag))
                {
                    tags.Add(interactable.tag);
                }
            }
        }

        void OnEnable()
        {
            renderer.material = offMaterial;
        }

        public void OnTriggerEnter(Collider collider)
        {
            var trigger =  collider.gameObject;
            var match = tags.FirstOrDefault(t => t.Equals(trigger.tag));
            
            if (match == null) return;

            OnPressed();
            onPressed.Invoke();
        }

        public void OnSimulateOnPressed() => OnPressed();

        private void OnPressed()
        {
            AudioSource.PlayClipAtPoint(clickClip, transform.position, 1.0f);
            renderer.material = onMaterial;
        }

        public void OnTriggerExit(Collider collider)
        {
            var trigger =  collider.gameObject;
            var match = tags.FirstOrDefault(t => t.Equals(trigger.tag));
            
            if (match == null) return;

            OnReleased();
            onReleased.Invoke();
        }

        public void OnSimulateOnReleased() => OnReleased();

        private void OnReleased()
        {
            renderer.material = offMaterial;
        }
    }
}
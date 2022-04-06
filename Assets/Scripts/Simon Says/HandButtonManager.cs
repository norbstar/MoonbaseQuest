using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace SimonSays
{
    [RequireComponent(typeof(MeshRenderer))]
    public class HandButtonManager : MonoBehaviour
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
        private IList<int> instanceIds;
        private IList<string> tags;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies()
        {
            renderer = GetComponent<MeshRenderer>() as MeshRenderer;
        }

        // Start is called before the first frame update
        void Start()
        {
            renderer.material = offMaterial;
            instanceIds = new List<int>();
            tags = new List<string>();

            foreach (XRDirectInteractor interactable in interactables)
            {
                if (!tags.Contains(interactable.tag))
                {
                    tags.Add(interactable.tag);
                }
            }
        }

        public void OnTriggerEnter(Collider collider)
        {
            var trigger =  collider.gameObject;
            var match = tags.FirstOrDefault(t => t.Equals(trigger.tag));
            
            if (match == null) return;

            if (instanceIds.Count == 0)
            {
                AudioSource.PlayClipAtPoint(clickClip, transform.position, 1.0f);
                renderer.material = onMaterial;
                onPressed.Invoke();
            }

            instanceIds.Add(trigger.GetInstanceID());
        }

        public void OnTriggerExit(Collider collider)
        {
            var trigger =  collider.gameObject;
            var match = tags.FirstOrDefault(t => t.Equals(trigger.tag));
            
            if (match == null) return;

            instanceIds.Remove(trigger.GetInstanceID());

            if (instanceIds.Count == 0)
            {
                renderer.material = offMaterial;
                onReleased.Invoke();
            }
        }
    }
}
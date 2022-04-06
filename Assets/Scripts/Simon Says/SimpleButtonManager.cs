using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SimonSays
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SimpleButtonManager : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] Material onMaterial;
        [SerializeField] Material offMaterial;

        [Header("Audio")]
        [SerializeField] AudioClip clickClip;

        [Header("Compatibility")]
        [SerializeField] List<InteractableManager> interactables;

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

            foreach (InteractableManager interactable in interactables)
            {
                if (TryGet.TryGetRootResolver(interactable.gameObject, out GameObject rootGameObject))
                {
                    tags.Add(rootGameObject.tag);
                }
            }
        }

        public void OnTriggerEnter(Collider collider)
        {
            var trigger =  collider.gameObject;

            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                var match = tags.FirstOrDefault(t => t.Equals(rootGameObject.tag));
                
                if (match == null) return;

                if (instanceIds.Count == 0)
                {
                    AudioSource.PlayClipAtPoint(clickClip, transform.position, 1.0f);
                    renderer.material = onMaterial;
                }

                instanceIds.Add(trigger.GetInstanceID());
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            var trigger =  collider.gameObject;

            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                var match = tags.FirstOrDefault(t => t.Equals(rootGameObject.tag));
                
                if (match == null) return;

                instanceIds.Remove(trigger.GetInstanceID());

                if (instanceIds.Count == 0)
                {
                    renderer.material = offMaterial;
                }
            }
        }
    }
}
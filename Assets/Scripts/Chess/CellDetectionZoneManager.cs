using UnityEngine;

using static Enum.ControllerEnums;

namespace Chess
{
    [RequireComponent(typeof(Outline))]
    public class CellDetectionZoneManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] MeshFilter meshFilter;

        private Outline outline;
        private Color defaultOutlineColor;
        private int inZoneId;

        void Awake()
        {
            ResolveDependencies();
            defaultOutlineColor = outline.OutlineColor;
        }

        private void ResolveDependencies()
        {
            outline = GetComponent<Outline>() as Outline;
        }

        public void OnTriggerEnter(Collider collider)
        {
            var trigger = collider.gameObject;
            
            if (!collider.isTrigger) return;
            
            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                if (rootGameObject.CompareTag("Chess Piece"))
                {
                    // Debug.Log($"OnTriggerEnter : {rootGameObject.name}");
                    ShowOutline();

                    var meshFilter = rootGameObject.GetComponent<MeshFilter>() as MeshFilter;
                    SetMesh(meshFilter.mesh, Quaternion.identity);
                    ShowMesh();

                    var interactable = rootGameObject.GetComponent<PinchInteractable>() as PinchInteractable;

                    if (interactable.GetInstanceID() != inZoneId)
                    {
                        inZoneId = interactable.GetInstanceID();
                        interactable.EventReceived += OnInteractableEvent;
                        interactable.InZone = true;
                    }
                    
                    Debug.Log($"OnTriggerEnter Subscribed to events");
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            var trigger = collider.gameObject;

            if (!collider.isTrigger) return;

            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                if (rootGameObject.CompareTag("Chess Piece"))
                {
                    // Debug.Log($"OnTriggerExit : {rootGameObject.name}");
                    HideOutline();
                    HideMesh();

                    var interactable = rootGameObject.GetComponent<PinchInteractable>() as PinchInteractable;
                    interactable.EventReceived -= OnInteractableEvent;
                    interactable.InZone = false;

                    inZoneId = default(int);

                    Debug.Log($"OnTriggerExit Unsubscribed from events");
                }
            }
        }

        private void OnInteractableEvent(GameObject gameObject, Enum.ControllerEnums.Input actuation)
        {
            Debug.Log($"OnInteractableEvent : {gameObject.name} Actuation : {actuation}");

            if (!actuation.HasFlag(Enum.ControllerEnums.Input.Trigger))
            {
                DockInteractable(gameObject);
                HideOutline();
                HideMesh();
            }
        }

        private void DockInteractable(GameObject gameObject) => gameObject.transform.position = transform.parent.position;

        public void HideOutline() => outline.enabled = false;

        public void SetOutlineColor(Color color) => outline.OutlineColor = color;

        public void ApplyDefaultOutlineColor() => outline.OutlineColor = defaultOutlineColor;

        public void ShowOutline() => outline.enabled = true;

        public void SetMesh(Mesh mesh, Quaternion rotation, Material material = null)
        {
            transform.rotation = rotation;
            meshFilter.mesh = mesh;

            if (material != null)
            {
                meshFilter.GetComponent<MeshRenderer>().material = material;
            }
        }

        public void ShowMesh() => meshFilter.gameObject.SetActive(true);

        public void HideMesh() => meshFilter.gameObject.SetActive(false);
    }
}
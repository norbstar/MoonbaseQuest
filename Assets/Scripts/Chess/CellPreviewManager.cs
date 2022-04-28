using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(Outline))]
    public class CellPreviewManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] MeshFilter meshFilter;

        private Outline outline;
        private Color defaultOutlineColor;

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
            
            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                if (rootGameObject.CompareTag("Chess Piece"))
                {
                    Debug.Log($"OnTriggerEnter : {rootGameObject.name}");
                    ShowOutline();
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            var trigger = collider.gameObject;

            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                if (rootGameObject.CompareTag("Chess Piece"))
                {
                    Debug.Log($"OnTriggerExit : {rootGameObject.name}");
                    HideOutline();
                }
            }
        }

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
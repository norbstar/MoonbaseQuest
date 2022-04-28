using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(Outline))]
    public class PieceSocketManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] OnTriggerHandler onTriggerHandler;
        [SerializeField] MeshFilter meshFilter;

        private new MeshRenderer renderer;
        private Outline outline;
        private Color defaultOutlineColor;

        void Awake()
        {
            ResolveDependencies();
            defaultOutlineColor = outline.OutlineColor;
        }

        private void ResolveDependencies()
        {
            renderer = GetComponent<MeshRenderer>() as MeshRenderer;
            outline = GetComponent<Outline>() as Outline;
        }

        public void SetMesh(Mesh mesh, Quaternion rotation, Material material = null)
        {
            transform.rotation = rotation;
            meshFilter.mesh = mesh;

            if (material != null)
            {
                meshFilter.GetComponent<MeshRenderer>().material = material;
            }

            meshFilter.gameObject.SetActive(true);
        }

        public void HideMesh() => meshFilter.gameObject.SetActive(false);

        public void SetOutlineColor(Color color) => outline.OutlineColor = color;

        public void ApplyDefaultOutlineColor() => outline.OutlineColor = defaultOutlineColor;

        public void HideOutline() => outline.enabled = false;

        public void ShowOutline() => outline.enabled = true;

        public void OnTriggerEnter(Collider collider)
        {
            var trigger = collider.gameObject;
            
            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                if (rootGameObject.CompareTag("Hand"))
                {
                    Debug.Log("OnTriggerEnter : {rootGameObject.name}");
                    ShowOutline();
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            var trigger = collider.gameObject;

            if (TryGet.TryGetRootResolver(trigger, out GameObject rootGameObject))
            {
                if (rootGameObject.CompareTag("Hand"))
                {
                    Debug.Log("OnTriggerExit : {rootGameObject.name}");
                    HideOutline();
                }
            }
        }
    }
}
using UnityEngine;

namespace Chess.Preview
{
    [RequireComponent(typeof(Outline))]
    public class PreviewManager : MonoBehaviour, IFocus
    {
        [Header("Components")]
        [SerializeField] MeshFilter customMeshFilter;
        [SerializeField] MeshFilter fixedMeshFilter;
        // [SerializeField] GameObject skull;

        public delegate void Event(PreviewManager manager, FocusType focusType);
        public static event Event EventReceived;

        public Cell Cell { get { return cell; } }

        private new MeshRenderer renderer;
        private Cell cell;
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

        public void PlaceAtCell(Cell cell)
        {
            this.cell = cell;
            transform.localPosition = new Vector3(cell.localPosition.x, transform.localPosition.y, cell.localPosition.z);
        }

        public void SetCustomMesh(Mesh mesh, Quaternion rotation, Material material = null)
        {
            transform.rotation = rotation;
            customMeshFilter.mesh = mesh;

            if (material != null)
            {
                customMeshFilter.GetComponent<MeshRenderer>().material = material;
            }

            customMeshFilter.gameObject.SetActive(true);
        }

        public void ShowFixedMesh(Quaternion rotation, Material material = null)
        {
            transform.rotation = rotation;

            if (material != null)
            {
                fixedMeshFilter.GetComponent<MeshRenderer>().material = material;
            }

            fixedMeshFilter.gameObject.SetActive(true);
        }

        // public void ShowSkull() => skull.SetActive(true);

        // public void HideSkull() => skull.SetActive(false);

        public void HideMesh()
        {
            customMeshFilter.gameObject.SetActive(false);
            fixedMeshFilter.gameObject.SetActive(false);
        }

        public void SetOutlineColor(Color color) => outline.OutlineColor = color;

        public void ApplyDefaultOutlineColor() => outline.OutlineColor = defaultOutlineColor;
        

        public void HideOutline() => outline.enabled = false;

        public void ShowOutline() => outline.enabled = true;

        public void GainedFocus(GameObject gameObject, Vector3 focalPoint) => EventReceived?.Invoke(this, FocusType.OnFocusGained);

        public void LostFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusLost);
    }
}
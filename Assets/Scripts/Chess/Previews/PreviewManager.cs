using UnityEngine;

namespace Chess.Preview
{
    public class PreviewManager : MonoBehaviour, IFocus
    {
        [Header("Components")]
        [SerializeField] MeshFilter customMeshFilter;
        [SerializeField] MeshFilter fixedMeshFilter;

        public delegate void Event(PreviewManager manager, FocusType focusType);
        public static event Event EventReceived;

        public Cell Cell { get { return cell; } }

        private new MeshRenderer renderer;
        private Cell cell;

        void Awake()
        {
            ResolveDependencies();
        }

        private void ResolveDependencies()
        {
            renderer = GetComponent<MeshRenderer>() as MeshRenderer;
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

        public void HideMesh()
        {
            customMeshFilter.gameObject.SetActive(false);
            fixedMeshFilter.gameObject.SetActive(false);
        }

        public void GainedFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusGained);

        public void LostFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusLost);
    }
}
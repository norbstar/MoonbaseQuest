using UnityEngine;

namespace Chess.Preview
{
    public class PlacementPreviewManager : MonoBehaviour, IFocus
    {
        [Header("Components")]
        [SerializeField] MeshFilter preview;

        public delegate void Event(PlacementPreviewManager manager, FocusType focusType);
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
            transform.localPosition = cell.localPosition;
        }

        public void SetMesh(Mesh mesh, Quaternion rotation)
        {
            transform.rotation = rotation;
            preview.mesh = mesh;
        }

        public void GainedFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusGained);

        public void LostFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusLost);
    }
}
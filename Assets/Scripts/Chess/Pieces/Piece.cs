using UnityEngine;

namespace Chess.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        [SerializeField] PieceType type;
        public PieceType Type { get { return type; } }
        
        public Cell Home { get { return homeCell; } set { homeCell = value; } }

        public delegate void HomeEvent();
        public event HomeEvent HomeEventReceived;
        
        private new Rigidbody rigidbody;
        private new Collider collider;
        private Quaternion localRotation;
        private Cell homeCell;
        
        void Awake()
        {
            ResolveDependencies();

            localRotation = transform.localRotation;
        }

        private void ResolveDependencies()
        {
            rigidbody = GetComponent<Rigidbody>() as Rigidbody;
            collider = GetComponent<Collider>() as Collider;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void ResetPhysics()
        {
            rigidbody.isKinematic = false;
            collider.enabled = true;
        }

        public void GoHome()
        {
            rigidbody.isKinematic = true;
            collider.enabled = false;

            transform.localRotation = localRotation;
            transform.localPosition = homeCell.localPosition;

            // TODO animation sequence

            HomeEventReceived?.Invoke();
        }
    }
}
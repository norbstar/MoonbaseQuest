using System.Collections;

using UnityEngine;

namespace Chess.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        [SerializeField] PieceType type;
        public PieceType Type { get { return type; } }
        
        public Cell HomeCell { get { return homeCell; } set { activeCell = homeCell = value; } }
        public Cell ActiveCell { get { return activeCell; } set { activeCell = value; } }

        public delegate void HomeEvent();
        public event HomeEvent HomeEventReceived;
        
        private new Rigidbody rigidbody;
        private new Collider collider;
        private Quaternion originalRotation;
        private Cell homeCell;
        private Cell activeCell;
        
        void Awake()
        {
            ResolveDependencies();

            originalRotation = transform.localRotation;
        }

        private void ResolveDependencies()
        {
            rigidbody = GetComponent<Rigidbody>() as Rigidbody;
            collider = GetComponent<Collider>() as Collider;
        }

        // Start is called before the first frame update
        void Start()
        {
            // TODO
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void ReinstatePhysics() => EnablePhysics(true);

        private void EnablePhysics(bool enabled)
        {
            rigidbody.isKinematic = !enabled;
            collider.enabled = enabled;
        }

        public void SnapToActiveCell()
        {
            EnablePhysics(false);

            transform.localRotation = originalRotation;
            transform.localPosition = activeCell.localPosition;

            EnablePhysics(true);
        }

        public void GoHome(float rotationSpeed, float movementSpeed) => StartCoroutine(GoHomeCoroutine(rotationSpeed, movementSpeed));

        private IEnumerator GoHomeCoroutine(float rotationSpeed, float movementSpeed)
        {
            EnablePhysics(false);

            while ((transform.localRotation != originalRotation) || (transform.localPosition != homeCell.localPosition))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalRotation, rotationSpeed * Time.deltaTime);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, homeCell.localPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }

            HomeEventReceived?.Invoke();
        }
    }
}
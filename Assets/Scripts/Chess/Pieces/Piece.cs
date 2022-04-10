using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Chess.Pieces
{
    public abstract class Piece : MonoBehaviour, IFocus
    {
        [SerializeField] PieceType type;
        public PieceType Type { get { return type; } }

        [SerializeField] Set set;
        public Set Set { get { return set; } }

        public Cell HomeCell { get { return homeCell; } set { activeCell = homeCell = value; } }
        public Cell ActiveCell { get { return activeCell; } set { activeCell = value; } }

        public delegate void HomeEvent(Piece piece);
        public event HomeEvent HomeEventReceived;

        public delegate void Event(Piece piece, FocusType focusType);
        public event Event EventReceived;
        
        private new MeshRenderer renderer;
        private new Rigidbody rigidbody;
        private new Collider collider;
        private Color defaultMaterialColor;
        private Quaternion originalRotation;
        private Cell homeCell;
        private Cell activeCell;
        
        void Awake()
        {
            ResolveDependencies();

            originalRotation = transform.localRotation;
            defaultMaterialColor = renderer.material.color;
        }

        private void ResolveDependencies()
        {
            renderer = GetComponent<MeshRenderer>() as MeshRenderer;
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

        public void GainedFocus(GameObject gameObject)
        {
            EventReceived?.Invoke(this, FocusType.OnFocusGained);
        }

        public void LostFocus(GameObject gameObject)
        {
            EventReceived?.Invoke(this, FocusType.OnFocusLost);
        }

        public void ApplyHighlight() => renderer.material.color = Color.yellow;

        public void ApplyDefault() => renderer.material.color = defaultMaterialColor;

        public void MarkAvailable() => renderer.material.color = Color.green;

        public void MarkUnavailable() => renderer.material.color = Color.red;

        public abstract List<Cell> CalculateMoves();

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

            HomeEventReceived?.Invoke(this);
        }
    }
}
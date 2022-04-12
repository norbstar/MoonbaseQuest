using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Chess.Pieces
{
    public abstract class PieceManager : MonoBehaviour, IFocus
    {
        [Header("Config")]
        [SerializeField] protected PieceType type;
        public PieceType Type { get { return type; } }

        [SerializeField] protected Set set;
        public Set Set { get { return set; } }

        public Cell HomeCell { get { return homeCell; } set { activeCell = homeCell = value; } }
        public Cell ActiveCell { get { return activeCell; } set { activeCell = value; } }

        public delegate void MoveEvent(PieceManager piece);
        public event MoveEvent MoveEventReceived;

        public delegate void Event(PieceManager piece, FocusType focusType);
        public event Event EventReceived;
        
        protected int maxColumnIdx, maxRowIdx;

        private MeshFilter filter;
        private new MeshRenderer renderer;
        private new Rigidbody rigidbody;
        private new Collider collider;
        private Material defaultMaterial;
        private Color defaultMaterialColor;
        private Quaternion originalRotation;
        private Cell homeCell;
        private Cell activeCell;
        
        void Awake()
        {
            ResolveDependencies();

            maxColumnIdx = ChessBoardManager.MatrixColumns - 1;
            maxRowIdx = ChessBoardManager.MatrixRows - 1;
            
            originalRotation = transform.localRotation;
            defaultMaterial = renderer.material;
            defaultMaterialColor = renderer.material.color;
        }

        private void ResolveDependencies()
        {
            filter = GetComponent<MeshFilter>() as MeshFilter;
            renderer = GetComponent<MeshRenderer>() as MeshRenderer;
            rigidbody = GetComponent<Rigidbody>() as Rigidbody;
            collider = GetComponent<Collider>() as Collider;
        }

        public Mesh Mesh { get { return filter.mesh; } }

        public void GainedFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusGained);

        public void LostFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusLost);

        public void ApplyDefaultTheme() => renderer.material.color = defaultMaterialColor;

        public void ApplyHighlightTheme() => renderer.material.color = Color.yellow;

        public void ApplySelectedTheme() => renderer.material.color = Color.green;

        public void ApplyMaterial(Material material) => renderer.material = material;

        public void UseDefaultMaterial() => ApplyMaterial(defaultMaterial);

        protected bool TryGetPotentialCoord(Coord coord, int x, int y, out Coord potentialCoord)
        {
            if ((x >= 0 && x <= maxColumnIdx) && (y >= 0 && y <= maxRowIdx))
            {
                potentialCoord = new Coord
                {
                    x = x,
                    y = y
                };

                return true;
            }

            potentialCoord = default(Coord);
            return false;
        }

        protected Coord GetCoordByOffset(Coord coord, int xOffset, int yOffset)
        {
            int offsetX = coord.x + xOffset;
            int offsetY = coord.y + yOffset;

            return new Coord
            {
                x = offsetX,
                y = offsetY
            };
        }

        // protected bool TryGetPotentialCoordByOffset(Coord coord, int xOffset, int yOffset, out Coord potentialCoord)
        // {
        //     int offsetX = coord.x + xOffset;
        //     int offsetY = coord.y + yOffset;

        //     if ((offsetX >= 0 && offsetX <= maxColumnIdx) && (offsetY >= 0 && offsetY <= maxRowIdx))
        //     {
        //         potentialCoord = new Coord
        //         {
        //             x = offsetX,
        //             y = offsetY
        //         };

        //         return true;
        //     }

        //     potentialCoord = default(Coord);
        //     return false;
        // }

        protected bool TryGetPotentialCoords(Coord coord, List<Coord> coords, out List<Coord> potentialCoords)
        {
            potentialCoords = new List<Coord>();

            foreach (Coord thisCoord in coords)
            {
                if (TryGetPotentialCoord(coord, thisCoord.x, thisCoord.y, out Coord offsetCoord))
                {
                    potentialCoords.Add(offsetCoord);
                }
            }

            return (potentialCoords.Count > 0);
        }

        protected bool TryGetPotentialCoordsByOffset(Coord coord, List<Coord> offsets, out List<Coord> potentialCoords)
        {
            potentialCoords = new List<Coord>();

            foreach (Coord offset in offsets)
            {
                Coord trueCoord = GetCoordByOffset(coord, offset.x, offset.y);

                // if (TryGetPotentialCoordByOffset(coord, offset.x, offset.y, out Coord offsetCoord))
                // {
                //     potentialCoords.Add(offsetCoord);
                // }

                if (TryGetPotentialCoord(coord, trueCoord.x, trueCoord.y, out Coord offsetCoord))
                {
                    potentialCoords.Add(offsetCoord);
                }
            }

            return (potentialCoords.Count > 0);
        }

        public abstract List<Cell> CalculateMoves(ChessBoardManager manager, Cell[,] matrix, int vector);

        public virtual void Reset()
        {
            UseDefaultMaterial();
            ApplyDefaultTheme();
        }

        public void EnablePhysics(bool enabled)
        {
            rigidbody.isKinematic = !enabled;
            collider.enabled = enabled;
        }

        public void ReinstatePhysics() => EnablePhysics(true);

        public void SnapToActiveCell()
        {
            EnablePhysics(false);

            transform.localRotation = originalRotation;
            transform.localPosition = activeCell.localPosition;

            EnablePhysics(true);
        }

        public void GoHome(float rotationSpeed, float movementSpeed) => StartCoroutine(GoToCellCoroutine(homeCell, rotationSpeed, movementSpeed));

        public void GoToCell(Cell cell, float rotationSpeed, float movementSpeed) => StartCoroutine(GoToCellCoroutine(cell, rotationSpeed, movementSpeed));

        private IEnumerator GoToCellCoroutine(Cell cell, float rotationSpeed, float movementSpeed)
        {
            EnablePhysics(false);

            while ((transform.localRotation != originalRotation) || (transform.localPosition != cell.localPosition))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalRotation, rotationSpeed * Time.deltaTime);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, cell.localPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }

            if (activeCell != null)
            {
                activeCell.piece = null;
            }

            activeCell = cell;
            MoveEventReceived?.Invoke(this);
        }
    }
}
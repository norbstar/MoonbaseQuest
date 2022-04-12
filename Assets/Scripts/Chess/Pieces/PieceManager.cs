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

        public virtual List<Cell> CalculateMoves(ChessBoardManager manager, Cell[,] matrix, int vector)
        {
            List<Cell> moves = new List<Cell>();
            List<Cell> potentialMoves = ResolvePotentialCells(manager, matrix, manager.PlayMode);

            foreach (Cell potentialMove in potentialMoves)
            {
                // TODO determine if the potential move would put your own King in check.
                // If so, the move is NOT legel, otherwise add it to the list of moves

                // TMP measure to test the base functionality of the piece
                moves.Add(potentialMove);
            }

            // if (manager.TryGetSetPiecesByType(set, PieceType.King, out List<Cell> cells))
            // {
            //     if (cells.Count > 0)
            //     {
            //         var kingCell = cells[0];
            //         Debug.Log("$Calculate Moves King Cell Coords : [{kingCell.coords.x} {kingCell.coords.y}]");
            //     }
            // }

            return moves;
        }

        protected abstract List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix);
        
        protected List<Cell> ResolvePotentialCells(ChessBoardManager manager, Cell[,] matrix, PlayMode playMode)
        {
            List<Cell> cells;

            if (manager.PlayMode == PlayMode.RuleBased)
            {
                cells = ResolveAllAvailableQualifyingCells(matrix);
            }
            else
            {
                cells = ResolveAllAvailableCells(manager, matrix);
            }

            return cells;
        }

        private List<Cell> ResolveAllAvailableCells(ChessBoardManager manager, Cell[,] matrix)
        {
            List<Cell> cells = new List<Cell>();
            List<Coord> coords = manager.AllCoords;

            if (TryGetPotentialCoords(ActiveCell.coord, coords, out List<Coord> potentialCoords))
            {
                foreach (Coord coord in potentialCoords)
                {
                    Cell cell = matrix[coord.x, coord.y];

                    if ((cell.piece == null) || (cell.piece.Set != set))
                    {
                        cells.Add(matrix[coord.x, coord.y]);
                    }
                }
            }

            return cells;
        }

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

                if (TryGetPotentialCoord(coord, trueCoord.x, trueCoord.y, out Coord offsetCoord))
                {
                    potentialCoords.Add(offsetCoord);
                }
            }

            return (potentialCoords.Count > 0);
        }

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
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Chess.Pieces
{
    public abstract class PieceManager : MonoBehaviour, IFocus
    {
        [Header("Components")]
        [SerializeField] ChessBoardManager chessBoardManager;
        [SerializeField] PieceCanvasManager canvasManager;

        [Header("Config")]
        [SerializeField] protected PieceType type;
        public PieceType Type { get { return type; } }

        [SerializeField] protected Set set;
        public Set Set { get { return set; } }

        protected class CoordSpec
        {
            public bool includeOccupedCells;
            public bool onlyIncludeOccupiedCells;
        }

        protected class CoordBundle
        {
            public List<Coord> coords;
            public CoordSpec coordSpec;

            public CoordBundle()
            {
                coords = new List<Coord>();
                coordSpec = new CoordSpec();
            }
        }

        public Cell HomeCell
        {
            get
            {
                return homeCell;
            }
            
            set
            {
                ActiveCell = homeCell = value;
            }
        }

        public Cell ActiveCell
        {
            get
            {
                return activeCell;
            }
            
            set
            {
                activeCell = value;

                if ((canvasManager.isActiveAndEnabled) && TryGets.TryGetCoordReference(activeCell.coord, out string reference))
                {
                    canvasManager.Text = reference;
                }
            }
        }

        public delegate void MoveEvent(PieceManager piece);
        public event MoveEvent MoveEventReceived;

        public delegate void Event(PieceManager piece, FocusType focusType);
        public event Event EventReceived;
        
        protected int maxColumnIdx, maxRowIdx;

        private static CoordSpec DefaultCoordSpec = new CoordSpec
        {
            includeOccupedCells = true,
            onlyIncludeOccupiedCells = false
        };

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

        public List<Cell> CalculateMoves(Cell[,] matrix, int vector)
        {
            List<Cell> moves = new List<Cell>();
            List<Cell> potentialMoves = ResolvePotentialCells(matrix, vector, chessBoardManager.PlayMode);

            // if (manager.TryGetSetPiecesByType(set, PieceType.King, out List<Cell> cells))
            // {
            //     if (cells.Count > 0)
            //     {
            //         var kingCell = cells[0];
            //         Debug.Log($"Calculate Moves King Cell Coords : [{kingCell.coord.x} {kingCell.coord.y}]");
            //     }
            // }

            foreach (Cell potentialMove in potentialMoves)
            {
                // TODO determine if the potential move would put your own King in check.
                // If so, the move is NOT legel, otherwise add it to the list of moves

                // TMP measure to test the base functionality of the piece
                moves.Add(potentialMove);
            }

            return moves;
        }

        protected abstract List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector);

        protected virtual List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix, int vector)
        {
            List<Cell> cells = new List<Cell>();
            List<CoordBundle> coordBundles = GenerateCoordBundles(matrix, vector);

            foreach (CoordBundle bundle in coordBundles)
            {
                if (TryGetPotentialCoords(ActiveCell.coord, bundle.coords, out List<Coord> potentialCoords))
                {
                    cells.AddRange(EvaluatePotentialCells(matrix, potentialCoords, bundle.coordSpec));
                }
            }

            return cells;
        }

        private List<Cell> EvaluatePotentialCells(Cell[,] matrix, List<Coord> potentialCoords, CoordSpec coordSpec)
        {
            List<Cell> cells = new List<Cell>();

            foreach (Coord coord in potentialCoords)
            {
                Cell cell = matrix[coord.x, coord.y];

                if (cell.IsOccupied)
                {
                    if ((cell.piece.Set != set) && coordSpec.includeOccupedCells)
                    {
                        cells.Add(cell);
                    }

                    return cells;
                }
                else if (!coordSpec.onlyIncludeOccupiedCells)
                {
                    cells.Add(cell);
                }
            }

            return cells;
        }
        
        protected List<Cell> ResolvePotentialCells(Cell[,] matrix, int vector, PlayMode playMode)
        {
            List<Cell> cells;

            if (chessBoardManager.PlayMode == PlayMode.RuleBased)
            {
                cells = ResolveAllAvailableQualifyingCells(matrix, vector);
            }
            else
            {
                cells = ResolveAllAvailableCells(matrix);
            }

            return cells;
        }

        private List<Cell> ResolveAllAvailableCells(Cell[,] matrix)
        {
            List<Cell> cells = new List<Cell>();
            List<Coord> coords = chessBoardManager.AllCoords;

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

        protected List<CoordBundle> TryOneTimeVector(int stepX, int stepY, List<CoordBundle> bundles, CoordSpec coordSpec = null)
        {
            if (coordSpec == null)
            {
                coordSpec = DefaultCoordSpec;
            }

            return TryVector(stepX, stepY, bundles, coordSpec, 1);
        }

        protected List<CoordBundle> TryVector(int stepX, int stepY, List<CoordBundle> bundles, CoordSpec coordSpec = null, int? iterationCap = null)
        {
            List<Coord> vectorCoords;

            if (coordSpec == null)
            {
                coordSpec = DefaultCoordSpec;
            }

            if (TryGetVectorCoords(ActiveCell.coord, stepX, stepY, out vectorCoords, iterationCap))
            {
                bundles.Add(new CoordBundle
                {
                    coords = vectorCoords,
                    coordSpec = coordSpec
                });
            }

            return bundles;
        }

        protected bool TryGetVectorCoords(Coord origin, int stepX, int stepY, out List<Coord> coords, int? iterationCap = null)
        {
            if ((stepX == 0) && (stepY == 0))
            {
                coords = null;
                return false;
            }

            Coord coord = new Coord
            {
                x = origin.x,
                y = origin.y
            };

            coords = new List<Coord>();
            int itr = 0;

            do
            {
                if (iterationCap.HasValue && itr == iterationCap.Value) break;

                coord.x += stepX;
                coord.y += stepY;

                if ((coord.x >= 0) && (coord.x <= maxColumnIdx) && (coord.y >= 0) && (coord.y <= maxRowIdx))
                {
                    coords.Add(new Coord
                    {
                        x = coord.x,
                        y = coord.y
                    });    
                }
                else
                {
                    break;
                }

                ++itr;
            } while (true);

            return true;
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

        public void ResetTheme()
        {
            UseDefaultMaterial();
        }

        public virtual void Reset() => ResetTheme();

        public void EnablePhysics(bool enabled) => rigidbody.isKinematic = !enabled;

        public void EnableCollider(bool enabled) => collider.enabled = enabled;

        public void EnableInteractions(bool enabled)
        {
            rigidbody.isKinematic = !enabled;
            collider.enabled = enabled;
        }

        public void SnapToActiveCell()
        {
            EnableInteractions(false);

            transform.localRotation = originalRotation;
            transform.localPosition = ActiveCell.localPosition;

            EnableInteractions(true);
        }

        public void GoHome(float rotationSpeed, float movementSpeed) => StartCoroutine(GoToCellCoroutine(HomeCell, rotationSpeed, movementSpeed));

        public void GoToCell(Cell cell, float rotationSpeed, float movementSpeed) => StartCoroutine(GoToCellCoroutine(cell, rotationSpeed, movementSpeed));

        protected virtual void OnMove(Cell fromCell, Cell toCell, bool resetting) { }

        private IEnumerator GoToCellCoroutine(Cell cell, float rotationSpeed, float movementSpeed)
        {
            EnableInteractions(false);

            if ((cell != ActiveCell) && cell.IsOccupied)
            {
                if (chessBoardManager.SetManager.TryReserveSlot(cell.piece, out Vector3 localPosition))
                {
                    cell.piece.transform.localPosition = localPosition;
                    cell.piece.EnableInteractions(false);
                    cell.piece.ActiveCell = null;
                    cell.piece = null;
                }
            }

            while ((transform.localRotation != originalRotation) || (transform.localPosition != cell.localPosition))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalRotation, rotationSpeed * Time.deltaTime);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, cell.localPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }

            if (ActiveCell != null)
            {
                ActiveCell.piece = null;
            }

            OnMove(ActiveCell, cell, cell == HomeCell);

            ActiveCell = cell;
            ActiveCell.piece = this;

            MoveEventReceived?.Invoke(this);
        }
    }
}
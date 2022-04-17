using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Chess.Pieces
{
    public abstract class PieceManager : MonoBehaviour, IFocus
    {
        [Header("Components")]
        [SerializeField] protected ChessBoardManager chessBoardManager;
        [SerializeField] PieceCanvasManager canvasManager;

        [Header("Config")]
        [SerializeField] Set set;
        public Set Set { get { return set; } }
        [SerializeField] PieceType type;
        public PieceType Type { get { return type; } }

        [SerializeField] float rotationSpeed = 25f;
        [SerializeField] float classicMovementSpeed = 25f;
        [SerializeField] float parabolaMovementSpeed = 2.5f;


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
        }

        private void ResolveDependencies()
        {
            filter = GetComponent<MeshFilter>() as MeshFilter;
            renderer = GetComponent<MeshRenderer>() as MeshRenderer;
            rigidbody = GetComponent<Rigidbody>() as Rigidbody;
            collider = GetComponent<Collider>() as Collider;
        }

#if false
        public void TestRealtiveKingVectors()
        {
            if ((type != PieceType.King) && (chessBoardManager.TryGetPiecesByType(PieceType.King, out List<Cell> cells)))
            {
                foreach (Cell king in cells)
                {
                    Cell activeCell = ActiveCell;
                    Vector2 vector = GetNormalizedVector(new Vector2(activeCell.coord.x, activeCell.coord.y), new Vector2(king.coord.x, king.coord.y));
                    Vector? vectorType = null;

                    if ((vector.y != 0) && (vector.x != 0))
                    {
                        if (Mathf.Abs(vector.x) == Mathf.Abs(vector.y))
                        {
                            vectorType = Vector.Diagonal;
                            vector = new Vector2((vector.x >= 0) ? 1 : -1, (vector.y >= 0) ? 1 : -1);
                        }
                    }
                    else if (vector.y != 0)
                    {
                        vectorType = Vector.Vertical;
                    }
                    else if (vector.x != 0)
                    {
                        vectorType = Vector.Horizontal;
                    }

                    if (vectorType.HasValue)
                    {
                        Debug.Log($"TestKingVector King : {king.piece.name} Piece : {name} Type : {vectorType.Value} Vector : [{vector.x}, {vector.y}]");
                    }
                }
            }
        }

        public void TestRealtiveKingVectors()
        {
            if ((type != PieceType.King) && (chessBoardManager.TryGetPiecesByType(PieceType.King, out List<Cell> cells)))
            {
                foreach (Cell kingCell in cells)
                {
                    if (TryGetRealtiveKingVector(kingCell, out VectorPackage package))
                    {
                        Debug.Log($"TestKingVector King : {kingCell.piece.name} Piece : {name} Type : {package.Type} Vector : [{package.Vector.x}, {package.Vector.y}]");
                    }
                }
            }
        }
#endif

        private bool TryGetRealtiveKingVector(Cell kingCell, out VectorPackage package)
        {
            Cell activeCell = ActiveCell;
            Vector2 normalizedVector = GetNormalizedVector(new Vector2(activeCell.coord.x, activeCell.coord.y), new Vector2(kingCell.coord.x, kingCell.coord.y));
            
            VectorType? vectorType = null;

            if ((normalizedVector.y != 0) && (normalizedVector.x != 0))
            {
                if (Mathf.Abs(normalizedVector.x) == Mathf.Abs(normalizedVector.y))
                {
                    normalizedVector = new Vector2((normalizedVector.x >= 0) ? 1 : -1, (normalizedVector.y >= 0) ? 1 : -1);
                    vectorType = VectorType.Diagonal;
                }
            }
            else if (normalizedVector.y != 0)
            {
                vectorType = VectorType.Vertical;
            }
            else if (normalizedVector.x != 0)
            {
                vectorType = VectorType.Horizontal;
            }

            if (vectorType.HasValue)
            {
                package = new VectorPackage
                {
                    Type = vectorType.Value,
                    Vector = normalizedVector
                };

                return true;
            }

            package = default(VectorPackage);
            return false;
        }

        public Mesh Mesh { get { return filter.mesh; } }

        public void HideMesh() => renderer.enabled = false;

        public void ShowMesh() => renderer.enabled = true;

        public void GainedFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusGained);

        public void LostFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusLost);

        public void ApplyMaterial(Material material) => renderer.material = material;

        public Material Material { get { return renderer.material; } }

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

        private Vector2 GetNormalizedVector(Vector2 origin, Vector2 target)
        {
            return (target - origin).normalized;
        }

        public Cell ResolveKingCell(Set set)
        {
            if (chessBoardManager.TryGetSingleSetPieceByType(set, PieceType.King, out Cell cell))
            {
                return cell;
            }

            return null;
        }

        private Cell[,] ProjectMatrix(Cell cell, Cell targetCell)
        {
            Cell[,] clone = chessBoardManager.CloneMatrix();

            PieceManager piece = cell.piece;
            cell.piece = null;
            targetCell.piece = piece;

            return clone;
        }

        private bool WouldMovePlaceKingInCheck(Cell kingCell, Cell targetCell)
        {
            if (kingCell == null) return false;

            Debug.Log($"WouldMovePlaceKingInCheck King : {kingCell.piece.name} Piece : {name} Target Cell : [{targetCell.coord.x}, {targetCell.coord.y}]");

#if true
            if (TryGetRealtiveKingVector(kingCell, out VectorPackage package))
            {
                Debug.Log($"WouldMovePlaceKingInCheck Vector : [{package.Vector.x}, {package.Vector.y}] Type : {package.Type}");

                Cell[,] projectedMatrix = ProjectMatrix(ActiveCell, targetCell);

                if (chessBoardManager.TryGetPiecesAlongVector(projectedMatrix, kingCell, package.Vector, out List<PieceManager> pieces))
                {
                    foreach (PieceManager piece in pieces)
                    {
                        Debug.Log($"WouldMovePlaceKingInCheck Piece : {piece.name}");
                    }
                }
            }
#endif

            return false;
        }

        private bool WouldMovingKingPlaceKingInCheck(Cell kingCell, Cell targetCell)
        {
            if (kingCell == null) return false;

            Debug.Log($"WouldMovingKingPlaceKingInCheck King : {kingCell.piece.name} Piece : {name} Target Cell : [{targetCell.coord.x}, {targetCell.coord.y}]");

            return false;
        }

        public List<Cell> CalculateMoves(Cell[,] matrix, int vector)
        {
            // Debug.Log($"CalculateMoves Piece : {name}");

            List<Cell> moves = new List<Cell>();
            List<Cell> potentialMoves = ResolvePotentialCells(matrix, vector, chessBoardManager.PlayMode);

#if true
            Cell kingCell = ResolveKingCell(set);

            foreach (Cell potentialMove in potentialMoves)
            {
                // Debug.Log($"CalculateMoves Piece : {name} Move : [{potentialMove.coord.x}, {potentialMove.coord.y}]");
                moves.Add(potentialMove);
            }
#endif

#if false
            Cell kingCell = ResolveKingCell(set);
            
            foreach (Cell potentialMove in potentialMoves)
            {
                Debug.Log($"CalculateMoves Piece : {name} Move : [{potentialMove.coord.x}, {potentialMove.coord.y}]");

                if ((type == PieceType.King) && (!WouldMovingKingPlaceKingInCheck(kingCell, potentialMove)))
                {
                    // Moving the King to the potential cell would not place it in check by a piece of the opposing set.
                    moves.Add(potentialMove);
                }
                else if (!WouldMovePlaceKingInCheck(kingCell, potentialMove))
                {
                    // Moving the piece relative to the King would not expose the King to check by a piece of the opposing set.
                    moves.Add(potentialMove);
                }
            }
#endif

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

        public void GoHome(MoveStyle moveStyle) => StartCoroutine(GoToCellCoroutine(HomeCell, moveStyle));

        public void GoToCell(Cell cell, MoveStyle moveStyle) => StartCoroutine(GoToCellCoroutine(cell, moveStyle));

        protected virtual void OnMove(Cell fromCell, Cell toCell, bool resetting) { }

        private IEnumerator GoToCellCoroutine(Cell cell, MoveStyle moveStyle)
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

            if (moveStyle == MoveStyle.Parabola)
            {
                yield return StartCoroutine(AdvancedMoveCoroutine(cell, rotationSpeed, parabolaMovementSpeed));
            }
            else
            {
                yield return StartCoroutine(ClassicMoveCoroutine(cell, rotationSpeed, classicMovementSpeed));
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

        private IEnumerator ClassicMoveCoroutine(Cell cell, float rotationSpeed, float movementSpeed)
        {
            while ((transform.localRotation != originalRotation) || (transform.localPosition != cell.localPosition))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalRotation, rotationSpeed * Time.deltaTime);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, cell.localPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator AdvancedMoveCoroutine(Cell cell, float rotationSpeed, float movementSpeed)
        {
            Vector3 targetPosition = ChessMath.RoundVector3(cell.localPosition);
            Vector3 startPosition = ChessMath.RoundVector3(transform.localPosition);
            
            float distance = Vector3.Distance(startPosition, targetPosition);
            float duration = distance / movementSpeed;
            float timestamp = 0;

            while ((transform.localRotation != originalRotation) || (ChessMath.RoundVector3(transform.localPosition) != targetPosition))
            {
                timestamp += Time.deltaTime;

                float timeframe = Mathf.Clamp01(timestamp / duration);

                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalRotation, rotationSpeed * Time.deltaTime);
                transform.localPosition = Parabola.MathParabola.Parabola(startPosition, targetPosition, 0.25f, timeframe);

                if (timeframe >= 1f)
                {
                    transform.localRotation = originalRotation;
                    transform.localPosition = targetPosition;
                }

                yield return null;
            }
       }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;

namespace Chess.Pieces
{
    [RequireComponent(typeof(Outline))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PieceSessionManager))]
    public abstract class PieceManager : MonoBehaviour, IFocus
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        
        [Header("Components")]
        [SerializeField] protected ChessBoardManager chessBoardManager;
        public ChessBoardManager ChessBoardManager { get { return chessBoardManager; } set { chessBoardManager = value; } }

        [SerializeField] PieceCanvasManager canvasManager;

        [Header("Animations")]
        [SerializeField] RuntimeAnimatorController shrinkAnimator;
        [SerializeField] RuntimeAnimatorController growAnimator;

        [Header("Config")]
        [SerializeField] Set set;
        public Set Set { get { return set; } set { set = value; } }
        [SerializeField] PieceType type;
        public PieceType Type { get { return type; } }

        public bool IsAddInPiece
        {
            get
            {
                return isAddInPiece;
            }

            set
            {
                isAddInPiece = value;
            }
        }

        public enum WhereAt
        {
            None,
            Cell,
            Slot
        }

        protected PieceSessionManager pieceSessionManager;
        public PieceSessionManager PieceSessionManager { get { return pieceSessionManager; } }

        protected bool hasHistory;
        public bool HasHistory { get { return hasHistory; } }

        private float rotationSpeed = 25f;
        private float moveSpeed = 25f;
        private float parabolaApexHeight = 0.25f;
        private bool isAddInPiece = false;
        private Vector3 originalScale;

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
                activeCell = homeCell = value;
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

        public delegate void Event(PieceManager manager, FocusType focusType);
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
        protected Material defaultMaterial;
        private Cell homeCell;
        private Cell activeCell;
        private Vector3? slot;
        private Outline outline;
        private Animator animator;
        private WhereAt whereAt;
                
        void Awake()
        {
            ResolveDependencies();

            maxColumnIdx = MatrixManager.MatrixColumns - 1;
            maxRowIdx = MatrixManager.MatrixRows - 1;
            
            defaultMaterial = renderer.material;
            originalScale = transform.localScale;
        }

        private void ResolveDependencies()
        {
            filter = GetComponent<MeshFilter>() as MeshFilter;
            renderer = GetComponent<MeshRenderer>() as MeshRenderer;
            rigidbody = GetComponent<Rigidbody>() as Rigidbody;
            collider = GetComponent<Collider>() as Collider;
            outline = GetComponent<Outline>() as Outline;
            animator = GetComponent<Animator>() as Animator;
        }

        public void AnimateOut()
        {
            animator.runtimeAnimatorController = shrinkAnimator;
            animator.SetTrigger("transform");
        }
        
        public void AnimateIn()
        {
            animator.runtimeAnimatorController = growAnimator;
            animator.SetTrigger("transform");
        }

        private bool TryGetRealtiveVector(Cell cell, out VectorPackage package)
        {
            Cell activeCell = ActiveCell;
            Vector2 normalizedVector = GetNormalizedVector(new Vector2(activeCell.coord.x, activeCell.coord.y), new Vector2(cell.coord.x, cell.coord.y));
            
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

        private bool TryGetRealtiveKingVector(Set set, out VectorPackage package)
        {
            Cell kingCell = chessBoardManager.MatrixManager.ResolveKingCell(set);
            return TryGetRealtiveVector(kingCell, out package);
        }

        public Mesh Mesh { get { return filter.mesh; } }

        public void HideMesh() => renderer.enabled = false;

        public void ShowMesh() => renderer.enabled = true;

        public void HideOutline() => outline.enabled = false;

        public void ShowOutline() => outline.enabled = true;

        public void GainedFocus(GameObject gameObject, Vector3 point) => EventReceived?.Invoke(this, FocusType.OnFocusGained);

        public void LostFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusLost);

        public void ApplyMaterial(Material material) => renderer.material = material;

        public Material DefaultMaterial { get { return defaultMaterial; } }
        
        public Material Material { get { return renderer.material; } }

        public virtual void UseDefaultMaterial() => ApplyMaterial(defaultMaterial);

        private Vector2 GetNormalizedVector(Vector2 origin, Vector2 target)
        {
            return (target - origin).normalized;
        }

        public bool CanMoveTo(Cell[,] matrix, Cell targetCell)
        {
            List<Cell> moves = CalculateMoves(matrix, (set == Set.Light) ? 1 : -1);
            return (moves.FirstOrDefault(c => c == targetCell) != null);
        }

        public List<Cell> CalculateMoves(Cell[,] matrix, int vector)
        {
            List<Cell> moves = new List<Cell>();
            List<Cell> potentialMoves = ResolvePotentialCells(matrix, vector);

            foreach (Cell potentialMove in potentialMoves)
            {
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
                    if ((cell.wrapper.manager.Set != set) && coordSpec.includeOccupedCells)
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
        
        protected List<Cell> ResolvePotentialCells(Cell[,] matrix, int vector)
        {
            return ResolveAllAvailableQualifyingCells(matrix, vector);
        }

        private List<Cell> ResolveAllAvailableCells(Cell[,] matrix)
        {
            List<Cell> cells = new List<Cell>();
            List<Coord> coords = chessBoardManager.MatrixManager.AllCoords;

            if (TryGetPotentialCoords(ActiveCell.coord, coords, out List<Coord> potentialCoords))
            {
                foreach (Coord coord in potentialCoords)
                {
                    Cell cell = matrix[coord.x, coord.y];

                    if ((cell.wrapper.manager == null) || (cell.wrapper.manager.Set != set))
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
                if (chessBoardManager.MatrixManager.TryGetPotentialCoord(coord, thisCoord.x, thisCoord.y, out Coord offsetCoord))
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
                Coord trueCoord = chessBoardManager.MatrixManager.GetCoordByOffset(coord, offset.x, offset.y);

                if (chessBoardManager.MatrixManager.TryGetPotentialCoord(coord, trueCoord.x, trueCoord.y, out Coord offsetCoord))
                {
                    potentialCoords.Add(offsetCoord);
                }
            }

            return (potentialCoords.Count > 0);
        }

        public void ResetTheme()
        {
            UseDefaultMaterial();
            HideOutline();
        }

        private void ResetScale() => transform.localScale = originalScale;

        private void ResetState()
        {
            hasHistory = false;
            activeCell = homeCell;
            slot = null;
            whereAt = WhereAt.None;
        }

        public virtual void Reset()
        {
            ResetTheme();
            ResetScale();
            ResetState();
        }

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

            transform.localRotation = (set == Set.Light) ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);;
            transform.localPosition = ActiveCell.localPosition;

            EnableInteractions(true);
        }

        public void GoHome(MoveType moveType, MoveStyle moveStyle) => StartCoroutine(GoToCellCoroutine(HomeCell, moveType, moveStyle));

        public void GoToCell(Cell cell, MoveType moveType, MoveStyle moveStyle) => StartCoroutine(GoToCellCoroutine(cell, moveType, moveStyle));

        protected virtual void OnMove(Cell fromCell, Cell toCell, bool resetting)
        {
            if (!resetting)
            {
                hasHistory = true;
            }
        }

        public void MoveToSlot()
        {
            if (chessBoardManager.SetManager.TryReserveSlot(this, out Vector3 localPosition))
            {
                EnableInteractions(false);
                slot = localPosition;
                transform.localPosition = slot.Value;
                activeCell.wrapper.manager = null;
                activeCell = null;
                ShowMesh();

                whereAt = WhereAt.Slot;
            }
        }

        private IEnumerator GoToCellCoroutine(Cell cell, MoveType moveType, MoveStyle moveStyle)
        {
            EnableInteractions(false);

            // bool isResetting = (cell == homeCell);

            // if (cell.IsOccupied)
            // {
            //     PieceManager piece = cell.wrapper.manager;

            //     if (piece.IsAddInPiece)
            //     {
            //         chessBoardManager.SetManager.RemovePiece(piece);
            //     }
            //     else if (!isResetting)
            //     {
            //         MoveToSlot(cell);
            //     }
            // }

            bool doMove = (cell != activeCell);

            if (doMove)
            {
                Vector3 targetPosition = ChessMath.RoundVector3(cell.localPosition);
                Vector3 startPosition = ChessMath.RoundVector3(transform.localPosition);
                float distance = Vector3.Distance(startPosition, targetPosition);

                float rotationSpeed = chessBoardManager.PieceRotationSpeed;
                float moveSpeed = chessBoardManager.PieceMoveSpeed;
                float speed = (moveType == MoveType.TimeRelativeToDistance) ? moveSpeed : moveSpeed * distance;

                if (moveStyle == MoveStyle.Parabola)
                {
                    yield return StartCoroutine(ParabolaMoveCoroutine(cell, rotationSpeed, speed));
                }
                else
                {
                    yield return StartCoroutine(DirectMoveCoroutine(cell, rotationSpeed, speed));
                }
            }

            if (activeCell != null)
            {
                activeCell.wrapper.manager = null;
            }

            OnMove(activeCell, cell, cell == homeCell);

            activeCell = cell;
            activeCell.wrapper.manager = this;
            whereAt = WhereAt.Cell;

            MoveEventReceived?.Invoke(this);
        }

        private IEnumerator DirectMoveCoroutine(Cell cell, float rotationSpeed, float movementSpeed)
        {
            Quaternion targetRotation = (set == Set.Light) ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);

            while ((transform.localRotation != targetRotation) || (transform.localPosition != cell.localPosition))
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, cell.localPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator ParabolaMoveCoroutine(Cell cell, float rotationSpeed, float movementSpeed)
        {
            Vector3 targetPosition = ChessMath.RoundVector3(cell.localPosition);
            Vector3 startPosition = ChessMath.RoundVector3(transform.localPosition);
            
            float distance = Vector3.Distance(startPosition, targetPosition);
            float duration = distance / movementSpeed;
            float timestamp = 0;

            Quaternion targetRotation = (set == Set.Light) ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);

            while ((transform.localRotation != targetRotation) || (ChessMath.RoundVector3(transform.localPosition) != targetPosition))
            {
                timestamp += Time.deltaTime;

                float timeframe = Mathf.Clamp01(timestamp / duration);

                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.localPosition = Parabola.MathParabola.Parabola(startPosition, targetPosition, parabolaApexHeight, timeframe);

                if (timeframe >= 1f)
                {
                    transform.localRotation = targetRotation;
                    transform.localPosition = targetPosition;
                }

                yield return null;
            }
       }
    }
}
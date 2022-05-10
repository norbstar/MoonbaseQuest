using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

using Random = UnityEngine.Random;

using static Enum.ControllerEnums;
using static Chess.StageManager;

using Chess.Pieces;
using Chess.Button;
using Chess.Preview;

namespace Chess
{
    [RequireComponent(typeof(AudioSource))]
    public class ChessBoardManager : BaseManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

#region Variables
        [Header("Camera")]
        [SerializeField] new Camera camera;

        [Header("Controllers")]
        [SerializeField] HybridHandController leftController;
        [SerializeField] HybridHandController rightController;

        [Header("Components")]
        [SerializeField] GameObject board;
        [SerializeField] ChessBoardSetManager setManager;
        public ChessBoardSetManager SetManager { get { return setManager; } }
        [SerializeField] PieceTransformManager pieceTransformManager;
        [SerializeField] NotificationManager notificationManager;
        
        [SerializeField] NewGameManager newGameManager;
        [SerializeField] PawnPromotionManager pawnPromotionManager;

        [Header("Clocks")]
        [SerializeField] TimerClockManager lightClock;
        [SerializeField] TimerClockManager darkClock;

        [Header("Audio")]
        [SerializeField] AudioClip adjustTableHeightClip;
        [SerializeField] AudioClip pieceDownClip;
        [SerializeField] AudioClip inCheckClip;
        [SerializeField] AudioClip checkmateClip;
        [SerializeField] AudioClip stalemateClip;
        [SerializeField] AudioClip pawnPromotionClip;

        [Header("Canvases")]
        [SerializeField] CoordReferenceCanvas coordReferenceCanvas;

        [Header("Pieces")]
        [Range(15f, 35f)]
        [SerializeField] float pieceRotationSpeed = 25f;
        public float PieceRotationSpeed { get { return pieceRotationSpeed; } }

        [Range(0.1f, 25f)]
        [SerializeField] float pieceMoveSpeed = 5f;
        public float PieceMoveSpeed { get { return pieceMoveSpeed; } }

        [Header("Materials")]
        [SerializeField] Material outOfScopeMaterial;
        [SerializeField] Material inFocusMaterial;
        [SerializeField] Material selectedMaterial;
        [SerializeField] Material pawnPromotionMaterial;

        [Header("Config")]
        [SerializeField] MoveStyle moveStyle;
        public MoveStyle MoveStyle { get { return moveStyle; } }

        [SerializeField] MoveType moveType;
        public MoveType MoveType { get { return moveType; } }

        [SerializeField] EngagementMode engagementMode;
        public EngagementMode EngagementMode { get { return engagementMode; } }

        [SerializeField] PlayerMode lightPlayer;
        public PlayerMode LightPlayer { get { return lightPlayer; } }
        [SerializeField] PlayerMode darkPlayer;
        public PlayerMode DarkPlayer { get { return darkPlayer; } }

        [SerializeField] GameObject previewPrefab;
        [SerializeField] float adjustTableSpeed = 0.5f;

        public delegate void MatrixEvent(Cell[,] matrix);
        public static event MatrixEvent MatrixEventReceived;

        public static int MatrixRows = 8;
        public static int MaxRowIdx = MatrixRows - 1;
        public static int MatrixColumns = 8;
        public static int MaxColumnIdx = MatrixColumns - 1;

        private TrackingMainCameraManager cameraManager;
        private AudioSource cameraAudioSource;
        private Cell[,] matrix;
        private int onHomeEventsPending;
        private Set activeSet;
        private PieceManager inFocusPiece;
        private PieceManager promotionPiece;
        private PieceManager pickedPiece;
        private PreviewManager inFocusPreview;
        private StageManager stageManager;
        private Dictionary<PieceManager, List<Cell>> availableMoves;
        private List<GameObject> previews;
        private float defaultTableYOffset = 0f;
        private float lowerYTableBounds = -0.25f;
        private float upperYTableBounds = 0.25f;
        private float inCheckNotificationDelay = 1f;
        private Coroutine coroutine;
        private AudioSource audioSource;
        private bool playPieceDownClip;
        // private bool gameOver;
#endregion

        void Awake()
        {
            ResolveDependencies();

            matrix = new Cell[MatrixColumns, MatrixRows];
            previews = new List<GameObject>();
            availableMoves = new Dictionary<PieceManager, List<Cell>>();
            stageManager = new StageManager();
            
            AttachLayerToControllers("Preview Layer");

            playPieceDownClip = true;
        }

        private void ResolveDependencies()
        {
            cameraManager = camera.GetComponent<TrackingMainCameraManager>() as TrackingMainCameraManager;

            if (cameraManager != null)
            {
                cameraAudioSource = cameraManager.GetComponent<AudioSource>() as AudioSource;
            }

            audioSource = GetComponent<AudioSource>() as AudioSource;
        }

        // Start is called before the first frame update
        void Start()
        {
            MapLayout();
            StartCoroutine(ShowNewGameUIAfterDelayCoroutine(0.25f));
        }

#region Mappings
        private void MapLayout()
        {
            MapMatrix();
            MapPieces();
        }

        private void MapMatrix()
        {
            for (int y = 0 ; y <= MaxRowIdx ; y++)
            {
                for (int x = 0 ; x <= MaxColumnIdx ; x++)
                {
                    Coord coord = new Coord
                    {
                        x = x,
                        y = y
                    };

                    Vector3 surface = setManager.transform.localPosition;

                    if (TryGets.TryGetCoordToPosition(coord, surface.y, out Vector3 localPosition))
                    {
                        matrix[x, y] = new Cell
                        {
                            coord = coord,
                            localPosition = localPosition,
                            wrapper = new PieceManagerWrapper()
                        };
                    }
                }
            }
        }

        private void MapPieces()
        {
            foreach (PieceManager pieceManager in EnabledPieces)
            {
                if ((pieceManager.isActiveAndEnabled) && (TryGets.TryGetPieceToCell(matrix, pieceManager, out Cell cell)))
                {
                    pieceManager.HomeCell = cell;
                    pieceManager.MoveEventReceived += OnMoveEvent;
                    pieceManager.EventReceived += OnEvent;

                    matrix[cell.coord.x, cell.coord.y].wrapper.manager = pieceManager;
                }
            }
        }
#endregion

        void OnEnable()
        {
            HandController.ActuationEventReceived += OnActuation;
            ButtonEventManager.EventReceived += OnButtonEvent;
            PreviewManager.EventReceived += OnPreviewEvent;
            NewGameManager.EventReceived += OnNewGameEvent;
            PawnPromotionManager.EventReceived += OnPawnPromotionEvent;
            PieceTransformManager.CompleteEventReceived += OnPieceTransformComplete;
            ClockManager.OnExpiredEventReceived += OnClockExpiredEvent;
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;
            ButtonEventManager.EventReceived -= OnButtonEvent;
            PreviewManager.EventReceived -= OnPreviewEvent;
            NewGameManager.EventReceived -= OnNewGameEvent;
            PawnPromotionManager.EventReceived -= OnPawnPromotionEvent;
            PieceTransformManager.CompleteEventReceived -= OnPieceTransformComplete;
            ClockManager.OnExpiredEventReceived -= OnClockExpiredEvent;
        }

        private void OnButtonEvent(ButtonEventManager manager, ButtonEventManager.ButtonId id, ButtonEventType eventType)
        {
            ReassignableButtonEventManager reassignableManager = null;

            if (eventType == ButtonEventType.OnPressed)
            {
                switch (id)
                {
                    case ButtonEventManager.ButtonId.LowerTable:
                        LowerTable();
                        break;

                    case ButtonEventManager.ButtonId.RaiseTable:
                        RaiseTable();
                        break;

                    case ButtonEventManager.ButtonId.ResetTable:
                        ResetTable();
                        break;

                    case ButtonEventManager.ButtonId.ResetBoard:
                        // if (!gameOver)
                        // {
                        //     DeferAction(ResetBoard);
                        // }
                        break;

                    case ButtonEventManager.ButtonId.SFXOff:
                        EnableSFX(false);

                        reassignableManager = ((ReassignableButtonEventManager) manager);
                        reassignableManager.Id = ButtonEventManager.ButtonId.SFXOn;
                        reassignableManager.Text = "On";
                        break;

                    case ButtonEventManager.ButtonId.SFXOn:
                        EnableSFX(true);

                        reassignableManager = ((ReassignableButtonEventManager) manager);
                        reassignableManager.Id = ButtonEventManager.ButtonId.SFXOff;
                        reassignableManager.Text = "Off";
                        break;

                    case ButtonEventManager.ButtonId.MusicOff:
                        EnableMusic(false);

                        reassignableManager = ((ReassignableButtonEventManager) manager);
                        reassignableManager.Id = ButtonEventManager.ButtonId.MusicOn;
                        reassignableManager.Text = "On";
                        break;

                    case ButtonEventManager.ButtonId.MusicOn:
                        EnableMusic(true);

                        reassignableManager = ((ReassignableButtonEventManager) manager);
                        reassignableManager.Id = ButtonEventManager.ButtonId.MusicOff;
                        reassignableManager.Text = "Off";
                        break;
                }
            }
            else if (eventType == ButtonEventType.OnReleased)
            {
                switch (id)
                {
                    case ButtonEventManager.ButtonId.LowerTable:
                    case ButtonEventManager.ButtonId.RaiseTable:
                        LockTable();
                        break;
                }
            }
        }

#region Pieces
        public List<PieceManager> EnabledPieces { get { return setManager.AllPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        public List<PieceManager> ActiveEnabledPieces { get { return setManager.AllPieces().Where(p => p.isActiveAndEnabled && (p.ActiveCell != null)).ToList(); } }

        public List<PieceManager> EnabledLightPieces { get { return setManager.LightPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        public List<PieceManager> ActiveEnabledLightPieces { get { return setManager.LightPieces().Where(p => p.isActiveAndEnabled && (p.ActiveCell != null)).ToList(); } }

        public List<PieceManager> EnabledDarkPieces { get { return setManager.DarkPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        public List<PieceManager> ActiveEnabledDarkPieces { get { return setManager.DarkPieces().Where(p => p.isActiveAndEnabled && (p.ActiveCell != null)).ToList(); } }
#endregion

#region Reset
        private void ResetGame()
        {
            ResetUI();
            ResetGameState();
            ResetSet();
            ResetClocks();
            PostMatrixUpdate();
        }

        private void ResetUI() => coordReferenceCanvas.TextUI = String.Empty;
        
        private void ResetGameState()
        {
            activeSet = Set.Light;
            // deferAction = gameOver = false;
        }

        private void ResetSet() => setManager.Reset();
#endregion

#region Clocks
        private void ResetClocks()
        {
            lightClock.Reset();
            darkClock.Reset();
        }

        private void PauseClocks()
        {
            lightClock.Pause();
            darkClock.Pause();
        }

        private void PauseActiveClock()
        {
            switch (activeSet)
            {
                case Set.Light:
                    lightClock.Pause();
                    break;

                case Set.Dark:
                    darkClock.Pause();
                    break;
            }
        }

        private void ResumeActiveClock()
        {
            switch (activeSet)
            {
                case Set.Light:
                    lightClock.Resume();
                    break;

                case Set.Dark:
                    darkClock.Resume();
                    break;
            }
        }

        private void OnClockExpiredEvent(ClockManager instance)
        {
            Debug.Log($"{instance.name} has expired");
        }
#endregion

        private void PostMatrixUpdate() => MatrixEventReceived?.Invoke(matrix);

        private void HideNotifications() => notificationManager.Hide();

        private bool IsMoving { get { return stageManager.LiveStage == Stage.Moving; } }

        private void EvaluateOpeningMove() => EvaluateMove();

        private void EvaluateMove()
        {
            stageManager.LiveStage = Stage.Evaluating;
            availableMoves.Clear();
            inFocusPiece = null;
            
            EnableInteractions(Set.Light, false);
            EnableInteractions(Set.Dark, false);

            bool inCheck = IsKingInCheck(activeSet, matrix);
            bool hasMoves = CalculateMoves();

            ResumeActiveClock();

            if (hasMoves)
            {
                if (inCheck)
                {
                    OnCheck(hasMoves);
                }

                if (ShouldAutomate())
                {
                    AutomateMove();
                }
            }
            else
            {
                if (inCheck)
                {
                    OnCheckmate();
                }
                else
                {
                    OnStalemate();
                }
            }
        }

        public Cell[,] CloneMatrix()
        {
            return matrix.Clone() as Cell[,];
        }

        private void AdjustChessPieceInteractableLayer(Set set, bool enabled)
        {
            string layer = null;

            switch (set)
            {
                case Set.Light:
                    layer = "Light Chess Piece Layer";
                    break;

                case Set.Dark:
                    layer = "Dark Chess Piece Layer";
                    break;
            }
            
            if (enabled)
            {
                AttachLayerToControllers(layer);
            }
            else
            {
                DetachLayerFromContollers(layer);
            }
        }

        private void EnableInteractions(Set set, bool enabled)
        {
            List<PieceManager> pieces = (set == Set.Light) ? EnabledLightPieces : EnabledDarkPieces;

            foreach (PieceManager piece in pieces)
            {
                if (piece.ActiveCell != null)
                {
                    piece.EnableInteractions(enabled);
                }
            }

            AdjustChessPieceInteractableLayer(set, enabled);
        }

        public bool IsKingInCheck(Set set, Cell[,] matrix)
        {
            if (TryGets.TryResolveKingCell(matrix, set, out Cell kingCell))
            {
                if (TryGets.TryGetSetPieces(this, (activeSet == Set.Light) ? Set.Dark : Set.Light, out List<PieceManager> opposingPieces))
                {
                    if (TryGets.TryGetCoordReference(kingCell.coord, out string kingReference))
                    {
                        foreach (PieceManager opposingPiece in opposingPieces)
                        {
                            TryGets.TryGetCoordReference(opposingPiece.ActiveCell.coord, out string reference);

                            if (opposingPiece.CanMoveTo(matrix, kingCell))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool CalculateMoves()
        {
            bool hasAnyMoves = false;

            List<PieceManager> activeEnabledPieces = (activeSet == Set.Light) ? ActiveEnabledLightPieces : ActiveEnabledDarkPieces;

            foreach (PieceManager piece in activeEnabledPieces)
            {
                List<Cell> potentialMoves = piece.CalculateMoves(matrix, (activeSet == Set.Light) ? 1 : -1);
                bool hasMoves = potentialMoves.Count > 0;

                List<Cell> legalMoves = new List<Cell>();

                foreach (Cell move in potentialMoves)
                {
                    if (!WouldKingBeInCheck(piece, move))
                    {
                        legalMoves.Add(move);
                    }
                }

                hasMoves = legalMoves.Count > 0;

                piece.EnableInteractions(hasMoves);

                if (hasMoves)
                {    
                    this.availableMoves.Add(piece, legalMoves);
                    hasAnyMoves = true;
                }
                else
                {
                    piece.ApplyMaterial(outOfScopeMaterial);
                }
            }

            stageManager.LiveStage = Stage.PendingSelect;
            
            AdjustChessPieceInteractableLayer(Set.Light, (activeSet == Set.Light));
            AdjustChessPieceInteractableLayer(Set.Dark, (activeSet == Set.Dark));

            return hasAnyMoves;
        }

        private Cell[,] ProjectMatrix(Cell cell, Cell targetCell)
        {
            Cell[,] clone = CloneMatrix();

            for (int y = 0 ; y <= MaxRowIdx ; y++)
            {
                for (int x = 0 ; x <= MaxColumnIdx ; x++)
                {
                    Cell thisCell = clone[x, y];
                    clone[x, y] = thisCell.Clone();
                }
            }

            PieceManager manager = cell.wrapper.manager;
            clone[cell.coord.x, cell.coord.y].wrapper.manager = null;
            clone[targetCell.coord.x, targetCell.coord.y].wrapper.manager = manager;

            return clone;
        }

        public bool WouldKingBeInCheck(PieceManager piece, Cell targetCell)
        {
            Cell[,] projectedMatrix = ProjectMatrix(piece.ActiveCell, targetCell);
            return IsKingInCheck(activeSet, projectedMatrix);
        }

        public Cell ResolveKingCell(Set set)
        {
            return ResolveKing(set).ActiveCell;
        }

        public PieceManager ResolveKing(Set set)
        {
            if (TryGets.TryGetSingleSetPieceByType(this, set, PieceType.King, out PieceManager piece))
            {
                return piece;
            }

            return null;
        }

        private bool ShouldAutomate()
        {
            bool result = false;

            switch (activeSet)
            {
                case Set.Light:
                    result = (lightPlayer == PlayerMode.Bot);
                    break;

                case Set.Dark:
                    result = (darkPlayer == PlayerMode.Bot);
                    break;
            }

            return result;
        }

        private void AutomateMove() => PlayRandomMove();

        private void PlayRandomMove()
        {
            if (availableMoves.Count == 0) return;

            int idx = Random.Range(0, availableMoves.Count);
            KeyValuePair<PieceManager, List<Cell>> availableCells = availableMoves.ElementAt(idx);
            int cellIdx = Random.Range(0, availableCells.Value.Count);
            inFocusPiece = availableCells.Key;
            Cell cell = availableCells.Value[cellIdx];
            CommitToMove(cell);
        }

        private void CompleteTurn()
        {
            stageManager.LiveStage = Stage.MoveComplete;

            PauseActiveClock();

            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Nominal;

            activeSet = (activeSet == Set.Light) ? Set.Dark : Set.Light;
            EvaluateMove();
        }

        private void ResetThemes()
        {
            foreach (PieceManager piece in EnabledPieces)
            {
                if (piece.isActiveAndEnabled)
                {
                    piece.ResetTheme();
                }
            }
        }

        public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
        {
            if ((stageManager.LiveStage == Stage.Evaluating) || (inFocusPiece == null)) return;

            if (actuation.HasFlag(Actuation.Button_AX))
            {
                ProcessIntent();
            }
            else if (actuation.HasFlag(Actuation.Button_BY))
            {
                if (stageManager.LiveStage == Stage.Selected)
                {
                    CancelIntent();
                }
            }
        }

        private void ProcessIntent()
        {
            if (stageManager.LiveStage == Stage.PendingSelect)
            {
                if (inFocusPiece == null) return;

                stageManager.LiveStage = Stage.Selected;
                inFocusPiece.ApplyMaterial(selectedMaterial);

                foreach (KeyValuePair<PieceManager, List<Cell>> element in availableMoves)
                {
                    element.Key.EnableInteractions(false);
                }

                if (availableMoves.TryGetValue(inFocusPiece, out List<Cell> cells))
                {
                    foreach (Cell cell in cells)
                    {
                        var preview = GameObject.Instantiate(previewPrefab, setManager.transform.position, Quaternion.identity, setManager.transform);
                        var manager = preview.GetComponentInChildren<PreviewManager>() as PreviewManager;
                        manager.PlaceAtCell(cell);
                        
                        preview.transform.parent = transform;
                        previews.Add(preview);
                    }
                }
            }
            else if (stageManager.LiveStage == Stage.Selected)
            {
                if (inFocusPreview == null) return;

                Cell cell = inFocusPreview.Cell;
                CommitToMove(cell);
            }
        }

        private void CommitToMove(Cell cell)
        {
            DestroyPreviews();
            ResetThemes();
            
            coordReferenceCanvas.TextUI = string.Empty;

            stageManager.LiveStage = Stage.Moving;
            inFocusPiece.GoToCell(cell, moveType, moveStyle);
        }

        private void CancelIntent()
        {
            stageManager.LiveStage = Stage.PendingSelect;
            inFocusPiece?.UseDefaultMaterial();
            inFocusPiece.HideOutline();
            DestroyPreviews();

            foreach (KeyValuePair<PieceManager, List<Cell>> element in availableMoves)
            {
                element.Key.EnableInteractions(true);
            }

            AdjustChessPieceInteractableLayer(activeSet, true);

            if (TryGets.TryGetCoordReference(inFocusPiece.ActiveCell.coord, out string reference))
            {
                coordReferenceCanvas.TextUI = reference;
            }
        }

#region Outcomes
        private void OnCheck(bool hasMoves)
        {
            AudioSource.PlayClipAtPoint(inCheckClip, transform.position, 1.0f);
            
            notificationManager.Text = "Check";
            notificationManager.ShowFor(inCheckNotificationDelay);

            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Check;
        }

        private void OnCheckmate()
        {
            AudioSource.PlayClipAtPoint(checkmateClip, transform.position, 1.0f);

            notificationManager.Text = "Checkmate";
            notificationManager.Show();

            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Checkmate;

            PauseClocks();
            ResetGame();

            // gameOver = true;

            StartCoroutine(ShowNewGameUIAfterDelayCoroutine(0.25f));
        }

        private void OnStalemate()
        {
            AudioSource.PlayClipAtPoint(stalemateClip, transform.position, 1.0f);

            notificationManager.Text = "Stalemate";
            notificationManager.Show();

            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Stalemate;

            PauseClocks();
            ResetGame();
            
            // gameOver = true;

            StartCoroutine(ShowNewGameUIAfterDelayCoroutine(0.25f));
        }
#endregion

        private void DestroyPreviews()
        {
            foreach (GameObject preview in previews)
            {
                Destroy(preview);
            }
            
            inFocusPreview = null;
        }

        private void ResetBoard()
        {
            var enabledPieces = EnabledPieces;
            onHomeEventsPending = enabledPieces.Where(p => !p.IsAddInPiece).Count();

            HideNotifications();

            stageManager.LiveStage = Stage.Resetting;
            
            int addInPieces = 0;
            int originalPieces = 0;

            foreach (PieceManager piece in enabledPieces)
            {
                if (piece.IsAddInPiece)
                {
                    ++addInPieces;
                    piece.EventReceived -= OnEvent;
                    piece.MoveEventReceived -= OnMoveEvent;
                    setManager.RemovePiece(piece);
                }
                else
                {
                    ++originalPieces;
                    piece.Reset();
                    piece.GoHome(moveType, moveStyle);
                }
            }
        }

        private void LowerTable() => coroutine = StartCoroutine(LowerTableCoroutine(adjustTableSpeed));

        private IEnumerator LowerTableCoroutine(float movementSpeed)
        {
            Vector3 targetPosition = new Vector3(board.transform.localPosition.x, lowerYTableBounds, board.transform.localPosition.z);
            
            audioSource.clip = adjustTableHeightClip;
            audioSource.Play();

            while (board.transform.localPosition != targetPosition)
            {
                board.transform.localPosition = Vector3.MoveTowards(board.transform.localPosition, targetPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }

            audioSource.Stop();
        }

        private void RaiseTable() => coroutine = StartCoroutine(RaiseTableCoroutine(adjustTableSpeed));

        private IEnumerator RaiseTableCoroutine(float movementSpeed)
        {
            Vector3 targetPosition = new Vector3(board.transform.localPosition.x, upperYTableBounds, board.transform.localPosition.z);
            
            audioSource.clip = adjustTableHeightClip;
            audioSource.Play();

            while (board.transform.localPosition != targetPosition)
            {
                board.transform.localPosition = Vector3.MoveTowards(board.transform.localPosition, targetPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }

            audioSource.Stop();
        }

        private void ResetTable() => coroutine = StartCoroutine(ResetTableCoroutine(adjustTableSpeed));

        private IEnumerator ResetTableCoroutine(float movementSpeed)
        {
            Vector3 targetPosition = new Vector3(board.transform.localPosition.x, defaultTableYOffset, board.transform.localPosition.z);
            
            audioSource.clip = adjustTableHeightClip;
            audioSource.Play();

            while (board.transform.localPosition != targetPosition)
            {
                board.transform.localPosition = Vector3.MoveTowards(board.transform.localPosition, targetPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }

            audioSource.Stop();
        }

        private void EnableSFX(bool enabled) => playPieceDownClip = enabled;

        private void EnableMusic(bool enabled) => cameraAudioSource.enabled = enabled;

        private void LockTable()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            audioSource.Stop();
        }

        private bool DeferTurnCompletion(PieceManager piece)
        {
            if (piece.Type == PieceType.Pawn && ShouldPawnBePromoted(piece))
            {
                return true;
            }

            return false;
        }

        private void SetPickedPiece(PieceManager piece)
        {
            pickedPiece = piece;

            pieceTransformManager.SetPiece(activeSet, piece.Type);
            pieceTransformManager.LowerAndHidePiece();
        }

#region Pawn Promotion
        private bool ShouldPawnBePromoted(PieceManager piece)
        {
            if (piece.Type == PieceType.Pawn)
            {
                int vector = (activeSet == Set.Light) ? 1 : -1;
                int lastRank = (vector == 1) ? MaxRowIdx : 0;

                if (piece.ActiveCell.coord.y == lastRank)
                {
                    PreparePawnForPromotion(piece);
                    return true;
                }
            }

            return false;
        }

        private void PreparePawnForPromotion(PieceManager piece)
        {
            stageManager.LiveStage = Stage.Promoting;
            AudioSource.PlayClipAtPoint(pawnPromotionClip, transform.position, 1.0f);

            promotionPiece = piece;
            piece.gameObject.SetActive(false);
            pieceTransformManager.ShowAndRaisePiece(activeSet, piece.transform.position);
        }

        private void PromotePiece()
        {
            Cell cell = promotionPiece.ActiveCell;

            if (promotionPiece.IsAddInPiece)
            {
                setManager.RemovePiece(promotionPiece);
            }
            else
            {
                promotionPiece.gameObject.SetActive(true);

                if (setManager.TryReserveSlot(cell.wrapper.manager, out Vector3 localPosition))
                {
                    cell.wrapper.manager.transform.localPosition = localPosition;
                    cell.wrapper.manager.EnableInteractions(false);
                    cell.wrapper.manager.ActiveCell = null;
                    cell.wrapper.manager.ShowMesh();
                    cell.wrapper.manager = null;
                }
            }

            PieceManager promotedPiece = setManager.AddPiece(activeSet, pickedPiece, cell.coord, true);
            cell.wrapper.manager = promotedPiece;

            promotedPiece.ActiveCell = cell;
            promotedPiece.MoveEventReceived += OnMoveEvent;
            promotedPiece.EventReceived += OnEvent;

            matrix[cell.coord.x, cell.coord.y].wrapper.manager = promotedPiece;

            promotionPiece = null;

            CompleteTurn();
        }

        private void OnPawnPromotionEvent(PieceManager piece)
        {
            HidePawnPromotionUI();
            SetPickedPiece(piece);
        }
        
        private void ShowPawnPromotionUI()
        {
            AttachLayerToControllers("Pawn Promotion Layer");
            pawnPromotionManager.ConfigureAndShow(activeSet);
        }

        private void HidePawnPromotionUI()
        {
            pawnPromotionManager.Hide();
            DetachLayerFromContollers("Pawn Promotion Layer");
        }
#endregion

        private void OnPieceTransformComplete(PieceTransformManager.Action action)
        {
            switch (action)
            {
                case PieceTransformManager.Action.Raise:
                    if (stageManager.LiveStage == Stage.Promoting)
                    {
                        if (ShouldAutomate())
                        {
                            PieceManager piece = pawnPromotionManager.ResolvePieceBySet(activeSet, pawnPromotionManager.PickRandomType());
                            SetPickedPiece(piece);
                        }
                        else
                        {
                            ShowPawnPromotionUI();
                        }
                    }
                    break;

                case PieceTransformManager.Action.Lower:
                    if (stageManager.LiveStage == Stage.Promoting)
                    {
                        PromotePiece();
                    }
                    break;
            }
        }

#region New Game
        private void OnNewGameEvent(NewGameManager.Mode mode)
        {
            HideNewGameUI();
            
            switch (mode)
            {
                case NewGameManager.Mode.PVP:
                    lightPlayer = PlayerMode.Human;
                    darkPlayer = PlayerMode.Human;
                    break;

                case NewGameManager.Mode.PVB:
                    lightPlayer = PlayerMode.Human;
                    darkPlayer = PlayerMode.Bot;
                    break;

                case NewGameManager.Mode.BVB:
                    lightPlayer = PlayerMode.Bot;
                    darkPlayer = PlayerMode.Bot;
                    break;
            }

            EvaluateOpeningMove();
        }

        private IEnumerator ShowNewGameUIAfterDelayCoroutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            ShowNewGameUI();
        }

        private void ShowNewGameUI()
        {
            AttachLayerToControllers("UI Input Layer");
            newGameManager.Show();
        }

        private void HideNewGameUI()
        {
            newGameManager.Hide();
            DetachLayerFromContollers("UI Input Layer");
        }
#endregion

        private void AttachLayerToControllers(string layer)
        {
            leftController.AttachLayer(layer);
            rightController.AttachLayer(layer);
        }

        private void DetachLayerFromContollers(string layer)
        {
            leftController.DetachLayer(layer);
            rightController.DetachLayer(layer);
        }

#region Events
        private void OnEvent(PieceManager piece, FocusType focusType)
        {
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    if (piece.Set != activeSet) return;

                    piece.ShowOutline();

                    if (TryGets.TryGetCoordReference(piece.ActiveCell.coord, out string reference))
                    {
                        coordReferenceCanvas.TextUI = reference;
                    }

                    inFocusPiece = piece;
                    break;

                case FocusType.OnFocusLost:
                    if (piece.Set != activeSet) return;

                    if (stageManager.LiveStage == Stage.Selected) return;
                    
                    piece.UseDefaultMaterial();
                    piece.HideOutline();
                    coordReferenceCanvas.TextUI = string.Empty;

                    inFocusPiece = null;
                    break;
            }
        }

        private void OnPreviewEvent(PreviewManager manager, FocusType focusType)
        {
            if (TryGets.TryGetCell(matrix, manager.transform.localPosition, out Cell cell))
            {
                string reference, moveReference;

                switch (focusType)
                {
                    case FocusType.OnFocusGained:
                        if (cell.IsOccupied)
                        {
                            cell.wrapper.manager.HideMesh();
                        }

                        manager.SetCustomMesh(inFocusPiece.Mesh, inFocusPiece.transform.localRotation, inFocusPiece.DefaultMaterial);

                        if (TryGets.TryGetCoordReference(inFocusPiece.ActiveCell.coord, out reference))
                        {
                            if (TryGets.TryGetCoordReference(cell.coord, out moveReference))
                            {
                                coordReferenceCanvas.TextUI = $"{reference} to {moveReference}";
                            }
                        }

                        inFocusPreview = manager;
                        break;

                    case FocusType.OnFocusLost:
                        if (cell.IsOccupied)
                        {
                            cell.wrapper.manager.ShowMesh();
                        }

                        manager.HideMesh();

                        if (TryGets.TryGetCoordReference(inFocusPiece.ActiveCell.coord, out reference))
                        {
                            coordReferenceCanvas.TextUI = reference;
                        }

                        break;
                }
            }
        }

        private void OnMoveEvent(PieceManager piece)
        {
            if (stageManager.LiveStage == Stage.Resetting)
            {
                --onHomeEventsPending;

                if (onHomeEventsPending == 0)
                {
                    var enabledPieces = EnabledPieces;
                    onHomeEventsPending = enabledPieces.Count;

                    foreach (PieceManager thisPiece in enabledPieces)
                    {
                        thisPiece.EnablePhysics(false);
                    }

                    PauseClocks();
                    ResetGame();
                    ShowNewGameUI();
                }
            }
            else if (stageManager.LiveStage == Stage.Moving)
            {
                if (playPieceDownClip)
                {
                    AudioSource.PlayClipAtPoint(pieceDownClip, transform.position, 1.0f);
                }

                PostMatrixUpdate();
                
                if (!DeferTurnCompletion(piece))
                {
                    CompleteTurn();
                }
            }
        }
#endregion
    }
}
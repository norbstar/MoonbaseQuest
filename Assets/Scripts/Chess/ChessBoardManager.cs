using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;
using static Chess.StageManager;

using Chess.Pieces;
using Chess.Button;
using Chess.Preview;

namespace Chess
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(BoardManager))]
    [RequireComponent(typeof(MatrixManager))]
    [RequireComponent(typeof(MoveManager))]
    [RequireComponent(typeof(TimingsManager))]
    public class ChessBoardManager : BaseManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Camera")]
        [SerializeField] new Camera camera;

        [Header("Controllers")]
        [SerializeField] HybridHandController leftController;
        [SerializeField] HybridHandController rightController;

        [Header("Components")]
        [SerializeField] GameObject board;
        public GameObject Board { get { return board; } }
        [SerializeField] ChessBoardSetManager setManager;
        [SerializeField] PieceTransformManager pieceTransformManager;
        [SerializeField] NotificationManager notificationManager;
        [SerializeField] NewGameManager newGameManager;
        [SerializeField] PawnPromotionManager pawnPromotionManager;

        [Header("Audio")]
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
        public Material OutOfScopeMaterial { get { return outOfScopeMaterial; } }
        [SerializeField] Material selectedMaterial;

        [SerializeField] EngagementMode engagementMode;
        public EngagementMode EngagementMode { get { return engagementMode; } }

        [SerializeField] PlayerMode lightPlayer;
        public PlayerMode LightPlayer { get { return lightPlayer; } }
        [SerializeField] PlayerMode darkPlayer;
        public PlayerMode DarkPlayer { get { return darkPlayer; } }

        [SerializeField] GameObject previewPrefab;

        public delegate void MatrixEvent(Cell[,] matrix);
        public static event MatrixEvent MatrixEventReceived;
        
        public ChessBoardSetManager SetManager { get { return setManager; } }
        public StageManager StageManager { get { return stageManager; } }
        public MatrixManager MatrixManager { get { return matrixManager; } }
        public TimingsManager TimingsManager { get { return timingsManager; } }
        public BoardManager BoardManager { get { return boardManager; } }

        public Set ActiveSet { get { return activeSet; } }

        public PieceManager InFocusPiece
        {
            get
            {
                return inFocusPiece;
            }
            
            set
            {
                inFocusPiece = value;
            }
        }

        private TrackingMainCameraManager cameraManager;
        private MoveManager moveManager;
        private MatrixManager matrixManager;
        private TimingsManager timingsManager;
        private BoardManager boardManager;
        private AudioSource cameraAudioSource;
        private int onHomeEventsPending;
        private Set activeSet;
        private PieceManager inFocusPiece;
        private PieceManager promotionPiece;
        private PieceManager pickedPiece;
        private PreviewManager inFocusPreview;
        private StageManager stageManager;
        private List<GameObject> previews;
        private float inCheckNotificationDelay = 1f;
        private Coroutine coroutine;
        private AudioSource audioSource;
        private bool enablePieceDownSFX;
        // private bool gameOver;

        void Awake()
        {
            ResolveDependencies();
            previews = new List<GameObject>();
            stageManager = new StageManager();

            AttachLayerToControllers("Preview Layer");
        }

        private void ResolveDependencies()
        {
            cameraManager = camera.GetComponent<TrackingMainCameraManager>() as TrackingMainCameraManager;

            if (cameraManager != null)
            {
                cameraAudioSource = cameraManager.GetComponent<AudioSource>() as AudioSource;
            }

            matrixManager = GetComponent<MatrixManager>() as MatrixManager;
            moveManager = GetComponent<MoveManager>() as MoveManager;
            timingsManager = GetComponent<TimingsManager>() as TimingsManager;
            boardManager = GetComponent<BoardManager>() as BoardManager; 
            audioSource = GetComponent<AudioSource>() as AudioSource;
        }

        // Start is called before the first frame update
        void Start()
        {
            matrixManager.MapLayout();
            ResetGame();
            StartCoroutine(ShowNewGameUIAfterDelayCoroutine(0.25f));
        }

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
            enablePieceDownSFX = true;
        }

        private void ResetSet() => setManager.Reset();

        private void PauseClocks() => timingsManager.PauseClocks();

        private void PauseActiveClock() => timingsManager.PauseActiveClock();

        private void ResetClocks() => timingsManager.ResetClocks();

        private void PostMatrixUpdate() => MatrixEventReceived?.Invoke(matrixManager.Matrix);

        private void HideNotifications() => notificationManager.Hide();

        private bool IsMoving { get { return stageManager.LiveStage == Stage.Moving; } }

        public void AdjustChessPieceInteractableLayer(Set set, bool enabled)
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

        public void EnableInteractions(Set set, bool enabled)
        {
            List<PieceManager> pieces = (set == Set.Light) ? setManager.EnabledLightPieces : setManager.EnabledDarkPieces;

            foreach (PieceManager piece in pieces)
            {
                if (piece.ActiveCell != null)
                {
                    piece.EnableInteractions(enabled);
                }
            }

            AdjustChessPieceInteractableLayer(set, enabled);
        }

        public Cell ResolveKingCell(Set set) => ResolveKing(set).ActiveCell;

        public PieceManager ResolveKing(Set set)
        {
            if (TryGets.TryGetSingleSetPieceByType(this, set, PieceType.King, out PieceManager piece))
            {
                return piece;
            }

            return null;
        }

        public bool ShouldAutomate()
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

        private void CompleteTurn()
        {
            stageManager.LiveStage = Stage.MoveComplete;

            PauseActiveClock();

            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Nominal;

            activeSet = (activeSet == Set.Light) ? Set.Dark : Set.Light;
            moveManager.EvaluateMove();
        }

        private void ResetThemes()
        {
            foreach (PieceManager piece in setManager.EnabledPieces)
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

                Dictionary<PieceManager, List<Cell>> availableMoves = moveManager.AvailableMoves;

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

        public void CommitToMove(Cell cell)
        {
            DestroyPreviews();
            ResetThemes();
            
            coordReferenceCanvas.TextUI = string.Empty;

            stageManager.LiveStage = Stage.Moving;
            inFocusPiece.GoToCell(cell, moveManager.MoveType, moveManager.MoveStyle);
        }

        private void CancelIntent()
        {
            stageManager.LiveStage = Stage.PendingSelect;
            inFocusPiece?.UseDefaultMaterial();
            inFocusPiece.HideOutline();
            DestroyPreviews();

            Dictionary<PieceManager, List<Cell>> availableMoves = moveManager.AvailableMoves;

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

        public void OnCheck(bool hasMoves)
        {
            AudioSource.PlayClipAtPoint(inCheckClip, transform.position, 1.0f);
            
            notificationManager.Text = "Check";
            notificationManager.ShowFor(inCheckNotificationDelay);

            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Check;
        }

        public void OnCheckmate()
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

        public void OnStalemate()
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
            var enabledPieces = setManager.EnabledPieces;
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
                    piece.GoHome(moveManager.MoveType, moveManager.MoveStyle);
                }
            }
        }

        private void EnableSFX(bool enabled) => enablePieceDownSFX = enabled;

        private void EnableMusic(bool enabled) => cameraAudioSource.enabled = enabled;

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
                int lastRank = (vector == 1) ? MatrixManager.MaxRowIdx : 0;

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

            matrixManager.Matrix[cell.coord.x, cell.coord.y].wrapper.manager = promotedPiece;

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

        private void OnClockExpiredEvent(ClockManager instance)
        {
            Debug.Log($"{instance.name} has expired");
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

            moveManager.EvaluateOpeningMove();
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
        public void OnEvent(PieceManager piece, FocusType focusType)
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
            if (TryGets.TryGetCell(matrixManager.Matrix, manager.transform.localPosition, out Cell cell))
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

        public void OnMoveEvent(PieceManager piece)
        {
            if (stageManager.LiveStage == Stage.Resetting)
            {
                --onHomeEventsPending;

                if (onHomeEventsPending == 0)
                {
                    var enabledPieces = setManager.EnabledPieces;
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
                if (enablePieceDownSFX)
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

        private void OnButtonEvent(ButtonEventManager manager, ButtonEventManager.ButtonId id, ButtonEventType eventType)
        {
            ReassignableButtonEventManager reassignableManager = null;

            if (eventType == ButtonEventType.OnPressed)
            {
                switch (id)
                {
                    case ButtonEventManager.ButtonId.LowerTable:
                        boardManager.MoveTable(BoardManager.MoveType.Lower);
                        break;

                    case ButtonEventManager.ButtonId.RaiseTable:
                        boardManager.MoveTable(BoardManager.MoveType.Raise);
                        break;

                    case ButtonEventManager.ButtonId.ResetTable:
                        boardManager.MoveTable(BoardManager.MoveType.Reset);
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
                        boardManager.LockTable();
                        break;
                }
            }
        }
#endregion
    }
}
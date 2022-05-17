using System.Collections.Generic;
using System.Collections;
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
    public class ChessBoardManager : ChessBoardCoreManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

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

        public StageManager.Stage Stage
        {
            get
            {
                return stageManager.LiveStage;
            }

            set
            {
                stageManager.LiveStage = value;
            }
        }

        [Header("Canvas")]
        [SerializeField] GameObject newGameUI;
        public GameObject NewGameUI { get { return newGameUI; } }

        private int onHomeEventsPending;
        private PieceManager inFocusPiece;
        private PreviewManager inFocusPreview;
        private List<GameObject> previews;
        private float inCheckNotificationDelay = 1f;
        private bool enablePieceDownSFX = true;
        private bool resetRequested;

        public override void Awake()
        {
            base.Awake();
            previews = new List<GameObject>();
        }

        // Start is called before the first frame update
        void Start()
        {
            MapConstruct();
            InitGame();
            RegisterMatrixChanges();

            newGameManager.ShowAfterDelay(0.25f);
        }

        void OnEnable()
        {
            HandController.ActuationEventReceived += OnActuation;
            ButtonEventManager.EventReceived += OnButtonEvent;
            PreviewManager.EventReceived += OnPreviewEvent;
            NewGameManager.EventReceived += OnNewGameEvent;
            PieceTransformManager.CompleteEventReceived += OnPieceTransformEvent;
            ClockManager.OnExpiredEventReceived += OnClockExpirationEvent;
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;
            ButtonEventManager.EventReceived -= OnButtonEvent;
            PreviewManager.EventReceived -= OnPreviewEvent;
            NewGameManager.EventReceived -= OnNewGameEvent;
            PieceTransformManager.CompleteEventReceived -= OnPieceTransformEvent;
            ClockManager.OnExpiredEventReceived -= OnClockExpirationEvent;
        }

        private void MapConstruct() => matrixManager.MapConstruct();

        private void InitGame()
        {
            InitUI();
            InitVariables();
            InitSet();
            InitClocks();
        }
        
        private void InitVariables()
        {
            activeSet = Set.Light;
            // deferAction = gameOver = false;
            resetRequested = false;
            AttachLayerToControllers("Preview Layer");
        }

        public void ConfigureChessPieceInteractableLayer(Set set, bool enabled)
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

            ConfigureChessPieceInteractableLayer(set, enabled);
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

        public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
        {
            if ((Stage == Stage.Evaluating) || (inFocusPiece == null)) return;

            if (actuation.HasFlag(Actuation.Button_AX))
            {
                ProcessIntent();
            }
            else if (actuation.HasFlag(Actuation.Button_BY))
            {
                if (Stage == Stage.Selected)
                {
                    CancelIntent();
                }
            }
        }

        private void DestroyPreviews()
        {
            foreach (GameObject preview in previews)
            {
                Destroy(preview);
            }
            
            inFocusPreview = null;
        }

        private IEnumerator TakePieceAtCellCoroutine(Cell cell)
        {
            PieceManager piece = cell.wrapper.manager;

            if (animatePieceWhenTaken)
            {
                piece.AnimateOut();
                yield return new WaitForSeconds(0.25f);
            }

            if (piece.IsAddInPiece)
            {
                setManager.RemovePiece(piece);
            }
            else
            {
                piece.MoveToSlot(cell);

                if (animatePieceWhenTaken)
                {
                    piece.AnimateIn();
                }
            }
        }

        public void CommitToMove(Cell cell) => StartCoroutine(CommitToMoveCoroutine(cell));

        private IEnumerator CommitToMoveCoroutine(Cell cell)
        {
            DestroyPreviews();
            matrixManager.ResetThemes();
            
            coordReferenceCanvas.TextUI = string.Empty;
            
            if (cell.IsOccupied)
            {
                yield return StartCoroutine(TakePieceAtCellCoroutine(cell));
            }

            Stage = Stage.Moving;
            inFocusPiece.GoToCell(cell, moveManager.MoveType, moveManager.MoveStyle);
        }

        private void ProcessIntent()
        {
            if (Stage == Stage.PendingSelect)
            {
                if (inFocusPiece == null) return;

                Stage = Stage.Selected;
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
            else if (Stage == Stage.Selected)
            {
                if (inFocusPreview == null) return;

                Cell cell = inFocusPreview.Cell;
                CommitToMove(cell);
            }
        }

        private void CancelIntent()
        {
            Stage = Stage.PendingSelect;
            inFocusPiece?.UseDefaultMaterial();
            inFocusPiece.HideOutline();
            DestroyPreviews();

            Dictionary<PieceManager, List<Cell>> availableMoves = moveManager.AvailableMoves;

            foreach (KeyValuePair<PieceManager, List<Cell>> element in availableMoves)
            {
                element.Key.EnableInteractions(true);
            }

            ConfigureChessPieceInteractableLayer(activeSet, true);

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

            PieceManager manager = matrixManager.ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Check;
        }

        public void OnCheckmate()
        {
            AudioSource.PlayClipAtPoint(checkmateClip, transform.position, 1.0f);

            notificationManager.Text = "Checkmate";
            notificationManager.Show();

            PieceManager manager = matrixManager.ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Checkmate;

            PauseClocks();
            RegisterMatrixChanges();
            SaveGameSession();
            newGameManager.ShowAfterDelay(0.25f);
        }

        public void OnStalemate()
        {
            AudioSource.PlayClipAtPoint(stalemateClip, transform.position, 1.0f);

            notificationManager.Text = "Stalemate";
            notificationManager.Show();

            PieceManager manager = matrixManager.ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Stalemate;

            PauseClocks();
            RegisterMatrixChanges();
            SaveGameSession();
            newGameManager.ShowAfterDelay(0.25f);
        }

        private void SaveGameSession() => gameSessionManager.SaveAsset("session", true);

        private void ClearGameSession() => gameSessionManager.Clear();

        private void Reset()
        {
            var enabledPieces = setManager.EnabledPieces;
            onHomeEventsPending = enabledPieces.Where(p => !p.IsAddInPiece).Count();

            notificationManager.Hide();

            Stage = Stage.Resetting;
            
            foreach (PieceManager piece in enabledPieces)
            {
                if (piece.IsAddInPiece)
                {
                    piece.EventReceived -= OnPieceEvent;
                    piece.MoveEventReceived -= OnMoveEvent;
                    setManager.RemovePiece(piece);
                }
                else
                {
                    piece.Reset();
                    piece.GoHome(moveManager.MoveType, moveManager.MoveStyle);
                }
            }
        }

        private void EnableSFX(ReassignableButtonEventManager manager, bool enabled)
        {
            enablePieceDownSFX = enabled;
            
            if (enabled)
            {
                manager.Id = ButtonEventManager.ButtonId.SFXOff;
                manager.Text = "Off";
            }
            else
            {
                manager.Id = ButtonEventManager.ButtonId.SFXOn;
                manager.Text = "On";
            }
        }

        private void EnableMusic(ReassignableButtonEventManager manager, bool enabled)
        {
            cameraAudioSource.enabled = enabled;

            if (enabled)
            {
                manager.Id = ButtonEventManager.ButtonId.MusicOff;
                manager.Text = "Off";
            }
            else
            {
                manager.Id = ButtonEventManager.ButtonId.MusicOn;
                manager.Text = "On";
            }
        }

        private void OnPieceTransformEvent(PieceTransformManager.Action action)
        {
            switch (action)
            {
                case PieceTransformManager.Action.Raise:
                    if (Stage == Stage.Promoting)
                    {
                        if (ShouldAutomate())
                        {
                            PieceManager piece = pawnPromotionManager.ResolvePieceBySet(activeSet, pawnPromotionManager.PickRandomType());
                            pawnPromotionManager.SetPickedPiece(activeSet, piece);
                        }
                        else
                        {
                            pawnPromotionManager.Show(activeSet);
                        }
                    }
                    break;

                case PieceTransformManager.Action.Lower:
                    if (Stage == Stage.Promoting)
                    {
                        pawnPromotionManager.PromotePiece(activeSet);
                    }
                    break;
            }
        }

        public void NewPvPGame() => newGameManager.OnEvent(NewGameManager.Mode.PVP);
        
        public void NewPvBGame() => newGameManager.OnEvent(NewGameManager.Mode.PVB);

        public void NewBvBGame() => newGameManager.OnEvent(NewGameManager.Mode.BVB);

        private void OnNewGameEvent(NewGameManager.Mode mode, PlayerMode lightPlayer, PlayerMode darkPlayer)
        {
            this.lightPlayer = lightPlayer;
            this.darkPlayer = darkPlayer;

            Reset();
        }

        public void OnPieceEvent(PieceManager piece, FocusType focusType)
        {
            if (piece.Set != activeSet) return;

            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    piece.ShowOutline();

                    if (TryGets.TryGetCoordReference(piece.ActiveCell.coord, out string reference))
                    {
                        coordReferenceCanvas.TextUI = reference;
                    }

                    inFocusPiece = piece;
                    break;

                case FocusType.OnFocusLost:
                    if (Stage == Stage.Selected) return;
                    
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
                PieceManager piece = cell.wrapper.manager;
                string reference, moveReference;

                switch (focusType)
                {
                    case FocusType.OnFocusGained:
                        if (cell.IsOccupied)
                        {
                            piece.HideMesh();
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
                            piece.ShowMesh();
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

        private bool DeferTurnCompletion(PieceManager piece)
        {
            if (piece.Type == PieceType.Pawn && pawnPromotionManager.ShouldPawnBePromoted(activeSet, piece))
            {
                pawnPromotionManager.PreparePawnForPromotion(activeSet, piece);
                return true;
            }

            return false;
        }

        public void CompleteTurn()
        {
            Stage = Stage.MoveComplete;

            PauseActiveClock();
            RegisterMatrixChanges();

            PieceManager manager = matrixManager.ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Nominal;

            if (resetRequested)
            {
                Reset();
            }
            else
            {
                activeSet = (activeSet == Set.Light) ? Set.Dark : Set.Light;
                moveManager.EvaluateMove();
            }
        }

        public void OnMoveEvent(PieceManager piece)
        {
            if (Stage == Stage.Resetting)
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

                    InitGame();
                    ClearGameSession();
                    RegisterMatrixChanges();
                    moveManager.EvaluateOpeningMove();
                }
            }
            else if (Stage == Stage.Moving)
            {
                if (enablePieceDownSFX)
                {
                    AudioSource.PlayClipAtPoint(pieceDownClip, transform.position, 1.0f);
                }
                
                if (!DeferTurnCompletion(piece))
                {
                    CompleteTurn();
                }
            }
        }

        private void OnClockExpirationEvent(ClockManager manager)
        {
            Debug.Log($"{manager.name} has expired");
        }

        public void LowerBoardLevel() => boardManager.Move(BoardManager.MoveType.Lower);

        public void RaiseBoardLevel() => boardManager.Move(BoardManager.MoveType.Raise);

        public void ResetBoardLevel() => boardManager.Move(BoardManager.MoveType.Reset);

        public void ResetBoard() => resetRequested = true;

        private void OnButtonEvent(ButtonEventManager manager, ButtonEventManager.ButtonId id, ButtonEventType eventType)
        {
            if (eventType == ButtonEventType.OnPressed)
            {
                switch (id)
                {
                    case ButtonEventManager.ButtonId.LowerBoardLevel:
                        LowerBoardLevel();
                        break;

                    case ButtonEventManager.ButtonId.RaiseBoardLevel:
                        RaiseBoardLevel();
                        break;

                    case ButtonEventManager.ButtonId.ResetBoardLevel:
                        ResetBoardLevel();
                        break;

                    case ButtonEventManager.ButtonId.ResetBoard:
                        ResetBoard();
                        break;

                    case ButtonEventManager.ButtonId.SFXOff:
                        EnableSFX((ReassignableButtonEventManager) manager, false);
                        break;

                    case ButtonEventManager.ButtonId.SFXOn:
                        EnableSFX((ReassignableButtonEventManager) manager, true);
                        break;

                    case ButtonEventManager.ButtonId.MusicOff:
                        EnableMusic((ReassignableButtonEventManager) manager,false);
                        break;

                    case ButtonEventManager.ButtonId.MusicOn:
                        EnableMusic((ReassignableButtonEventManager) manager,true);
                        break;
                }
            }
            else if (eventType == ButtonEventType.OnReleased)
            {
                switch (id)
                {
                    case ButtonEventManager.ButtonId.LowerBoardLevel:
                    case ButtonEventManager.ButtonId.RaiseBoardLevel:
                        boardManager.LockTable();
                        break;
                }
            }
        }
    }
}
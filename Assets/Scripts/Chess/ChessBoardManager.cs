using System;
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

        private int onHomeEventsPending;
        private PieceManager inFocusPiece;
        private PreviewManager inFocusPreview;
        private List<GameObject> previews;
        private float inCheckNotificationDelay = 1f;
        private Coroutine coroutine;
        private bool enablePieceDownSFX;
        // private bool gameOver;

        public override void Awake()
        {
            base.Awake();

            previews = new List<GameObject>();
            AttachLayerToControllers("Preview Layer");
        }

        // Start is called before the first frame update
        void Start()
        {
            matrixManager.MapLayout();
            ResetGame();
            newGameManager.ShowAfterDelay(0.25f);
        }

        void OnEnable()
        {
            HandController.ActuationEventReceived += OnActuation;
            ButtonEventManager.EventReceived += OnButtonEvent;
            PreviewManager.EventReceived += OnPreviewEvent;
            NewGameManager.EventReceived += OnNewGameEvent;
            PieceTransformManager.CompleteEventReceived += OnPieceTransformComplete;
            ClockManager.OnExpiredEventReceived += OnClockExpiredEvent;
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;
            ButtonEventManager.EventReceived -= OnButtonEvent;
            PreviewManager.EventReceived -= OnPreviewEvent;
            NewGameManager.EventReceived -= OnNewGameEvent;
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

        private void HideNotifications() => notificationManager.Hide();

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

        public void CompleteTurn()
        {
            Stage = Stage.MoveComplete;

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

        public void CommitToMove(Cell cell)
        {
            DestroyPreviews();
            ResetThemes();
            
            coordReferenceCanvas.TextUI = string.Empty;

            Stage = Stage.Moving;
            inFocusPiece.GoToCell(cell, moveManager.MoveType, moveManager.MoveStyle);
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

            newGameManager.ShowAfterDelay(0.25f);
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

            newGameManager.ShowAfterDelay(0.25f);
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

            Stage = Stage.Resetting;
            
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
            if (piece.Type == PieceType.Pawn && pawnPromotionManager.ShouldPawnBePromoted(activeSet, piece))
            {
                pawnPromotionManager.PreparePawnForPromotion(activeSet, piece);
                return true;
            }

            return false;
        }

        private void OnPieceTransformComplete(PieceTransformManager.Action action)
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

        private void OnClockExpiredEvent(ClockManager instance)
        {
            Debug.Log($"{instance.name} has expired");
        }

        private void OnNewGameEvent(NewGameManager.Mode mode)
        {
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

                    PauseClocks();
                    ResetGame();
                    newGameManager.Show();
                }
            }
            else if (Stage == Stage.Moving)
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
    }
}
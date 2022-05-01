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

        [Header("Camera")]
        [SerializeField] new Camera camera;

        [Header("Controllers")]
        [SerializeField] HybridHandController leftController;
        [SerializeField] HybridHandController rightController;

        [Header("Components")]
        [SerializeField] GameObject board;
        [SerializeField] ChessBoardSetManager setManager;
        public ChessBoardSetManager SetManager { get { return setManager; } }

        [SerializeField] NotificationManager notificationManager;
        [SerializeField] PiecePickerManager piecePickerManager;

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

        [Range(0.1f, 25f)]
        [SerializeField] float pieceMoveSpeed = 5f;

        [Header("Materials")]
        [SerializeField] Material outOfScopeMaterial;
        [SerializeField] Material inFocusMaterial;
        [SerializeField] Material selectedMaterial;
        [SerializeField] Material pawnPromotionMaterial;

        [Header("Config")]
        [SerializeField] PlayMode playMode;
        public PlayMode PlayMode { get { return playMode; } }

        [SerializeField] MoveStyle moveStyle;
        public MoveStyle MoveStyle { get { return moveStyle; } }

        [SerializeField] MoveType moveType;
        public MoveType MoveType { get { return moveType; } }

        [SerializeField] EngagementMode engagementMode;
        public EngagementMode EngagementMode { get { return engagementMode; } }

        [SerializeField] OppositionMode oppositionMode;
        public OppositionMode OppositionMode { get { return oppositionMode; } }

        [SerializeField] GameObject previewPrefab;
        [SerializeField] float adjustTableSpeed = 0.5f;

        public delegate void MatrixEvent(Cell[,] matrix);
        public static event MatrixEvent MatrixEventReceived;

        public static int MatrixRows = 8;
        public static int MatrixColumns = 8;

        private TrackingMainCameraManager cameraManager;
        private AudioSource cameraAudioSource;
        private Cell[,] matrix;
        private int onHomeEventsPending;
        private Set activeSet;
        private PieceManager inFocusPiece;
        private PieceManager promotionPiece;
        private PreviewManager inFocusPreview;
        private StageManager stageManager;
        private Dictionary<PieceManager, List<Cell>> availableMoves;
        private int maxRowIdx = MatrixRows - 1;
        private int maxColumnIdx = MatrixColumns - 1;
        private List<GameObject> previews;
        private float defaultTableYOffset = 0f;
        private float lowerYTableBounds = -0.25f;
        private float upperYTableBounds = 0.25f;
        private float inCheckNotificationDelay = 1f;
        private Coroutine coroutine;
        private AudioSource audioSource;

        void Awake()
        {
            ResolveDependencies();

            matrix = new Cell[MatrixColumns, MatrixRows];
            previews = new List<GameObject>();
            availableMoves = new Dictionary<PieceManager, List<Cell>>();
            stageManager = new StageManager();
            
            cameraManager.IncludeInteractableLayer("Preview Layer");
            leftController.IncludeInteractableLayer("Preview Layer");
            rightController.IncludeInteractableLayer("Preview Layer");
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
            MapMatrix();
            MapPieces();
            InitGame();
        }

        void OnEnable()
        {
            HandController.ActuationEventReceived += OnActuation;
            ButtonEventManager.EventReceived += OnButtonEvent;
            PreviewManager.EventReceived += OnPreviewEvent;
            PiecePickerManager.EventReceived += OnPiecePickerEvent;
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;
            ButtonEventManager.EventReceived -= OnButtonEvent;
            PreviewManager.EventReceived -= OnPreviewEvent;
            PiecePickerManager.EventReceived -= OnPiecePickerEvent;
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
                        ResetBoard();
                        break;

                    case ButtonEventManager.ButtonId.MusicOff:
                        EnableAudio(false);

                        reassignableManager = ((ReassignableButtonEventManager) manager);
                        reassignableManager.Id = ButtonEventManager.ButtonId.MusicOn;
                        reassignableManager.Text = "On";
                        break;

                    case ButtonEventManager.ButtonId.MusicOn:
                        EnableAudio(true);

                        reassignableManager = ((ReassignableButtonEventManager) manager);
                        reassignableManager.Id = ButtonEventManager.ButtonId.MusicOff;
                        reassignableManager.Text = "Off";
                        break;

                    case ButtonEventManager.ButtonId.BotOff:
                        EnableBot(false);

                        reassignableManager = ((ReassignableButtonEventManager) manager);
                        reassignableManager.Id = ButtonEventManager.ButtonId.BotOn;
                        reassignableManager.Text = "On";
                        break;

                    case ButtonEventManager.ButtonId.BotOn:
                        EnableBot(true);

                        reassignableManager = ((ReassignableButtonEventManager) manager);
                        reassignableManager.Id = ButtonEventManager.ButtonId.BotOff;
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

        private List<PieceManager> EnabledPieces { get { return setManager.AllPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        private List<PieceManager> ActiveEnabledPieces { get { return setManager.AllPieces().Where(p => p.isActiveAndEnabled && (p.ActiveCell != null)).ToList(); } }

        private List<PieceManager> EnabledLightPieces { get { return setManager.LightPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        private List<PieceManager> ActiveEnabledLightPieces { get { return setManager.LightPieces().Where(p => p.isActiveAndEnabled && (p.ActiveCell != null)).ToList(); } }

        private List<PieceManager> EnabledDarkPieces { get { return setManager.DarkPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        private List<PieceManager> ActiveEnabledDarkPieces { get { return setManager.DarkPieces().Where(p => p.isActiveAndEnabled && (p.ActiveCell != null)).ToList(); } }

        private void InitGame() => ResetGame();

        private void ResetGame()
        {
            ResetUI();
            ResetGameState();
            ResetSet();
            ManageTurn();
        }

        private void ResetGameState()
        {
            activeSet = Set.Light;
        }

        private void ResetSet() => setManager.Reset();

        private void HideNotifications() => notificationManager.Hide();

        private void ResetUI() => coordReferenceCanvas.TextUI = String.Empty;

        private void ManageTurn()
        {
            MatrixEventReceived?.Invoke(matrix);

            stageManager.LiveStage = Stage.Evaluating;
            availableMoves.Clear();
            inFocusPiece = null;
            
            if (playMode == PlayMode.RuleBased)
            {
                EnableInteractions(Set.Light, false);
                EnableInteractions(Set.Dark, false);
            }

            bool inCheck = IsKingInCheck(activeSet, matrix);
            bool hasMoves = CalculateMoves();

            if (hasMoves)
            {
                if (inCheck)
                {
                    OnInCheck(hasMoves);
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
                cameraManager.IncludeInteractableLayer(layer);
                leftController.IncludeInteractableLayer(layer);
                rightController.IncludeInteractableLayer(layer);
            }
            else
            {
                cameraManager.ExcludeInteractableLayer(layer);
                leftController.ExcludeInteractableLayer(layer);
                rightController.ExcludeInteractableLayer(layer);
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
            if (TryResolveKingCell(matrix, set, out Cell kingCell))
            {
                List<PieceManager> opposingPieces = GetSetPieces((activeSet == Set.Light) ? Set.Dark : Set.Light);

                TryGets.TryGetCoordReference(kingCell.coord, out string kingReference);

                foreach (PieceManager opposingPiece in opposingPieces)
                {
                    TryGets.TryGetCoordReference(opposingPiece.ActiveCell.coord, out string reference);

                    if (opposingPiece.CanMoveTo(matrix, kingCell))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryResolveKingCell(Cell[,] matrix, Set set, out Cell cell)
        {
            for (int y = 0 ; y <= maxRowIdx ; y++)
            {
                for (int x = 0 ; x <= maxColumnIdx ; x++)
                {
                    Cell thisCell = matrix[x, y];
                    if (!thisCell.IsOccupied) continue;
                    PieceManager piece = thisCell.wrapper.manager;
                    
                    if (piece.Set == set && piece.Type == PieceType.King)
                    {
                        cell = thisCell;
                        return true;
                    }
                }
            }

            cell = default(Cell);
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
            
            if (playMode == PlayMode.RuleBased)
            {
                AdjustChessPieceInteractableLayer(Set.Light, (activeSet == Set.Light));
                AdjustChessPieceInteractableLayer(Set.Dark, (activeSet == Set.Dark));
            }

            return hasAnyMoves;
        }

        private Cell[,] ProjectMatrix(Cell cell, Cell targetCell)
        {
            Cell[,] clone = CloneMatrix();

            for (int y = 0 ; y <= maxRowIdx ; y++)
            {
                for (int x = 0 ; x <= maxColumnIdx ; x++)
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
            if (TryGetSingleSetPieceByType(set, PieceType.King, out PieceManager piece))
            {
                return piece;
            }

            return null;
        }

        private bool ShouldAutomate()
        {
            return (activeSet == Set.Dark) && (oppositionMode != OppositionMode.None);
        }

        private void AutomateMove()
        {
            switch (oppositionMode)
            {
                case OppositionMode.DumbBot:
                    AutomateDumbMove();
                    break;
            }
        }

        private void AutomateDumbMove()
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
            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Nominal;

            activeSet = (activeSet == Set.Light) ? Set.Dark : Set.Light;
            ManageTurn();
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

                if (playMode == PlayMode.RuleBased)
                {
                    foreach (KeyValuePair<PieceManager, List<Cell>> element in availableMoves)
                    {
                        element.Key.EnableInteractions(false);
                    }
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

        public float PieceRotationSpeed { get { return pieceRotationSpeed; } }
        public float PieceMoveSpeed { get { return pieceMoveSpeed; } }

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

            if (playMode == PlayMode.RuleBased)
            {
                foreach (KeyValuePair<PieceManager, List<Cell>> element in availableMoves)
                {
                    element.Key.EnableInteractions(true);
                }

                AdjustChessPieceInteractableLayer(activeSet, true);
            }

            if (TryGets.TryGetCoordReference(inFocusPiece.ActiveCell.coord, out string reference))
            {
                coordReferenceCanvas.TextUI = reference;
            }
        }

        private void OnInCheck(bool hasMoves)
        {
            AudioSource.PlayClipAtPoint(inCheckClip, transform.position, 1.0f);
            
            notificationManager.Text = "Check";
            notificationManager.ShowFor(inCheckNotificationDelay);

            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.InCheck;

            Log($"{Time.time} {gameObject.name} {className} OnInCheck:Set : {activeSet} Has Moves :{hasMoves}");
        }

        private void OnCheckmate()
        {
            AudioSource.PlayClipAtPoint(checkmateClip, transform.position, 1.0f);

            notificationManager.Text = "Checkmate";
            notificationManager.Show();

            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Checkmate;

            Set winner = (activeSet == Set.Light) ? Set.Dark : Set.Light;

            Log($"{Time.time} {gameObject.name} {className} OnCheckmate:{activeSet} has LOST, {winner} WINS");
        }

        private void OnStalemate()
        {
            AudioSource.PlayClipAtPoint(stalemateClip, transform.position, 1.0f);

            notificationManager.Text = "Stalemate";
            notificationManager.Show();

            PieceManager manager = ResolveKing(activeSet);
            ((KingManager) manager).KingState = KingManager.State.Stalemate;

            Log($"{Time.time} {gameObject.name} {className} OnStalemate:Nobody WINS");
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
            var enabledPieces = EnabledPieces;
            onHomeEventsPending = enabledPieces.Where(p => !p.IsAddInPiece).Count();

            HideNotifications();

            stageManager.LiveStage = Stage.Resetting;
            
            foreach (PieceManager piece in enabledPieces)
            {
                if (piece.IsAddInPiece)
                {
                    piece.EventReceived -= OnEvent;
                    piece.MoveEventReceived -= OnMoveEvent;
                    setManager.RemovePiece(piece);
                }
                else
                {
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

        private void EnableAudio(bool enabled) => cameraAudioSource.enabled = enabled;

        private void EnableBot(bool enabled) => oppositionMode = (enabled) ? OppositionMode.DumbBot : OppositionMode.None;

        private void LockTable()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            audioSource.Stop();
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

                    ResetGame();
                }
            }
            else if (stageManager.LiveStage == Stage.Moving)
            {
                AudioSource.PlayClipAtPoint(pieceDownClip, transform.position, 1.0f);
                MatrixEventReceived?.Invoke(matrix);
                
                bool completeTurn = true;

                if (piece.Type == PieceType.Pawn)
                {
                    int vector = (activeSet == Set.Light) ? 1 : -1;
                    int lastRank = (vector == 1) ? maxRowIdx : 0;

                    if (piece.ActiveCell.coord.y == lastRank)
                    {
                        completeTurn = false;
                        PromotePawn(piece);
                    }
                }

                if (completeTurn)
                {
                    CompleteTurn();
                }
            }
        }

        private void PromotePawn(PieceManager piece)
        {
            AudioSource.PlayClipAtPoint(pawnPromotionClip, transform.position, 1.0f);
            piece.ApplyMaterial(pawnPromotionMaterial);

            promotionPiece = piece;

            leftController.IncludeInteractableLayer("Piece Picker Layer");
            rightController.IncludeInteractableLayer("Piece Picker Layer");

            piecePickerManager.gameObject.SetActive(true);
        }

        private void OnEvent(PieceManager piece, FocusType focusType)
        {
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    if ((playMode == PlayMode.RuleBased) && (piece.Set != activeSet)) return;

                    piece.ShowOutline();

                    if (TryGets.TryGetCoordReference(piece.ActiveCell.coord, out string reference))
                    {
                        coordReferenceCanvas.TextUI = reference;
                    }

                    inFocusPiece = piece;
                    break;

                case FocusType.OnFocusLost:
                    if ((playMode == PlayMode.RuleBased) && (piece.Set != activeSet)) return;

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
            if (TryGetCell(manager.transform.localPosition, out Cell cell))
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

        private void OnPiecePickerEvent(PieceManager piece)
        {
            piecePickerManager.gameObject.SetActive(false);
            
            leftController.ExcludeInteractableLayer("Piece Picker Layer");
            rightController.ExcludeInteractableLayer("Piece Picker Layer");

            Cell cell = promotionPiece.ActiveCell;

            if (setManager.TryReserveSlot(cell.wrapper.manager, out Vector3 localPosition))
            {
                cell.wrapper.manager.transform.localPosition = localPosition;
                cell.wrapper.manager.EnableInteractions(false);
                cell.wrapper.manager.ActiveCell = null;
                cell.wrapper.manager.ShowMesh();
                cell.wrapper.manager = null;
            }

            PieceManager promotedPiece = setManager.AddPiece(activeSet, piece, cell.coord, true);

            cell.wrapper.manager = promotedPiece;

            promotedPiece.ActiveCell = cell;
            promotedPiece.MoveEventReceived += OnMoveEvent;
            promotedPiece.EventReceived += OnEvent;

            matrix[cell.coord.x, cell.coord.y].wrapper.manager = promotedPiece;

            promotionPiece = null;

            CompleteTurn();
        }

#region Matrix
    private void MapMatrix()
    {
        for (int y = 0 ; y <= maxRowIdx ; y++)
        {
            for (int x = 0 ; x <= maxColumnIdx ; x++)
            {
                Coord coord = new Coord
                {
                    x = x,
                    y = y
                };

                if (TryGetCoordToPosition(coord, out Vector3 localPosition))
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

    public void ReportCells(Cell[,] matrix, bool includeEmptyCells = false)
    {
        List<Cell> cells = ExtractCells(matrix);
        ReportCells(cells, includeEmptyCells);
    }

    public void ReportCells(List<Cell> cells, bool includeEmptyCells = false)
    {
        foreach (Cell cell in cells)
        {
            if (cell != null)
            {
                TryGets.TryGetCoordReference(cell.coord, out string reference);

                if (cell.IsOccupied)
                {
                    Debug.Log($"ReportMatrix Piece : {cell.wrapper.manager.name} Coord : [{cell.coord.x}, {cell.coord.y}] Reference : {reference} Position : [{cell.localPosition.x}, {cell.localPosition.y}, {cell.localPosition.z}]");
                }
                else if (includeEmptyCells)
                {
                    Debug.Log($"ReportMatrix EMPTY Coord : [{cell.coord.x} {cell.coord.y}] Reference : {reference} Position : [{cell.localPosition.x}, {cell.localPosition.y}, {cell.localPosition.z}]");
                }
            }
        }
    }

    public List<Cell> ExtractCells(Cell[,] matrix)
    {
        List<Cell> cells = new List<Cell>();

        for (int y = 0 ; y <= maxRowIdx ; y++)
        {
            for (int x = 0 ; x <= maxColumnIdx ; x++)
            {
                cells.Add(matrix[x, y]);
            }
        }

        return cells;
    }

    public List<Cell> AllCells
    {
        get
        {
            return ExtractCells(matrix);
        }
    }

    public List<Coord> AllCoords
    {
        get
        {
            List<Coord> coords = new List<Coord>();

            for (int y = 0 ; y <= maxRowIdx ; y++)
            {
                for (int x = 0 ; x <= maxColumnIdx ; x++)
                {
                    coords.Add(new Coord
                    {
                        x = x,
                        y = y
                    });
                }
            }

            return coords;
        }
    }
#endregion

#region Pieces
    private void MapPieces()
    {
        foreach (PieceManager pieceManager in EnabledPieces)
        {
            if ((pieceManager.isActiveAndEnabled) && (TryGetPieceToCell(pieceManager, out Cell cell)))
            {
                pieceManager.HomeCell = cell;
                pieceManager.MoveEventReceived += OnMoveEvent;
                pieceManager.EventReceived += OnEvent;

                matrix[cell.coord.x, cell.coord.y].wrapper.manager = pieceManager;
            }
        }
    }

    public void ReportPieces()
    {
        foreach (PieceManager piece in EnabledPieces)
        {
            TryGets.TryGetCoordReference(piece.ActiveCell.coord, out string reference);

            Debug.Log($"ReportPieces {piece.name} Cell : {reference} Coord : [{piece.ActiveCell.coord.x}, {piece.ActiveCell.coord.y}] Position : [{piece.ActiveCell.localPosition.x}, {piece.ActiveCell.localPosition.y}, {piece.ActiveCell.localPosition.z}]");
        }
    }
#endregion

#region TryGets
    public List<PieceManager> GetSetPieces(Set set)
    {
        return (set == Set.Light) ? ActiveEnabledLightPieces : ActiveEnabledDarkPieces;
    }

    public bool TryGetSingleSetPieceByType(Set set, PieceType type, out PieceManager pieceManager)
    {
        List<PieceManager> activeEnabledPieces = GetSetPieces(set);
        List<Cell> matchingCells = activeEnabledPieces.Where(p => p.Type == type).Select(p => p.ActiveCell).ToList();

        if (matchingCells.Count > 0)
        {
            pieceManager = matchingCells.First().wrapper.manager;
            return true;
        }
        
        pieceManager = default(PieceManager);
        return false;
    }


    public bool TryGetSetPiecesByType(Set set, PieceType type, out List<PieceManager> pieces)
    {
        List<PieceManager> activeEnabledPieces = GetSetPieces(set);
        pieces = activeEnabledPieces.Where(p => p.Type == type).ToList();

        return (pieces.Count > 0);
    }

    public bool TryGetPiecesByType(PieceType type, out List<PieceManager> pieces)
    {
        pieces = ActiveEnabledPieces.Where(p => p.Type == type).ToList();
        return (pieces.Count > 0);
    }

    public bool TryGetPieceToCell(PieceManager piece, out Cell cell)
    {
        var localPosition = ChessMath.RoundPosition(piece.transform.localPosition);
        
        if (TryGetCoord(localPosition, out Coord coord))
        {
            cell = matrix[coord.x, coord.y];
            return true;
        }

        cell = default(Cell);
        return false;
    }

    public bool TryGetCoord(Vector3 localPosition, out Coord coord)
    {
        var normX = ChessMath.Normalize(localPosition.x, -0.35f, 0.35f);
        int x = (int) Mathf.Round(maxColumnIdx * (float) normX);

        var normZ = ChessMath.Normalize(localPosition.z, -0.35f, 0.35f);
        int z = (int) Mathf.Round(maxRowIdx * (float) normZ);

        if ((x >= 0 && x <= maxColumnIdx) && (z >= 0 && z <= maxRowIdx))
        {
            coord = new Coord
            {
                x = x,
                y = z
            };

            return true;
        }

        coord = default(Coord);
        return false;
    }

    public bool TryGetCoordToPosition(Coord coord, out Vector3 localPosition)
    {
        Vector3 surface = setManager.transform.localPosition;

        if ((coord.x >= 0 && coord.x <= maxColumnIdx) && (coord.y >= 0 && coord.y <= maxRowIdx))
        {
            float x = ChessMath.RoundFloat(-0.35f + (coord.x * 0.1f));
            float y = ChessMath.RoundFloat(-0.35f + (coord.y * 0.1f));

            localPosition = new Vector3(x, surface.y, y);
            return true;
        }

        localPosition = default(Vector3);
        return false;
    }

    public bool TryGetCell(Vector3 localPosition, out Cell cell)
    {
        for (int y = 0 ; y <= maxRowIdx ; y++)
        {
            for (int x = 0 ; x <= maxColumnIdx ; x++)
            {
                cell = matrix[x, y];
                Vector3 cellPosition = ChessMath.RoundVector3(cell.localPosition);
                Vector3 queryPosition = ChessMath.RoundVector3(localPosition);

                if ((cellPosition.x == queryPosition.x) && (cellPosition.z == queryPosition.z))
                {
                    return true;
                }
            }
        }

        cell = default(Cell);
        return false;
    }

    public bool TryGetPiecesAlongVector(Cell[,] projectedMatrix, Cell origin, Vector2 vector, out List<PieceManager> pieceManagers)
    {
        pieceManagers = new List<PieceManager>();

        int x = origin.coord.x;
        int y = origin.coord.y;

        do
        {
            x += (int) vector.x;
            y += (int) vector.y;

            if ((x >= 0 && x <= maxColumnIdx) && (y >= 0 && y <= maxRowIdx))
            {
                Cell cell = projectedMatrix[x, y];

                if (cell.wrapper.manager != null)
                {
                    pieceManagers.Add(cell.wrapper.manager);
                }
            }
        } while ((x >= 0 && x <= maxColumnIdx) && (y >= 0 && y <= maxRowIdx));

        return false;
    }
#endregion
    }
}
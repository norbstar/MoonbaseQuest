using System;
using System.Collections.Generic;
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
    public class ChessBoardManager : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] new Camera camera;

        [Header("Components")]
        [SerializeField] ChessBoardSetManager setManger;
        public ChessBoardSetManager SetManager { get { return setManger; } }

        [SerializeField] CoordReferenceCanvas coordReferenceCanvas;
        [SerializeField] ButtonEventManager resetButton;

        [Header("Pieces")]
        [SerializeField] float rotationSpeed = 25f;
        [SerializeField] float movementSpeed = 25f;
        [SerializeField] Material outOfScopeMaterial;

        [Header("Config")]
        [SerializeField] PlayMode playMode;
        public PlayMode PlayMode { get { return playMode; } }

        [SerializeField] EngagementMode engagementMode;
        public EngagementMode EngagementMode { get { return engagementMode; } }

        [SerializeField] OppositionMode oppositionMode;
        public OppositionMode OppositionMode { get { return oppositionMode; } }

        [SerializeField] GameObject placementPreviewPrefab;

        public static int MatrixRows = 8;
        public static int MatrixColumns = 8;

        private TrackingMainCameraManager cameraManager;
        private Cell[,] matrix;
        private int onHomeEventsPending;
        private Set activeSet;
        private PieceManager inFocusPiece;
        private PreviewManager inFocusPreview;
        private bool checkMate;
        private StageManager stageManager;
        private Dictionary<PieceManager, List<Cell>> availableMoves;
        private int maxRowIdx = MatrixRows - 1;
        private int maxColumnIdx = MatrixColumns - 1;
        private List<GameObject> previews;

        void Awake()
        {
            ResolveDependencies();

            matrix = new Cell[8, 8];
            previews = new List<GameObject>();
            availableMoves = new Dictionary<PieceManager, List<Cell>>();

            stageManager = new StageManager();
        }

        private void ResolveDependencies()
        {
            cameraManager = camera.GetComponent<TrackingMainCameraManager>() as TrackingMainCameraManager;
        }

        // Start is called before the first frame update
        void Start()
        {
            MapMatrix();
            MapPieces();
            // ReportMatrix();
            // ReportPieces();
            ResetGame();
        }

        void OnEnable()
        {
            HandController.ActuationEventReceived += OnActuation;
            ButtonEventManager.EventReceived += OnButtonEvent;
            PreviewManager.EventReceived += OnPreviewEvent;
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;
            ButtonEventManager.EventReceived -= OnButtonEvent;
            PreviewManager.EventReceived -= OnPreviewEvent;
        }

        private void OnButtonEvent(ButtonEventManager.Id id, ButtonEventType eventType)
        {
            if (eventType == ButtonEventType.OnPressed)
            {
                switch (id)
                {
                    case ButtonEventManager.Id.Reset:
                        ResetBoard();
                        break;
                }
            }
        }

        private List<PieceManager> ActivePieces { get { return setManger.AllPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        private List<PieceManager> ActiveLightPieces { get { return setManger.LightPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        private List<PieceManager> ActiveDarkPieces { get { return setManger.DarkPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        private void ResetGame()
        {
            InitiateUI();
            checkMate = false;
            activeSet = Set.Light;
            cameraManager.IncludeInteractableLayer("Placement Preview Layer");

            if (playMode == PlayMode.Freeform)
            {
                ConfigSetInteractions(Set.Light, true);
                ConfigSetInteractions(Set.Dark, true);
            }

            ManageTurn();
        }

        private void InitiateUI() => coordReferenceCanvas.TextUI = String.Empty;

        private void ManageTurn()
        {
            stageManager.LiveStage = Stage.Evaluating;
            availableMoves.Clear();
            inFocusPiece = null;
            
            if (playMode == PlayMode.RuleBased)
            {
                ConfigSetInteractions(Set.Light, false);
                ConfigSetInteractions(Set.Dark, false);
            }

            CalculateMoves();
        }

        private void ConfigSetInteractions(Set set, bool enabled)
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
            }
            else
            {
                cameraManager.ExcludeInteractableLayer(layer);
            }

            List<PieceManager> pieces = (set == Set.Light) ? ActiveLightPieces : ActiveDarkPieces;

            foreach (PieceManager piece in pieces)
            {
                if (piece.ActiveCell != null)
                {
                    piece.EnablePhysics(enabled);
                }
            }
        }

        private void CalculateMoves()
        {
            List<PieceManager> activePieces;

            if (playMode == PlayMode.Freeform)
            {
                activePieces = ActivePieces;
            }
            else
            {
                activePieces = (activeSet == Set.Light) ? ActiveLightPieces : ActiveDarkPieces;
            }

            if (ShouldAutomate())
            {

            }
            else
            {
                
            }

            foreach (PieceManager piece in activePieces)
            {
                // A necessary check to ensure we don't include any piece relegated to the capture zone,
                // Once a piece is taken off the board it no longer has an ActiveCell reference
                if (piece.ActiveCell == null) continue;

                List<Cell> moves = piece.CalculateMoves(matrix, (activeSet == Set.Light) ? 1 : -1);
                var hasMoves = moves.Count > 0;

                piece.EnablePhysics(hasMoves);

                if (hasMoves)
                {
                    availableMoves.Add(piece, moves);
                }
                else
                {
                    piece.ApplyMaterial(outOfScopeMaterial);
                }
            }

            stageManager.LiveStage = Stage.PendingSelect;
            
            if (playMode == PlayMode.RuleBased)
            {
                ConfigSetInteractions(Set.Light, (activeSet == Set.Light));
                ConfigSetInteractions(Set.Dark, (activeSet == Set.Dark));
            }

            AutomationCheck();
        }

        private bool ShouldAutomate()
        {
            return (activeSet == Set.Dark) && (oppositionMode != OppositionMode.None);
        }

        private void AutomationCheck()
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
            int moveIdx = Random.Range(0, availableMoves.Count);
        }

        private void CompleteTurn()
        {
            if (checkMate)
            {
                OnGameOver();
            }
            else
            {
                if (playMode == PlayMode.RuleBased)
                {
                    activeSet = (activeSet == Set.Light) ? Set.Dark : Set.Light;
                }

                ManageTurn();
            }
        }

        private void ResetThemes()
        {
            foreach (PieceManager piece in ActivePieces)
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
                inFocusPiece.ApplySelectedTheme();
                ConfigSetInteractions(activeSet, false);

                if (availableMoves.TryGetValue(inFocusPiece, out List<Cell> cells))
                {
                    foreach (Cell cell in cells)
                    {
                        var preview = GameObject.Instantiate(placementPreviewPrefab, Vector3.zero, Quaternion.identity, setManger.transform);
                        var manager = preview.GetComponent<PreviewManager>() as PreviewManager;
                        manager.PlaceAtCell(cell);
                        
                        preview.transform.parent = transform;
                        previews.Add(preview);
                    }
                }
            }
            else if (stageManager.LiveStage == Stage.Selected)
            {
                if (inFocusPreview == null) return;

                CommitToMove();
            }
        }

        private void CommitToMove()
        {
            Cell cell = inFocusPreview.Cell;

            FreePreviews();
            ResetThemes();

            coordReferenceCanvas.TextUI = string.Empty;

            stageManager.LiveStage = Stage.Moving;
            inFocusPiece.GoToCell(cell, rotationSpeed, movementSpeed);
        }

        private void CancelIntent()
        {
            stageManager.LiveStage = Stage.PendingSelect;
            inFocusPiece?.ApplyDefaultTheme();
            FreePreviews();

            ConfigSetInteractions(activeSet, true);
        }

        private void OnGameOver()
        {
            // TODO
        }

        private void FreePreviews()
        {
            foreach (GameObject preview in previews)
            {
                Destroy(preview);
            }
            
            inFocusPreview = null;
        }

        private void ResetBoard()
        {
            var activePieces = ActivePieces;
            onHomeEventsPending = activePieces.Count;

            stageManager.LiveStage = Stage.Resetting;
            
            foreach (PieceManager piece in activePieces)
            {
                if (piece.isActiveAndEnabled)
                {
                    piece.Reset();
                    piece.GoHome(rotationSpeed, movementSpeed);
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
                    var activePieces = ActivePieces;
                    onHomeEventsPending = activePieces.Count;

                    foreach (PieceManager thisPiece in activePieces)
                    {
                        thisPiece.ReinstatePhysics();
                    }

                    ResetGame();
                }
            }
            else if (stageManager.LiveStage == Stage.Moving)
            {
                CompleteTurn();
            }
        }

        private void OnEvent(PieceManager piece, FocusType focusType)
        {
            string cellReference = String.Empty;
                
            if (TryGets.TryGetCoordReference(piece.ActiveCell.coord, out string tmp))
            {
                cellReference = tmp;
            }
            
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    if ((playMode == PlayMode.RuleBased) && (piece.Set != activeSet)) return;

                    piece.ApplyHighlightTheme();

                    if (TryGets.TryGetCoordReference(piece.ActiveCell.coord, out string reference))
                    {
                        coordReferenceCanvas.TextUI = reference;
                    }

                    inFocusPiece = piece;
                    break;

                case FocusType.OnFocusLost:
                    if ((playMode == PlayMode.RuleBased) && (piece.Set != activeSet)) return;

                    if (stageManager.LiveStage == Stage.Selected) return;
                    
                    piece.ApplyDefaultTheme();
                    coordReferenceCanvas.TextUI = string.Empty;
                    break;
            }
        }

        private void OnPreviewEvent(PreviewManager manager, FocusType focusType)
        {
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    manager.SetMesh(inFocusPiece.Mesh, inFocusPiece.transform.localRotation);
                    inFocusPreview = manager;
                    break;

                case FocusType.OnFocusLost:
                    manager.SetMesh(null, Quaternion.identity);
                    break;
            }
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
                        localPosition = localPosition
                    };
                }
            }
        }
    }

    public void ReportMatrix(bool includeEmptyCells = false)
    {
        List<Cell> cells = AllCells;

        foreach (Cell cell in cells)
        {
            if (cell != null)
            {
                string cellReference = String.Empty;
                
                if (TryGets.TryGetCoordReference(cell.coord, out string reference))
                {
                    cellReference = reference;
                }

                if (cell.IsOccupied)
                {
                    Debug.Log($"ReportMatrix Piece : {cell.piece.name} Coord : [{cell.coord.x}, {cell.coord.y}] Reference : {cellReference} Position : [{cell.localPosition.x}, {cell.localPosition.y}, {cell.localPosition.z}]");
                }
                else if (includeEmptyCells)
                {
                    Debug.Log($"ReportMatrix EMPTY Coord : [{cell.coord.x} {cell.coord.y}] Reference : {cellReference} Position : [{cell.localPosition.x}, {cell.localPosition.y}, {cell.localPosition.z}]");
                }
            }
        }
    }

    public List<Cell> AllCells
    {
        get
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
        foreach (PieceManager piece in ActivePieces)
        {
            if ((piece.isActiveAndEnabled) && (TryGetPieceToCell(piece, out Cell cell)))
            {
                piece.HomeCell = cell;
                piece.MoveEventReceived += OnMoveEvent;
                piece.EventReceived += OnEvent;

                string cellReference = String.Empty;
                
                if (TryGets.TryGetCoordReference(cell.coord, out string reference))
                {
                    cellReference = reference;
                }

                // Debug.Log($"MapPieces {piece.name} Coord : [{piece.ActiveCell.coord.x}, {piece.ActiveCell.coord.y}] Reference : {cellReference} Position : [{piece.ActiveCell.localPosition.x}, {piece.ActiveCell.localPosition.y}, {piece.ActiveCell.localPosition.z}]");

                matrix[cell.coord.x, cell.coord.y].piece = piece;
            }
        }
    }

    public void ReportPieces()
    {
        foreach (PieceManager piece in ActivePieces)
        {
            string cellReference = String.Empty;
                
            if (TryGets.TryGetCoordReference(piece.ActiveCell.coord, out string reference))
            {
                cellReference = reference;
            }
            
            Debug.Log($"ReportPieces {piece.name} Coord : [{piece.ActiveCell.coord.x}, {piece.ActiveCell.coord.y}] Reference : {cellReference} Position : [{piece.ActiveCell.localPosition.x}, {piece.ActiveCell.localPosition.y}, {piece.ActiveCell.localPosition.z}]");
        }
    }
#endregion

#region TryGets
    public bool TryGetSetPiecesByType(Set set, PieceType type, out List<Cell> cells)
    {
        List<PieceManager> activePieces = (set == Set.Light) ? ActiveLightPieces : ActiveDarkPieces;
        List<Cell> matchingCells = activePieces.Where(p => p.Type == type).Select(p => p.ActiveCell).ToList();

        cells = matchingCells;
        return (matchingCells.Count > 0);
    }

    public bool TryGetPiecesByType(PieceType type, out List<Cell> cells)
    {
        List<Cell> matchingCells = ActivePieces.Where(p => p.Type == type).Select(p => p.ActiveCell).ToList();

        cells = matchingCells;
        return (matchingCells.Count > 0);
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
        Vector3 surface = setManger.transform.localPosition;

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
#endregion
    }
}
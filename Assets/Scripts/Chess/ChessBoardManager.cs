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

        [Header("Controllers")]
        [SerializeField] HybridHandController leftController;
        [SerializeField] HybridHandController rightController;

        [Header("Components")]
        [SerializeField] ChessBoardSetManager setManager;
        public ChessBoardSetManager SetManager { get { return setManager; } }

        [SerializeField] CoordReferenceCanvas coordReferenceCanvas;
        [SerializeField] ButtonEventManager resetButton;

        [Header("Materials")]
        [SerializeField] Material outOfScopeMaterial;
        [SerializeField] Material inFocusMaterial;
        [SerializeField] Material selectedMaterial;
        // [SerializeField] Material underThreatMaterial;
        // [SerializeField] Material moveMaterial;

        [Header("Config")]
        [SerializeField] PlayMode playMode;
        public PlayMode PlayMode { get { return playMode; } }

        [SerializeField] MoveStyle moveStyle;
        public MoveStyle MoveStyle { get { return moveStyle; } }

        [SerializeField] EngagementMode engagementMode;
        public EngagementMode EngagementMode { get { return engagementMode; } }

        [SerializeField] OppositionMode oppositionMode;
        public OppositionMode OppositionMode { get { return oppositionMode; } }

        [SerializeField] GameObject previewPrefab;

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
            
            cameraManager.IncludeInteractableLayer("Preview Layer");
            leftController.IncludeInteractableLayer("Preview Layer");
            rightController.IncludeInteractableLayer("Preview Layer");
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

            if (playMode == PlayMode.Freeform)
            {
                AdjustChessPieceInteractableLayer(Set.Light, true);
                AdjustChessPieceInteractableLayer(Set.Dark, true);
            }

            InitGame();
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

        private List<PieceManager> ActivePieces { get { return setManager.AllPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        private List<PieceManager> ActiveLightPieces { get { return setManager.LightPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        private List<PieceManager> ActiveDarkPieces { get { return setManager.DarkPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

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
            checkMate = false;
            activeSet = Set.Light;
        }

        private void ResetSet() => setManager.Reset();

        private void ResetUI() => coordReferenceCanvas.TextUI = String.Empty;

        private void ManageTurn()
        {
            stageManager.LiveStage = Stage.Evaluating;
            availableMoves.Clear();
            inFocusPiece = null;
            
            if (playMode == PlayMode.RuleBased)
            {
                EnableInteractions(Set.Light, false);
                EnableInteractions(Set.Dark, false);
            }

            CalculateMoves();
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
            List<PieceManager> pieces = (set == Set.Light) ? ActiveLightPieces : ActiveDarkPieces;

            foreach (PieceManager piece in pieces)
            {
                if (piece.ActiveCell != null)
                {
                    piece.EnableInteractions(enabled);
                }
            }

            AdjustChessPieceInteractableLayer(set, enabled);
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

            // Debug.Log($"CalculateMoves Set : {activeSet} Active Pieces : {activePieces.Count}");
            
            bool shouldAutomate = ShouldAutomate();

            foreach (PieceManager piece in activePieces)
            {
                // Debug.Log($"CalculateMoves Set : {activeSet} Piece : {piece.name}");

                // A necessary check to ensure we don't include any piece relegated to the capture zone,
                // Once a piece is taken off the board it no longer has an ActiveCell reference
                if (piece.ActiveCell == null) continue;

                List<Cell> moves = piece.CalculateMoves(matrix, (activeSet == Set.Light) ? 1 : -1);
                var hasMoves = moves.Count > 0;

                if (!shouldAutomate)
                {
                    piece.EnableInteractions(hasMoves);
                }

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
                AdjustChessPieceInteractableLayer(Set.Light, (activeSet == Set.Light));
                AdjustChessPieceInteractableLayer(Set.Dark, (activeSet == Set.Dark));
            }

            if (shouldAutomate)
            {
                AutomateMove();
            }
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
            if (checkMate)
            {
                OnGameOver();
            }
            else
            {
                activeSet = (activeSet == Set.Light) ? Set.Dark : Set.Light;
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

        private void CommitToMove(Cell cell)
        {
            FreePreviews();
            ResetThemes();

            coordReferenceCanvas.TextUI = string.Empty;

            stageManager.LiveStage = Stage.Moving;
            inFocusPiece.GoToCell(cell, moveStyle);
        }

        private void CancelIntent()
        {
            stageManager.LiveStage = Stage.PendingSelect;
            inFocusPiece?.UseDefaultMaterial();
            FreePreviews();

            if (playMode == PlayMode.RuleBased)
            {
                foreach (KeyValuePair<PieceManager, List<Cell>> element in availableMoves)
                {
                    element.Key.EnableInteractions(true);
                }

                AdjustChessPieceInteractableLayer(activeSet, true);
            }
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
                piece.Reset();
                piece.GoHome(moveStyle);
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
                        thisPiece.EnablePhysics(false);
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

                    piece.ApplyMaterial(inFocusMaterial);

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
                    coordReferenceCanvas.TextUI = string.Empty;
                    break;
            }
        }

        private void OnPreviewEvent(PreviewManager manager, FocusType focusType)
        {
            if (TryGetCell(manager.transform.localPosition, out Cell cell))
            {
                switch (focusType)
                {
                    case FocusType.OnFocusGained:
                        if (cell.IsOccupied)
                        {
                            cell.piece.HideMesh();
                        }

                        manager.SetCustomMesh(inFocusPiece.Mesh, inFocusPiece.transform.localRotation, inFocusPiece.DefaultMaterial /*inFocusPiece.Material*/ /*moveMaterial*/);
                
                        inFocusPreview = manager;
                        break;

                    case FocusType.OnFocusLost:
                        if (cell.IsOccupied)
                        {
                            cell.piece.ShowMesh();
                        }

                        manager.HideMesh();

                        break;
                }
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

        // foreach (PieceManager piece in ActivePieces)
        // {
        //     if (piece.isActiveAndEnabled)
        //     {
        //         piece.TestKingTrajectories();
        //     }
        // }
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
    public bool TryGetSingleSetPieceByType(Set set, PieceType type, out Cell cell)
    {
        List<PieceManager> activePieces = (set == Set.Light) ? ActiveLightPieces : ActiveDarkPieces;
        List<Cell> matchingCells = activePieces.Where(p => p.Type == type).Select(p => p.ActiveCell).ToList();

        if (matchingCells.Count > 0)
        {
            cell = matchingCells.First();
            return true;
        }
        
        cell = default(Cell);
        return false;
    }

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

    public bool TryGetPiecesAlongVector(Cell[,] projectedMatrix, Cell origin, Vector2 vector, out List<PieceManager> pieces)
    {
        Debug.Log($"TryGetPiecesAlongVector Origin : [{origin.coord.x}, {origin.coord.y}] Vector : [{vector.x}, {vector.y}]");

        pieces = new List<PieceManager>();

        int x = origin.coord.x;
        int y = origin.coord.y;

        do
        {
            x += (int) vector.x;
            y += (int) vector.y;

            if ((x >= 0 && x <= maxColumnIdx) && (y >= 0 && y <= maxRowIdx))
            {
                Cell cell = projectedMatrix[x, y];

                if (cell.piece != null)
                {
                    pieces.Add(cell.piece);
                }
            }
        } while ((x >= 0 && x <= maxColumnIdx) && (y >= 0 && y <= maxRowIdx));

        return false;
    }
#endregion
    }
}
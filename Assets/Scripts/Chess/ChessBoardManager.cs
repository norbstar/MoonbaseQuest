using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

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
        [SerializeField] ChessBoardSetManager set;
        [SerializeField] CoordReferenceCanvas coordReferenceCanvas;
        [SerializeField] ButtonEventManager resetButton;

        [Header("Pieces")]
        [SerializeField] float rotationSpeed = 25f;
        [SerializeField] float movementSpeed = 25f;
        [SerializeField] Material outOfScopeMaterial;

        [Header("Config")]
        [SerializeField] GameObject placementPreviewPrefab;

        public static int MatrixRows = 8;
        public static int MatrixColumns = 8;

        private enum Stage
        {
            Evaluation,
            Uncommited,
            Selected
        }

        private TrackingMainCameraManager cameraManager;
        private Cell[,] matrix;
        private int onHomeEventsPending;
        private Set activeSet;
        private Piece inFocusPiece;
        private bool checkMate;
        private Stage stage;
        private Dictionary<Piece, List<Cell>> availableMoves;
        private int maxRowIdx = MatrixRows - 1;
        private int maxColumnIdx = MatrixColumns - 1;
        private List<GameObject> previews;

        void Awake()
        {
            ResolveDependencies();

            matrix = new Cell[8, 8];
            previews = new List<GameObject>();
            availableMoves = new Dictionary<Piece, List<Cell>>();
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
            InitiateGame();
        }

        void OnEnable()
        {
            HandController.ActuationEventReceived += OnActuation;
            ButtonEventManager.EventReceived += OnButtonEvent;
            PlacementPreviewManager.EventReceived += OnPlacementPreviewEvent;
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;
            ButtonEventManager.EventReceived -= OnButtonEvent;
            PlacementPreviewManager.EventReceived -= OnPlacementPreviewEvent;
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

        private List<Piece> ActivePieces()
        {
            var allPieces = set.AllPieces();
            return allPieces.Where(p => p.isActiveAndEnabled).ToList();
        }

        private void InitiateGame()
        {
            InitiateUI();
            checkMate = false;
            activeSet = Set.Light;
            cameraManager.IncludeInteractableLayer("Placement Preview Layer");

            ManageTurn();
        }

        private void InitiateUI() => coordReferenceCanvas.TextUI = String.Empty;

        private void ManageTurn()
        {
            stage = Stage.Evaluation;
            cameraManager.ExcludeInteractableLayer("Chess Piece Layer");
            availableMoves.Clear();
            inFocusPiece = null;

            CalculateMoves();
        }

        private void CalculateMoves()
        {
            List<Piece> pieces = (activeSet == Set.Light) ? set.LightPieces() : set.DarkPieces();

            foreach (Piece piece in pieces)
            {
                List<Cell> moves = piece.CalculateMoves(matrix, maxColumnIdx, maxRowIdx, (activeSet == Set.Light) ? 1 : -1);
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

            stage = Stage.Uncommited;
            cameraManager.IncludeInteractableLayer("Chess Piece Layer");
        }

        private void CompleteTurn()
        {
            FreePreviews();

            if (checkMate)
            {
                OnGameOver();
            }
            else
            {
                ResetThemes();
                activeSet = (activeSet == Set.Light) ? Set.Dark : Set.Light;
                ManageTurn();
            }
        }

        private void ResetThemes()
        {
            var activePieces = ActivePieces();
            
            foreach (Piece piece in activePieces)
            {
                if (piece.isActiveAndEnabled)
                {
                    piece.Reset();
                }
            }
        }

        public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
        {
            // if ((int) characteristics == (int) HandController.LeftHand)
            // {
            //     Debug.Log($"OnActuation Left Hand: {actuation}");
            // }
            // else if ((int) characteristics == (int) HandController.RightHand)
            // {
            //     Debug.Log($"OnActuation Right Hand : {actuation}");
            // }

            if ((stage == Stage.Evaluation) || (inFocusPiece == null)) return;

            if (actuation.HasFlag(Actuation.Button_AX))
            {
                ProcessIntent();
            }
            else if (actuation.HasFlag(Actuation.Button_BY))
            {
                if (stage == Stage.Selected)
                {
                    CancelIntent();
                }
            }
        }

        private void ProcessIntent()
        {
            if (stage == Stage.Uncommited)
            {
                stage = Stage.Selected;
                inFocusPiece.ApplySelectedTheme();
                cameraManager.ExcludeInteractableLayer("Chess Piece Layer");

                // TODO keep tracking enabled but prevent other pieces from being selectable as we have locked in on this piece already

                if (availableMoves.TryGetValue(inFocusPiece, out List<Cell> moves))
                {
                    foreach (Cell move in moves)
                    {
                        var placementPreview = GameObject.Instantiate(placementPreviewPrefab, Vector3.zero, Quaternion.identity, set.transform);
                        placementPreview.transform.localPosition = move.localPosition;
                        placementPreview.transform.parent = transform;
                        
                        previews.Add(placementPreview);
                    }
                }
            }
            else if (stage == Stage.Selected)
            {
                // TODO this only applies if we have selected a legal cell to move the piece to

                CompleteTurn();
            }
        }

        private void CancelIntent()
        {
            stage = Stage.Uncommited;
            inFocusPiece?.ApplyDefaultTheme();
            FreePreviews();

            cameraManager.IncludeInteractableLayer("Chess Piece Layer");
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
        }

        private void ResetBoard()
        {
            var activePieces = ActivePieces();
            onHomeEventsPending = activePieces.Count;

            foreach (Piece piece in activePieces)
            {
                if (piece.isActiveAndEnabled)
                {
                    piece.Reset();
                    piece.GoHome(rotationSpeed, movementSpeed);
                }
            }
        }

        private void OnHomeEvent(Piece piece)
        {
            --onHomeEventsPending;

            if (onHomeEventsPending == 0)
            {
                var activePieces = ActivePieces();
                onHomeEventsPending = activePieces.Count;

                foreach (Piece thisPiece in activePieces)
                {
                    thisPiece.ReinstatePhysics();
                }

                InitiateGame();
            }
        }

        private void OnEvent(Piece piece, FocusType focusType)
        {
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    if (piece.Set != activeSet) return;

                    piece.ApplyHighlightTheme();

                    if (TryGetCoordReference(piece.ActiveCell.coord, out string reference))
                    {
                        coordReferenceCanvas.TextUI = reference;
                    }

                    inFocusPiece = piece;
                    break;

                case FocusType.OnFocusLost:
                    if (piece.Set != activeSet) return;

                    if (stage == Stage.Selected) return;
                    
                    piece.ApplyDefaultTheme();

                    coordReferenceCanvas.TextUI = string.Empty;
                    break;
            }
        }

        private void OnPlacementPreviewEvent(PlacementPreviewManager manager, FocusType focusType)
        {
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    manager.SetMesh(inFocusPiece.Mesh, inFocusPiece.transform.localRotation);
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

        private void ReportMatrix()
        {
            for (int y = 0 ; y <= maxRowIdx ; y++)
            {
                for (int x = 0 ; x <= maxColumnIdx ; x++)
                {
                    Cell cell = matrix[x, y];

                    if (cell != null)
                    {
                        if (cell.piece != null)
                        {
                            Debug.Log($"ReportMatrix X : {x} Y : {y} Piece : {cell.piece.name} Coord : [{cell.coord.x} {cell.coord.y}] Position : [{cell.localPosition.x} {cell.localPosition.y} {cell.localPosition.z}]");
                        }
                        else
                        {
                            Debug.Log($"ReportMatrix X : {x} Y : {y} Coord : [{cell.coord.x} {cell.coord.y}] Position : [{cell.localPosition.x} {cell.localPosition.y} {cell.localPosition.z}]");
                        }
                    }
                }
            }
        }
#endregion

#region Pieces
        private void MapPieces()
        {
            var activePieces = ActivePieces();

            foreach (Piece piece in activePieces)
            {
                if ((piece.isActiveAndEnabled) && (TryGetPieceToCell(piece, out Cell cell)))
                {
                    piece.HomeCell = cell;
                    piece.HomeEventReceived += OnHomeEvent;
                    piece.EventReceived += OnEvent;

                    matrix[cell.coord.x, cell.coord.y].piece = piece;
                }
            }
        }
#endregion

#region TryGets
        private bool TryGetPieceToCell(Piece piece, out Cell cell)
        {
            var localPosition = RoundPosition(piece.transform.localPosition);
         
            if (TryGetCoord(localPosition, out Coord coord))
            {
                cell = matrix[coord.x, coord.y];
                return true;
            }

            cell = default(Cell);
            return false;
        }

        private bool TryGetCoord(Vector3 localPosition, out Coord coord)
        {
            var normX = Normalize(localPosition.x, -0.35f, 0.35f);
            int x = (int) Mathf.Round(maxColumnIdx * (float) normX);

            var normZ = Normalize(localPosition.z, -0.35f, 0.35f);
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

        private bool TryGetCoordToPosition(Coord coord, out Vector3 localPosition)
        {
            Vector3 surface = set.transform.localPosition;

            if ((coord.x >= 0 && coord.x <= maxColumnIdx) && (coord.y >= 0 && coord.y <= maxRowIdx))
            {
                float x = RoundFloat(-0.35f + (coord.x * 0.1f));
                float y = RoundFloat(-0.35f + (coord.y * 0.1f));

                localPosition = new Vector3(x, surface.y, y);
                return true;
            }

            localPosition = default(Vector3);
            return false;
        }

        private bool TryGetCoordReference(Coord coord, out string reference)
        {
            if ((coord.x >= 0 && coord.x <= maxColumnIdx) && (coord.y >= 0 && coord.y <= maxRowIdx))
            {
                char letter = Convert.ToChar((int) 'a' + coord.x);
                char digit = Convert.ToChar((int) '1' + coord.y);
                reference = $"{letter} : {digit}";
                return true;
            }

            reference = default(string);
            return false;
        }
#endregion

#region Math
        private Vector3 RoundPosition(Vector3 localPosition)
        {
            return new Vector3(RoundFloat(localPosition.x), RoundFloat(localPosition.y), RoundFloat(localPosition.z));
        }

        private float RoundFloat(float value)
        {
            return Mathf.Round(value * 100f) / 100f;
        }

        private double Normalize(double value, double min, double max) {
            return (value - min) / (max - min);
        }
#endregion
    }
}
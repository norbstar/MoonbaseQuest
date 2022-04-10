using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

using Chess.Pieces;
using Chess.Button;

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

        private enum Stage
        {
            Uncommited,
            Selected,
            Confirmed
        }

        private TrackingMainCameraManager cameraManager;
        private Cell[,] matrix;
        private int onHomeEventsPending;
        private Set activeSet;
        private Piece inFocusPiece;
        private bool checkMate;
        private Stage stage;

        void Awake()
        {
            ResolveDependencies();
            matrix = new Cell[8, 8];
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
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;
            ButtonEventManager.EventReceived -= OnButtonEvent;
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
            ManageTurn();
        }

        private void InitiateUI() => coordReferenceCanvas.TextUI = String.Empty;

        private void ManageTurn()
        {
            stage = Stage.Uncommited;
            cameraManager.EnableTracking = true;
            inFocusPiece = null;

            // TODO
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

            if (inFocusPiece == null) return;

            if (actuation.HasFlag(Actuation.Button_AX))
            {
                ProcessIntent();
            }
            else if (actuation.HasFlag(Actuation.Button_BY))
            {
                CancelIntent();
            }
        }

        private void ProcessIntent()
        {
            if (stage == Stage.Uncommited)
            {
                cameraManager.EnableTracking = false;
                
                List<Cell> moves = inFocusPiece.CalculateMoves();

                if (moves != null)
                {
                    inFocusPiece.MarkAvailable();

                    foreach (Cell move in moves)
                    {
                        // TODO
                    }
                }
                else
                {
                    inFocusPiece.MarkUnavailable();
                }

                stage = Stage.Selected;
            }
            else if (stage == Stage.Selected)
            {
                // TODO
            }
        }

        private void CancelIntent()
        {
            stage = Stage.Uncommited;
            inFocusPiece.ApplyDefault();
            cameraManager.EnableTracking = true;
        }

        private void OnGameOver() { }

        private void ResetBoard()
        {
            var activePieces = ActivePieces();
            onHomeEventsPending = activePieces.Count;

            foreach (Piece piece in activePieces)
            {
                if (piece.isActiveAndEnabled)
                {
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

                    piece.ApplyHighlight();

                    if (TryGetCoordReference(piece.ActiveCell.coord, out string reference))
                    {
                        coordReferenceCanvas.TextUI = reference;
                    }

                    inFocusPiece = piece;
                    break;

                case FocusType.OnFocusLost:
                    if (piece.Set != activeSet) return;
                    
                    piece.ApplyDefault();

                    coordReferenceCanvas.TextUI = string.Empty;
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // TODO
        }

#region Matrix
        private void MapMatrix()
        {
            for (int y = 0 ; y <= 7 ; y++)
            {
                for (int x = 0 ; x <= 7 ; x++)
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
            for (int y = 0 ; y <= 7 ; y++)
            {
                for (int x = 0 ; x <= 7 ; x++)
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
            int x = (int) Mathf.Round(7 * (float) normX);

            var normZ = Normalize(localPosition.z, -0.35f, 0.35f);
            int z = (int) Mathf.Round(7 * (float) normZ);

            if ((x >= 0 && x <= 7) && (z >= 0 && z <= 7))
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

            if ((coord.x >= 0 && coord.x <= 7) && (coord.y >= 0 && coord.y <= 7))
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
            if ((coord.x >= 0 && coord.x <= 7) && (coord.y >= 0 && coord.y <= 7))
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
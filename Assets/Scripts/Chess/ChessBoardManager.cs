using UnityEngine;

using Chess.Pieces;
using Chess.Button;

namespace Chess
{
    public class ChessBoardManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] ChessBoardSetManager set;
        [SerializeField] ButtonEventManager resetButton;

        private Cell[,] matrix;
        private int onHomeEventsPending;

        void Awake()
        {
            matrix = new Cell[8, 8];
        }

        // Start is called before the first frame update
        void Start()
        {
            MapMatrix();
            MapPieces();
        }

        void OnEnable()
        {
            ButtonEventManager.EventReceived += OnButtonEvent;
        }

        void OnDisable()
        {
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

        private void ResetBoard()
        {
            var allPieces = set.AllPieces();
            onHomeEventsPending = allPieces.Count;

            foreach (Piece piece in allPieces)
            {
                piece.GoHome();
            }
        }

        private void OnHomeEvent()
        {
            --onHomeEventsPending;

            if (onHomeEventsPending == 0)
            {
                foreach (Piece piece in set.AllPieces())
                {
                    piece.ResetPhysics();
                }
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
            foreach (Piece piece in set.AllPieces())
            {
                if (TryGetPieceToCell(piece, out Cell cell))
                {
                    piece.Home = cell;
                    piece.HomeEventReceived += OnHomeEvent;
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
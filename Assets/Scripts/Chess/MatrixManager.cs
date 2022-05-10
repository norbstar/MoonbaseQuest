using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    [RequireComponent(typeof(ChessBoardManager))]
    public class MatrixManager : MonoBehaviour
    {
        public static int MatrixRows = 8;
        public static int MaxRowIdx = MatrixRows - 1;
        public static int MatrixColumns = 8;
        public static int MaxColumnIdx = MatrixColumns - 1;

        public Cell[,] Matrix { get { return matrix; } }

        private ChessBoardManager chessBoardManager;
        private Cell[,] matrix;

        void Awake()
        {
            ResolveDependencies();
            matrix = new Cell[MatrixColumns, MatrixRows];
        }

        private void ResolveDependencies() => chessBoardManager = GetComponent<ChessBoardManager>() as ChessBoardManager;

        public void MapLayout()
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

                    Vector3 surface = chessBoardManager.SetManager.transform.localPosition;

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
            foreach (PieceManager pieceManager in chessBoardManager.SetManager.EnabledPieces)
            {
                if ((pieceManager.isActiveAndEnabled) && (TryGets.TryGetPieceToCell(matrix, pieceManager, out Cell cell)))
                {
                    pieceManager.HomeCell = cell;
                    pieceManager.MoveEventReceived += chessBoardManager.OnMoveEvent;
                    pieceManager.EventReceived += chessBoardManager.OnEvent;

                    matrix[cell.coord.x, cell.coord.y].wrapper.manager = pieceManager;
                }
            }
        }

        public Cell[,] CloneMatrix()
        {
            return matrix.Clone() as Cell[,];
        }

        public Cell[,] ProjectMatrix(Cell cell, Cell targetCell)
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
    }
}
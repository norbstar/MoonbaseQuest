using System.Collections.Generic;

namespace Chess.Pieces
{
    public class PawnManager : PieceManager
    {
        private bool hasHistory;

        public override List<Cell> CalculateMoves(ChessBoardManager manager, Cell[,] matrix, int vector)
        {
            List<Cell> moves = base.CalculateMoves(manager, matrix, vector);
            hasHistory = true;

            return moves;
        }

        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix, int vector)
        {
            List<Cell> cells = new List<Cell>();
            List<Coord> coords = GenerateCoords(matrix, vector);

            if (TryGetPotentialCoords(ActiveCell.coord, coords, out List<Coord> potentialCoords))
            {
                foreach (Coord coord in potentialCoords)
                {
                    Cell cell = matrix[coord.x, coord.y];

                    if ((cell.piece == null) || (cell.piece.Set != set))
                    {
                        cells.Add(cell);
                    }
                }
            }

            return cells;
        }

        private List<Coord> GenerateCoords(Cell[,] matrix, int vector)
        {
            List<Coord> coords = new List<Coord>();
            List<Coord> vectorCoords;

            Coord activeCoord = ActiveCell.coord;
            
            if (TryGetVectorCoords(activeCoord, 0, vector, out vectorCoords))
            {
                coords.AddRange(vectorCoords);
            }

            return coords;
        }

        public override void Reset()
        {
            base.Reset();
            hasHistory = false;
        }
    }
}
using System.Linq;
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

        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix)
        {
            List<Cell> cells = new List<Cell>();

            List<Coord> coords = new[]
            {
                new Coord { x = 1, y = 3 },
                new Coord { x = 2, y = 2 },
                new Coord { x = 2, y = 4 },
                new Coord { x = 3, y = 3 },
                new Coord { x = 4, y = 2 },
                new Coord { x = 4, y = 4 },
                new Coord { x = 5, y = 3 }
            }.ToList<Coord>();

            if (TryGetPotentialCoords(ActiveCell.coord, coords, out List<Coord> potentialCoords))
            {
                foreach (Coord coord in potentialCoords)
                {
                    Cell cell = matrix[coord.x, coord.y];

                    if ((cell.piece == null) || (cell.piece.Set != set))
                    {
                        cells.Add(matrix[coord.x, coord.y]);
                    }
                }
            }

            return cells;
        }

        public override void Reset()
        {
            base.Reset();
            hasHistory = false;
        }
    }
}
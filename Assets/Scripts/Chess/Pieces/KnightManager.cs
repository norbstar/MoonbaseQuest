using System.Linq;
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KnightManager : PieceManager
    {
        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix, int vector)
        {
            List<Cell> cells = new List<Cell>();

            List<Coord> coords = new[]
            {
                new Coord { x = -1, y = 2 },
                new Coord { x = 1, y = 2 },
                new Coord { x = 2, y = 1 },
                new Coord { x = 2, y = -1 },
                new Coord { x = -1, y = -2 },
                new Coord { x = 1, y = -2 },
                new Coord { x = -2, y = 1 },
                new Coord { x = -2, y = -1 }
            }.ToList<Coord>();

            if (TryGetPotentialCoordsByOffset(ActiveCell.coord, coords, out List<Coord> potentialCoords))
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
    }
}
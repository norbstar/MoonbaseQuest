using System.Linq;
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class RookManager : PieceManager
    {
        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix, int vector)
        {
            List<Cell> cells = new List<Cell>();
            List<Coord> coords = GenerateCoords(matrix);

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

        private List<Coord> GenerateCoords(Cell[,] matrix)
        {
            List<Coord> coords = new List<Coord>();
            List<Coord> vectorCoords;

            Coord activeCoord = ActiveCell.coord;
            
            if (TryGetVectorCoords(activeCoord, -1, 0, out vectorCoords))
            {
                coords.AddRange(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 1, 0, out vectorCoords))
            {
                coords.AddRange(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 0, -1, out vectorCoords))
            {
                coords.AddRange(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 0, 1, out vectorCoords))
            {
                coords.AddRange(vectorCoords);
            }

            return coords;
        }
        
        private bool HasLineOfSightToCoord(Coord coord)
        {
            // TODO

            return false;
        }
    }
}
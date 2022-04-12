using System.Linq;
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KnightManager : PieceManager
    {
        public override List<Cell> CalculateMoves(ChessBoardManager manager, Cell[,] matrix, int vector)
        {
            List<Cell> moves = new List<Cell>();
            List<Cell> potentialMoves = ResolvePotentialCells(matrix);

            foreach (Cell potentialMove in potentialMoves)
            {
                // TODO determine if the potential move would put your own King in check.
                // If so, the move is NOT legel, otherwise add it to the list of moves

                // TMP measure to test the base functionality of the piece
                moves.Add(potentialMove);
            }

            // if (manager.TryGetSetPiecesByType(set, PieceType.King, out List<Cell> cells))
            // {
            //     if (cells.Count > 0)
            //     {
            //         var kingCell = cells[0];
            //         Debug.Log("$Calculate Moves King Cell Coords : [{kingCell.coords.x} {kingCell.coords.y}]");
            //     }
            // }

            return moves;
        }

        private List<Cell> ResolvePotentialCells(Cell[,] matrix)
        {
            List<Cell> cells = new List<Cell>();

            // Coord offsetCoord;

            // if (TryGetOffsetCoord(ActiveCell.coord, -1, 3, out offsetCoord))
            // {
            //     cells.Add(offsetCoord);
            // }

            // if (TryGetOffsetCoord(ActiveCell.coord, 1, 3, out offsetCoord))
            // {
            //     cells.Add(offsetCoord);
            // }

            List<Coord> offsets = new[]
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

            if (TryGetPotentialCoords(ActiveCell.coord, offsets, out List<Coord> potentialCoords))
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
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class RookManager : PieceManager
    {
        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix, int vector)
        {
            List<Cell> cells = new List<Cell>();
            List<List<Coord>> generatedCoords = GenerateCoords(matrix);

            foreach (List<Coord> coords in generatedCoords)
            {
                if (TryGetPotentialCoords(ActiveCell.coord, coords, out List<Coord> potentialCoords))
                {
                    cells.AddRange(EvaluatePotentialCells(matrix, potentialCoords));
                }
            }

            return cells;
        }

        private List<Cell> EvaluatePotentialCells(Cell[,] matrix, List<Coord> potentialCoords)
        {
            // UnityEngine.Debug.Log($"EvaluatePotentialCoords Potential Coord Count : {potentialCoords.Count}");

            List<Cell> cells = new List<Cell>();

            foreach (Coord coord in potentialCoords)
            {
                Cell cell = matrix[coord.x, coord.y];

                if (cell.piece != null)
                {
                    // The cell is occupied
                    if (cell.piece.Set != set)
                    {
                        // The cell is occuplied by an opposing piece
                        cells.Add(cell);
                    }

                    return cells;
                }
                else
                {
                    // The cell is unoccupied
                    cells.Add(cell);
                }
            }

            return cells;
        }

        private List<List<Coord>> GenerateCoords(Cell[,] matrix)
        {
            List<List<Coord>> coords = new List<List<Coord>>();
            List<Coord> vectorCoords;

            Coord activeCoord = ActiveCell.coord;
            
            if (TryGetVectorCoords(activeCoord, -1, 0, out vectorCoords))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 1, 0, out vectorCoords))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 0, -1, out vectorCoords))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 0, 1, out vectorCoords))
            {
                coords.Add(vectorCoords);
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
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class RookManager : PieceManager
    {
        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix, int vector)
        {
            List<Cell> cells = new List<Cell>();
            List<Coord> coords = GenerateCoords(matrix);

            // THIS IS WRONG
            // COORDS (ABOVE) NEED TO RESTURN A LIST OF SET OF COORDS
            // THE FUNCTION SHOULD RETURN A SET OF VECTOR SPECIFIC COORDS AND NOT AN AGGREGATE LIST
            // EACH VECTOR SET NEEDS TO BE EVALUATED IN THE SEQUENCE IN WHICH THE CELLS WERE RESOLVED
            // SUCH THAT BREAKING THE LOOP ONLY BREAKS THAT ONE VECTOR LINE.

            if (TryGetPotentialCoords(ActiveCell.coord, coords, out List<Coord> potentialCoords))
            {
                bool complete = false;

                foreach (Coord coord in potentialCoords)
                {
                    if (complete) break;

                    Cell cell = matrix[coord.x, coord.y];

                    UnityEngine.Debug.Log("1");

                    if (cell.piece != null)
                    {
                        UnityEngine.Debug.Log("2 Piece : {cell.piece.name}");

                        // The cell is occupied
                        if (cell.piece.Set != set)
                        {
                            UnityEngine.Debug.Log("3 Set : {set}");
                           
                            // The cell is occuplied by an opposing piece
                            cells.Add(cell);
                        }
 
                        complete = true;
                    }
                    else
                    {
                        UnityEngine.Debug.Log("4 Unoccupied");
                        // The cell is unoccupied
                        cells.Add(cell);
                    }
                }
            }

            UnityEngine.Debug.Log("4");

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
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class BishopManager : PieceManager
    {
        protected override List<List<Coord>> GenerateCoords(Cell[,] matrix, int vector)
        {
            List<List<Coord>> coords = new List<List<Coord>>();
            Coord activeCoord = ActiveCell.coord;

            TryCoord(-1, 1, coords);
            TryCoord(1, 1, coords);
            TryCoord(-1, -1, coords);
            TryCoord(1, -1, coords);
            
#if false
            List<Coord> vectorCoords;
            
            if (TryGetVectorCoords(activeCoord, -1, 1, out vectorCoords))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 1, 1, out vectorCoords))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, -1, -1, out vectorCoords))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 1, -1, out vectorCoords))
            {
                coords.Add(vectorCoords);
            }
#endif

            return coords;
        }
    }
}
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KingManager : PieceManager
    {
        protected override List<List<Coord>> GenerateCoords(Cell[,] matrix, int vector)
        {
            List<List<Coord>> coords = new List<List<Coord>>();
            Coord activeCoord = ActiveCell.coord;

            TryOneTimeCoord(-1, 0, coords);
            TryOneTimeCoord(-1, 1, coords);
            TryOneTimeCoord(1, 0, coords);
            TryOneTimeCoord(1, 1, coords);
            TryOneTimeCoord(0, -1, coords);
            TryOneTimeCoord(-1, -1, coords);
            TryOneTimeCoord(0, 1, coords);
            TryOneTimeCoord(1, -1, coords);

#if false
            List<Coord> vectorCoords;

            if (TryGetVectorCoords(activeCoord, -1, 0, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, -1, 1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 1, 0, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 1, 1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 0, -1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, -1, -1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 0, 1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 1, -1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }
#endif

            return coords;
        }
    }
}
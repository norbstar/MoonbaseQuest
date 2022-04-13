// using System.Linq;
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KnightManager : PieceManager
    {
#if false
        protected override List<List<Coord>> GenerateCoords(Cell[,] matrix, int vector)
        {
            List<List<Coord>> coords = new List<List<Coord>>();

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

            if (TryGetPotentialCoordsByOffset(ActiveCell.coord, offsets, out List<Coord> potentialCoords))
            {
                foreach (Coord coord in potentialCoords)
                {
                    coords.Add(new List<Coord> { coord });
                }
            }

            return coords;
        }
#endif

#if true
        protected override List<List<Coord>> GenerateCoords(Cell[,] matrix, int vector)
        {
            List<List<Coord>> coords = new List<List<Coord>>();
            Coord activeCoord = ActiveCell.coord;

            TryOneTimeCoord(-1, 2, coords);
            TryOneTimeCoord(1, 2, coords);
            TryOneTimeCoord(2, 1, coords);
            TryOneTimeCoord(2, -1, coords);
            TryOneTimeCoord(-1, -2, coords);
            TryOneTimeCoord(1, -2, coords);
            TryOneTimeCoord(-2, 1, coords);
            TryOneTimeCoord(-2, -1, coords);

#if false            
            List<Coord> vectorCoords;

            if (TryGetVectorCoords(activeCoord, -1, 2, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 1, 2, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 2, 1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 2, -1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, -1, -2, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, 1, -2, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, -2, 1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }

            if (TryGetVectorCoords(activeCoord, -2, -1, out vectorCoords, 1))
            {
                coords.Add(vectorCoords);
            }
#endif

            return coords;
        }
#endif
    }
}
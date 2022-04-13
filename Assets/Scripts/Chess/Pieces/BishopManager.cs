using System.Collections.Generic;

namespace Chess.Pieces
{
    public class BishopManager : PieceManager
    {
        // protected override List<List<Coord>> GenerateCoordBundles(Cell[,] matrix, int vector)
        // {
        //     List<List<Coord>> coords = new List<List<Coord>>();
        //     Coord activeCoord = ActiveCell.coord;

        //     TryCoord(-1, 1, coords);
        //     TryCoord(1, 1, coords);
        //     TryCoord(-1, -1, coords);
        //     TryCoord(1, -1, coords);
  
        //     return coords;
        // }

        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();
            // List<Coord> vectorCoords;

            // if (TryGetVectorCoords(activeCoord, -1, 1, out vectorCoords))
            // {
            //     bundles.Add(new CoordBundle
            //     {
            //         coords = vectorCoords
            //     });
            // }

            TryCoord(-1, 1, bundles);
            TryCoord(1, 1, bundles);
            TryCoord(-1, -1, bundles);
            TryCoord(1, -1, bundles);

            return bundles;
        }
    }
}
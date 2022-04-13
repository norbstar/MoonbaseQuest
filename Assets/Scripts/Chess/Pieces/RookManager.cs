using System.Collections.Generic;

namespace Chess.Pieces
{
    public class RookManager : PieceManager
    {
        // protected override List<List<Coord>> GenerateCoordBundles(Cell[,] matrix, int vector)
        // {
        //     List<List<Coord>> coords = new List<List<Coord>>();
        //     Coord activeCoord = ActiveCell.coord;
            
        //     TryCoord(-1, 0, coords);
        //     TryCoord(1, 0, coords);
        //     TryCoord(0, -1, coords);
        //     TryCoord(0, 1, coords);

        //     return coords;
        // }

        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();

            TryCoord(-1, 0, bundles);
            TryCoord(1, 0, bundles);
            TryCoord(0, -1, bundles);
            TryCoord(0, 1, bundles);

            return bundles;
        }
    }
}
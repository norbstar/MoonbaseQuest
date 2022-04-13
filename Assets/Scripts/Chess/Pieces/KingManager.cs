using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KingManager : PieceManager
    {
        // protected override List<List<Coord>> GenerateCoordBundles(Cell[,] matrix, int vector)
        // {
        //     List<List<Coord>> coords = new List<List<Coord>>();
        //     Coord activeCoord = ActiveCell.coord;

        //     TryOneTimeCoord(-1, 0, coords);
        //     TryOneTimeCoord(-1, 1, coords);
        //     TryOneTimeCoord(1, 0, coords);
        //     TryOneTimeCoord(1, 1, coords);
        //     TryOneTimeCoord(0, -1, coords);
        //     TryOneTimeCoord(-1, -1, coords);
        //     TryOneTimeCoord(0, 1, coords);
        //     TryOneTimeCoord(1, -1, coords);

        //     return coords;
        // }

        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();

            TryOneTimeCoord(-1, 0, bundles);
            TryOneTimeCoord(-1, 1, bundles);
            TryOneTimeCoord(1, 0, bundles);
            TryOneTimeCoord(1, 1, bundles);
            TryOneTimeCoord(0, -1, bundles);
            TryOneTimeCoord(-1, -1, bundles);
            TryOneTimeCoord(0, 1, bundles);
            TryOneTimeCoord(1, -1, bundles);

            return bundles;
        }
    }
}
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KnightManager : PieceManager
    {
        // protected override List<List<Coord>> GenerateCoordBundles(Cell[,] matrix, int vector)
        // {
        //     List<List<Coord>> coords = new List<List<Coord>>();
        //     Coord activeCoord = ActiveCell.coord;

        //     TryOneTimeCoord(-1, 2, coords);
        //     TryOneTimeCoord(1, 2, coords);
        //     TryOneTimeCoord(2, 1, coords);
        //     TryOneTimeCoord(2, -1, coords);
        //     TryOneTimeCoord(-1, -2, coords);
        //     TryOneTimeCoord(1, -2, coords);
        //     TryOneTimeCoord(-2, 1, coords);
        //     TryOneTimeCoord(-2, -1, coords);

        //     return coords;
        // }

        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();

            TryOneTimeCoord(-1, 2, bundles);
            TryOneTimeCoord(1, 2, bundles);
            TryOneTimeCoord(2, 1, bundles);
            TryOneTimeCoord(2, -1, bundles);
            TryOneTimeCoord(-1, -2, bundles);
            TryOneTimeCoord(1, -2, bundles);
            TryOneTimeCoord(-2, 1, bundles);
            TryOneTimeCoord(-2, -1, bundles);

            return bundles;
        }
    }
}
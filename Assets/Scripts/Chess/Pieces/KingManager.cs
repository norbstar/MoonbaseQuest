using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KingManager : PieceManager
    {
        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();

            TryOneTimeVector(-1, 0, bundles);
            TryOneTimeVector(-1, 1, bundles);
            TryOneTimeVector(1, 0, bundles);
            TryOneTimeVector(1, 1, bundles);
            TryOneTimeVector(0, -1, bundles);
            TryOneTimeVector(-1, -1, bundles);
            TryOneTimeVector(0, 1, bundles);
            TryOneTimeVector(1, -1, bundles);

            return bundles;
        }
    }
}
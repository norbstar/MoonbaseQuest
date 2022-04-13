using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KnightManager : PieceManager
    {
        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();

            TryOneTimeVector(-1, 2, bundles);
            TryOneTimeVector(1, 2, bundles);
            TryOneTimeVector(2, 1, bundles);
            TryOneTimeVector(2, -1, bundles);
            TryOneTimeVector(-1, -2, bundles);
            TryOneTimeVector(1, -2, bundles);
            TryOneTimeVector(-2, 1, bundles);
            TryOneTimeVector(-2, -1, bundles);

            return bundles;
        }
    }
}
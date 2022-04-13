using System.Collections.Generic;

namespace Chess.Pieces
{
    public class BishopManager : PieceManager
    {
        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();

            TryVector(-1, 1, bundles);
            TryVector(1, 1, bundles);
            TryVector(-1, -1, bundles);
            TryVector(1, -1, bundles);

            return bundles;
        }
    }
}
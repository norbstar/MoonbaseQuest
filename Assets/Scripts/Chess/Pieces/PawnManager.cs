using System.Collections.Generic;

namespace Chess.Pieces
{
    public class PawnManager : PieceManager
    {
        private bool hasHistory;

        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();
            int iterationCap = (!hasHistory) ? 2 : 1;
            
            TryVector(0, vector, bundles, new CoordSpec
            {
                includeOccupedCells = false
            }, iterationCap);
            
            TryOneTimeVector(-1, vector, bundles, new CoordSpec
            {
                includeOccupedCells = true,
                onlyIncludeOccupiedCells = true
            });

            TryOneTimeVector(1, vector, bundles, new CoordSpec
            {
                includeOccupedCells = true,
                onlyIncludeOccupiedCells = true
            });

            return bundles;
        }

        protected override void OnMove(Cell fromCell, Cell toCell, bool resetting)
        {
            if (!resetting)
            {
                hasHistory = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            hasHistory = false;
        }
    }
}
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class PawnManager : PieceManager
    {
        private bool hasHistory;

        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();
            // List<Coord> vectorCoords;

            // Coord activeCoord = ActiveCell.coord;
            int iterationCap = (!hasHistory) ? 2 : 1;
            
            // if (TryGetVectorCoords(activeCoord, 0, vector, out vectorCoords, iterationCap))
            // {
            //     bundles.Add(new CoordBundle
            //     {
            //         coords = vectorCoords,
            //         includeOccupedCells = false
            //     });
            // }

            TryCoord(0, vector, bundles, false, iterationCap);
            // TryOneTimeCoord(-1, vector, bundles);
            // TryOneTimeCoord(1, vector, bundles);

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
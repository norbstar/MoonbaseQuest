using System.Collections.Generic;

namespace Chess.Pieces
{
    public class PawnManager : PieceManager
    {
        private bool hasHistory;

        protected override List<List<Coord>> GenerateCoords(Cell[,] matrix, int vector)
        {
            List<List<Coord>> coords = new List<List<Coord>>();
            List<Coord> vectorCoords;

            Coord activeCoord = ActiveCell.coord;
            int iterationCap = (!hasHistory) ? 2 : 1;
            
            if (TryGetVectorCoords(activeCoord, 0, vector, out vectorCoords, iterationCap))
            {
                coords.Add(vectorCoords);
            }

            return coords;
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
using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KingManager : PieceManager
    {
        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix)
        {
            return new List<Cell>();
        }
    }
}
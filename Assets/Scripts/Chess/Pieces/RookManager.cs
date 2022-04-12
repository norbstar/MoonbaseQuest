using System.Collections.Generic;

namespace Chess.Pieces
{
    public class RookManager : PieceManager
    {
        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix)
        {
            return new List<Cell>();
        }
    }
}
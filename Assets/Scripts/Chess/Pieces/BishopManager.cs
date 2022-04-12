using System.Collections.Generic;

namespace Chess.Pieces
{
    public class BishopManager : PieceManager
    {
        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix)
        {
            return new List<Cell>();
        }
    }
}
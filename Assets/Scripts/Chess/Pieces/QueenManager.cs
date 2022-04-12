using System.Collections.Generic;

namespace Chess.Pieces
{
    public class QueenManager : PieceManager
    {
        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix)
        {
            return new List<Cell>();
        }
    }
}
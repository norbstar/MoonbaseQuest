using System.Collections.Generic;

namespace Chess.Pieces
{
    public class BishopManager : PieceManager
    {
        public override List<Cell> CalculateMoves(ChessBoardManager manager, Cell[,] matrix, int maxColumnIdx, int maxRowIdx, int vector)
        {
            List<Cell> moves = new List<Cell>();

            // TODO

            return moves;
        }
    }
}
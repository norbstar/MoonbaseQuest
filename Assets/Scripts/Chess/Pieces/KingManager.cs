using System.Collections.Generic;

namespace Chess.Pieces
{
    public class KingManager : PieceManager
    {
        public override List<Cell> CalculateMoves(ChessBoardManager manager, Cell[,] matrix, int maxColumnIdx, int maxRowIdx, int vector)
        {
            List<Cell> moves = new List<Cell>();

            // TODO

            return moves;
        }
    }
}
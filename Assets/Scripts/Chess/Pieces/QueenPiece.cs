using System.Collections.Generic;

namespace Chess.Pieces
{
    public class QueenPiece : Piece
    {
        public override List<Cell> CalculateMoves(Cell[,] matrix, int maxColumnIdx, int maxRowIdx, int vector)
        {
            List<Cell> moves = new List<Cell>();

            // TODO

            return moves;
        }
    }
}
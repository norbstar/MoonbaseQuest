using System.Collections.Generic;

namespace Chess.Pieces
{
    public class PawnPiece : Piece
    {
        private bool hasHistory;

        public override List<Cell> CalculateMoves(Cell[,] matrix, int maxColumnIdx, int maxRowIdx, int vector)
        {
            List<Cell> moves = new List<Cell>();

            // TODO

            // TEST
            moves.Add(matrix[3, 3]);

            hasHistory = true;

            return moves;
        }

        public override void Reset()
        {
            base.Reset();
            hasHistory = false;
        }
    }
}
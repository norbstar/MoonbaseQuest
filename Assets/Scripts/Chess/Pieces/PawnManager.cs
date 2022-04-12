using System.Collections.Generic;

namespace Chess.Pieces
{
    public class PawnManager : PieceManager
    {
        private bool hasHistory;

        // public override List<Cell> CalculateMoves(ChessBoardManager manager, Cell[,] matrix, int vector)
        // {
        //     List<Cell> moves = new List<Cell>();

        //     // TODO

        //     // TEST

        //     moves.Add(matrix[1, 3]);
        //     moves.Add(matrix[2, 2]);
        //     moves.Add(matrix[2, 4]);
        //     moves.Add(matrix[3, 3]);
        //     moves.Add(matrix[4, 2]);
        //     moves.Add(matrix[4, 4]);
        //     moves.Add(matrix[5, 3]);

        //     hasHistory = true;

        //     return moves;
        // }

        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix)
        {
            return new List<Cell>();
        }

        public override void Reset()
        {
            base.Reset();
            hasHistory = false;
        }
    }
}
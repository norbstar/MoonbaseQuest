using System.Collections.Generic;

namespace Chess.Pieces
{
    public class RookManager : PieceManager
    {
        // public override List<Cell> CalculateMoves(ChessBoardManager manager, Cell[,] matrix, int vector)
        // {
        //     List<Cell> moves = new List<Cell>();

        //     // TODO

        //     return moves;
        // }

        protected override List<Cell> ResolveAllAvailableQualifyingCells(Cell[,] matrix)
        {
            return new List<Cell>();
        }
    }
}
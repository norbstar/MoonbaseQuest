using System.Collections.Generic;

using UnityEngine;

namespace Chess.Pieces
{
    public class KnightManager : PieceManager
    {
        public override List<Cell> CalculateMoves(ChessBoardManager manager, Cell[,] matrix, int maxColumnIdx, int maxRowIdx, int vector)
        {
            List<Cell> moves = new List<Cell>();

            if (manager.TryGetSetPiecesByType(set, PieceType.King, out List<Cell> cells))
            {
                if (cells.Count > 0)
                {
                    var kingCell = cells[0];
                    Debug.Log("$Calculate Moves King Cell Coords : [{kingCell.coords.x} {kingCell.coords.y}]");
                }
            }

            return moves;
        }
    }
}
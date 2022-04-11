using System.Collections.Generic;

namespace Chess.Pieces
{
    public class PawnPiece : Piece
    {
        private bool hasHistory;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public override List<Cell> CalculateMoves()
        {
            List<Cell> moves = new List<Cell>();
            return moves;
        }

        public override void Reset()
        {
            hasHistory = false;
        }
    }
}
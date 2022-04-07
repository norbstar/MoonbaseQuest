using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class ChessBoardManager : MonoBehaviour
    {
        private class Cell
        {
            public Piece piece;
        }

        private Cell[,] cells;

        // Start is called before the first frame update
        void Start()
        {
            cells = new Cell[8, 8];
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
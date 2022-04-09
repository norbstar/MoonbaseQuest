    using UnityEngine;

    using Chess.Pieces;

    namespace Chess
    {
        public class Cell
        {
            public Coord coord;
            public Vector3 localPosition;
            public Piece piece;
        }
    }
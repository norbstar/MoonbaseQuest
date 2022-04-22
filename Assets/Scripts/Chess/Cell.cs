    using UnityEngine;

    using Chess.Pieces;

    namespace Chess
    {
        public class Cell
        {
            public Coord coord;
            public Vector3 localPosition;
            public PieceManager piece;

            public bool IsOccupied { get { return piece != null; } }

            // public Cell Clone()
            // {
            //     return new Cell
            //     {
            //         coord = coord,
            //         localPosition = localPosition,
            //         piece = piece.Clone()
            //     };
            // }
        }
    }
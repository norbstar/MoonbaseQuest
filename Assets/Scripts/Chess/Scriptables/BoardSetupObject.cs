using System;
using System.Collections.Generic;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class BoardSetupObject
    {
        [Serializable]
        public class Piece
        {
            public PieceManager manager;
            public Coord coord;
        }

        [Serializable]
        public class Setup
        {
            [SerializeField] List<Piece> lightPieces;
            public List<Piece> LightPieces { get { return lightPieces; } }

            [SerializeField] List<Piece> darkPieces;
            public List<Piece> DarkPieces { get { return darkPieces; } }
        }
    }
}
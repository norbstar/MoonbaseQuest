using System;
using System.Collections.Generic;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    [CreateAssetMenu(fileName = "Data", menuName = "Holo Chess/Board Setup", order = 1)]
    public class BoardSetupScriptable : ScriptableObject
    {
        [Serializable]
        public class Piece
        {
            public PieceManager manager;
            public Coord coord;
        }

        [SerializeField] List<Piece> lightPieces;
        public List<Piece> LightPieces { get { return lightPieces; } }

        [SerializeField] List<Piece> darkPieces;
        public List<Piece> DarkPieces { get { return darkPieces; } }
    }
}
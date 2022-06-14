using System;
using System.Collections.Generic;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class BoardTrackingObject : ScriptableObject
    {
        [Serializable]
        public class TakenPiece
        {
            public PieceType pieceType;
            public Vector3 slot;
        }

        [Serializable]
        public class Move
        {
            public PieceType pieceType;
            public Coord from;
            public Coord to;
        }

        [Serializable]
        public class Sequence
        {
            public Set set;
            public List<Move> moves;
            public bool taken;
            public TakenPiece takenPiece;
            public KingManager.State opposingState;

            public Sequence() => moves = new List<Move>();
        }

        [Serializable]
        public class Tracking
        {
            public List<Sequence> sequences;

            public Tracking() => sequences = new List<Sequence>();
        }
    }
}
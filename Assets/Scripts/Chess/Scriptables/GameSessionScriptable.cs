using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    [CreateAssetMenu(fileName = "Data", menuName = "Holo Chess/Game Session", order = 2)]
    public class GameSessionScriptable : ScriptableObject
    {
        [Serializable]
        public class Snapshot
        {
            public Set lightSet;
            public Set darkSet;

            public Snapshot()
            {
                lightSet = new Set();
                darkSet = new Set();
            }
        }

        [Serializable]
        public class Set
        {
            public List<Piece> pieces;
            public KingManager.State state;

            public Set() => pieces = new List<Piece>();
        }

        [Serializable]
        public class Piece
        {
            public Pieces.PieceType type;
            public Coord coord;
        }

        [Serializable]
        public class Move
        {
            public Piece piece;
            public Coord to;
        }

        public List<Snapshot> snapshots = new List<Snapshot>();
        public List<Move> moves = new List<Move>();

        public void SaveAsset(string filename)
        {
            AssetDatabase.CreateAsset(this, AssetFunctions.BuildPath(AssetFunctions.Path.Resources, $"{filename}.gs"));
            AssetDatabase.SaveAssets();
        }

        public object LoadAsset(string filename, Type type)
        {
            return AssetDatabase.LoadAssetAtPath(filename, type);
        }

        public void Clear()
        {
            snapshots.Clear();
            moves.Clear();
        }
    }
}
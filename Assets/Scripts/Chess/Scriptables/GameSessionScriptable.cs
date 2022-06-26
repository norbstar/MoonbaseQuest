#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using System.IO;

using UnityEngine;

using UnityEditor;

using Chess.Pieces;

namespace Chess.Scriptable
{
    [CreateAssetMenu(fileName = "Data", menuName = "Holo Chess/Game Session", order = 2)]
    public class GameSessionScriptable : ScriptableObject
    {
        [Serializable]
        public class BoardLayout
        {
            public Set lightSet;
            public Set darkSet;

            public string Serialize(Set set) => JsonUtility.ToJson(set);

            public Set Deserialize(string json) => JsonUtility.FromJson<Set>(json);

            public BoardLayout()
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
        public class KingStates
        {
            public KingManager.State lightKingState;
            public KingManager.State darkKingState;
        }

        [Serializable]
        public class Move
        {
            public Piece piece;
            public Coord to;
            public KingStates kingStates;
        }

        private BoardLayout layout = new BoardLayout();
        private List<Move> moves = new List<Move>();
        public List<Move> Moves { get { return moves; } }

        public void SetLayout(BoardLayout layout) => this.layout = layout;

        public void DeleteAssetIfExists(string filename)
        {
            if (AssetExists(filename))
            {
                DeleteAsset(filename);
            }
        }

        public bool AssetExists(string filename) => File.Exists(AssetFunctions.BuildPath(AssetFunctions.Path.Resources, $"{filename}.gs"));

        public object LoadAsset(string filename, System.Type type) => AssetDatabase.LoadAssetAtPath(filename, type);
        
        public void SaveAsset(string filename, bool deleteExisting = false)
        {
            if (deleteExisting)
            {
                DeleteAssetIfExists(filename);
            }

            AssetDatabase.CreateAsset(this, AssetFunctions.BuildPath(AssetFunctions.Path.Resources, $"{filename}.gs"));
            AssetDatabase.SaveAssets();
        }

        public void DeleteAsset(string filename) => AssetDatabase.DeleteAsset(AssetFunctions.BuildPath(AssetFunctions.Path.Resources, $"{filename}.gs"));

        public void Reset()
        {
            layout = new BoardLayout();
            moves.Clear();
        }
    }
}
#endif
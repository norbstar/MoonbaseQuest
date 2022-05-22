using System;
using System.Collections.Generic;

// using System.IO;

using UnityEngine;

// #if UNITY_EDITOR
// using UnityEditor;
// #endif

namespace Chess.Scriptable
{
    [CreateAssetMenu(fileName = "Data", menuName = "Holo Chess/Piece Session", order = 3)]
    public class PieceSessionScriptable : ScriptableObject
    {
        [Serializable]
        public class SessionNode
        {
            public int id;
        }

        [Serializable]
        public class MoveToCellNode : SessionNode
        {
            public Coord coord;
        }

        public class MoveToSlotNode : SessionNode
        {
            public Vector3 localPosition;
        }

        public class DestroyNode : SessionNode
        {
            public Vector3 localPosition;
        }

        [Serializable]
        public class EventNode : SessionNode
        {
            
        }
        
        public Coord start;
        public List<SessionNode> session;

        void Awake() => session = new List<SessionNode>();

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
using System;
using System.Collections.Generic;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class GameSessionManager : MonoBehaviour
    {
        [Serializable]
        public class Data
        {
            public List<PieceSessionManager.Data> pieces;

            public Data()
            {
                pieces = new List<PieceSessionManager.Data>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }
    }
}
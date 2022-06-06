using System;
using System.Collections.Generic;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    [CreateAssetMenu(fileName = "Data", menuName = "Holo Chess/Board Tracking", order = 2)]
    public class BoardTrackingScriptable : BoardSetupScriptable
    {
        // [Serializable]
        // public class Moves
        // {
        //     public List<MoveData> collection;

        //     public _Moves() => collection = new List<MoveData>();
        // }

        // private _Moves moves;

        // public _Moves Moves { get { return moves; } }
    }
}
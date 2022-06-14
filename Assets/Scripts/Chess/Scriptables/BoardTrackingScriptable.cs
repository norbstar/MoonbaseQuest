using System;

using UnityEngine;

namespace Chess
{
    [CreateAssetMenu(fileName = "Data", menuName = "Holo Chess/Board Tracking", order = 2)]
    public class BoardTrackingScriptable : BoardScriptable
    {
        [Serializable]
        public class Data
        {
            public BoardSetupObject.Setup setup;
            public BoardTrackingObject.Tracking tracking;

            public Data()
            {
                setup = new BoardSetupObject.Setup();
                tracking = new BoardTrackingObject.Tracking();
            }
        }
        
        public Data data;
    }
}
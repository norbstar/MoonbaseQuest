using System;

using UnityEngine;

namespace Chess
{
    [CreateAssetMenu(fileName = "Data", menuName = "Holo Chess/Board Setup", order = 1)]
    public class BoardSetupScriptable : BoardScriptable
    {
        [Serializable]
        public class Data
        {
            public BoardSetupObject.Setup setup;

            public Data() => setup = new BoardSetupObject.Setup();
        }

        public Data data;
    }
}
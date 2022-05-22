using System;
using System.Collections.Generic;

using UnityEngine;

namespace Tests
{
    [CreateAssetMenu(fileName = "Data", menuName = "Tests/Persistence Test", order = 1)]
    public class PersistenceTestScriptable : ScriptableObject
    {
        [Serializable]
        public class _Data
        {
            public List<PersistenceObjectScriptable.ObjectData> objectData;

            public _Data() => objectData = new List<PersistenceObjectScriptable.ObjectData>();
        }

        private _Data data;

        void Awake() => data = new _Data();

        public _Data Data { get { return data;  } }

        public void AddData(PersistenceObjectScriptable.ObjectData objectData) => this.data.objectData.Add(objectData);
    }
}
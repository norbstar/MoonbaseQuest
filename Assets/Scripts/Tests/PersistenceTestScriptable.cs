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
            public List<PersistenceObjectScriptable.ObjectData> collection;

            public _Data() => collection = new List<PersistenceObjectScriptable.ObjectData>();
        }

        private _Data data;

        void Awake() => data = new _Data();

        public _Data Data { get { return data;  } set { data = value; } }

        public void AddData(PersistenceObjectScriptable.ObjectData collection) => this.data.collection.Add(collection);
    }
}
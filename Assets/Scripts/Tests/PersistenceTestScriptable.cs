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

        public _Data Data { get { return data; } }

        public void SetData(_Data data) => this.data = data;

        public void AddData(PersistenceObjectScriptable.ObjectData collection)
        {
            if (data == null)
            {
                data = new _Data();
            }

            data.collection.Add(collection);
        }
    }
}
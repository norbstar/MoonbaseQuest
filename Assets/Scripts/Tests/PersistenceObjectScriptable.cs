using System;

using UnityEngine;

namespace Tests
{
    [CreateAssetMenu(fileName = "Data", menuName = "Tests/Persistence Object", order = 2)]
    public class PersistenceObjectScriptable : ScriptableObject
    {
        [Serializable]
        public class ObjectData
        {
            public string name;
            public Vector3 localPosition;
        }
    }
}
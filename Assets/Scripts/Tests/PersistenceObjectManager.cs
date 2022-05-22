using UnityEngine;

namespace Tests
{
    public class PersistenceObjectManager : MonoBehaviour
    {
        public PersistenceObjectScriptable.ObjectData GetObjectData()
        {
            return new PersistenceObjectScriptable.ObjectData
            {
                name = gameObject.name,
                localPosition = transform.localPosition
            };
        }
    }
}
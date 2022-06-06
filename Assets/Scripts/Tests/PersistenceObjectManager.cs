using UnityEngine;

namespace Tests
{
    public class PersistenceObjectManager : MonoBehaviour
    {
        public PersistenceObjectScriptable.ObjectData GetData()
        {
            return new PersistenceObjectScriptable.ObjectData
            {
                name = gameObject.name,
                localPosition = transform.localPosition
            };
        }
    }
}
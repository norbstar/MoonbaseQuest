using UnityEngine;

namespace Chess
{
    public interface IPropagationEvent
    {
        public void OnUpdate(Vector3 position, float radius);
    }
}
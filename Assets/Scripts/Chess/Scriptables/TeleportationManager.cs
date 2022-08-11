using UnityEngine;

namespace Chess
{
    public class TeleportationManager : MonoBehaviour
    {
        [SerializeField] GameObject reference;

        public void OnTriggerEnter(Collider collider)
        {
            var collisionPoint = collider.ClosestPoint(transform.position);
            Debug.Log($"{Time.time} {gameObject.name} Closest Point : {collisionPoint}");

            reference.transform.position = collisionPoint;
        }
    }
}
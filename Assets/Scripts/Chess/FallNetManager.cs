using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class FallNetManager : MonoBehaviour
    {
        private void OnTriggerEnter(Collider collider)
        {
            GameObject trigger = collider.gameObject;

            if (trigger.CompareTag("Chess Piece"))
            {
                if (trigger.TryGetComponent<Piece>(out Piece piece))
                {
                    piece.SnapToActiveCell();
                }
            }
        }
    }
}
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
                if (trigger.TryGetComponent<PieceManager>(out PieceManager piece))
                {
                    if (piece.ActiveCell != null)
                    {
                        piece.SnapToActiveCell();
                    }
                }
            }
        }
    }
}
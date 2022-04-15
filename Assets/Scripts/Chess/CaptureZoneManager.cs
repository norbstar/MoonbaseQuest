using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class CaptureZoneManager : MonoBehaviour
    {
        [SerializeField] Transform[] slots;

        private int nextSlot;

        public bool TryReserveSlot(PieceManager piece, out Vector3 localPosition)
        {
            if (nextSlot < slots.Length)
            {
                localPosition = slots[nextSlot].localPosition;
                ++nextSlot;
                return true;
            }

            localPosition = default(Vector3);
            return false;
        }

        public void Reset() => nextSlot = 0;
    }
}
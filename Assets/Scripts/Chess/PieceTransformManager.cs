using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class PieceTransformManager : MonoBehaviour
    {
        public enum Action
        {
            Raise,
            Lower
        }

        [Header("Animations")]
        [SerializeField] PieceTransformAnimationManager animationManager;

        [Header("Prefabs")]
        [SerializeField] GameObject pawn;
        [SerializeField] GameObject queen;
        [SerializeField] GameObject knight;
        [SerializeField] GameObject rook;
        [SerializeField] GameObject bishop;

        public delegate void Event(PieceTransformManager.Action action);
        public static event Event CompleteEventReceived;

        public void ShowAndRaisePiece(Vector3 position)
        {
            SetPiece(PieceType.Pawn);
            
            transform.position = position;
            gameObject.SetActive(true);
            animationManager.Invoke(Action.Raise);
        }

        public void SetPiece(PieceType type)
        {
            switch (type)
            {
                case PieceType.Pawn:
                    pawn.SetActive(true);
                    queen.SetActive(false);
                    knight.SetActive(false);
                    rook.SetActive(false);
                    bishop.SetActive(false);
                    break;

                case PieceType.Queen:
                    pawn.SetActive(false);
                    queen.SetActive(true);
                    knight.SetActive(false);
                    rook.SetActive(false);
                    bishop.SetActive(false);
                    break;

                case PieceType.Knight:
                    pawn.SetActive(false);
                    queen.SetActive(false);
                    knight.SetActive(true);
                    rook.SetActive(false);
                    bishop.SetActive(false);
                    break;

                case PieceType.Rook:
                    pawn.SetActive(false);
                    queen.SetActive(false);
                    knight.SetActive(false);
                    rook.SetActive(true);
                    bishop.SetActive(false);
                    break;

                case PieceType.Bishop:
                    pawn.SetActive(false);
                    queen.SetActive(false);
                    knight.SetActive(false);
                    rook.SetActive(false);
                    bishop.SetActive(true);
                    break;
            }
        }

        public void LowerAndHidePiece() => animationManager.Invoke(Action.Lower);

        public void OnRaiseComplete()
        {
            CompleteEventReceived?.Invoke(PieceTransformManager.Action.Raise);
        }

        public void OnLowerComplete()
        {
            gameObject.SetActive(false);
            CompleteEventReceived?.Invoke(PieceTransformManager.Action.Lower);
        }
    }
}
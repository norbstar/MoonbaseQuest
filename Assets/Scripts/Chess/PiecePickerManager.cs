using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

using Chess.Pieces;

namespace Chess
{
    public class PiecePickerManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] List<PieceManager> pieces;

        [Header("Prefabs")]
        [SerializeField] PieceManager lightQueen;
        [SerializeField] PieceManager darkQueen;
        [SerializeField] PieceManager lightKnight;
        [SerializeField] PieceManager darkKnight;
        [SerializeField] PieceManager lightRook;
        [SerializeField] PieceManager darkRook;
        [SerializeField] PieceManager lightBishop;
        [SerializeField] PieceManager darkBishop;

        public delegate void Event(PieceManager piece);
        public static event Event EventReceived;

        private PieceManager inFocusPiece;

        void OnEnable()
        {
            HandController.ActuationEventReceived += OnActuation;

            foreach (PieceManager piece in pieces)
            {
                piece.EventReceived += OnEvent;
            }
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;

            foreach (PieceManager piece in pieces)
            {
                piece.EventReceived -= OnEvent;
            }
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        private void OnEvent(PieceManager piece, FocusType focusType)
        {
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    piece.ShowOutline();
                    inFocusPiece = piece;
                    break;

                case FocusType.OnFocusLost:
                    piece.HideOutline();
                    inFocusPiece = null;
                    break;
            }
        }

        public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
        {
            if (inFocusPiece == null) return;

            if (actuation.HasFlag(Actuation.Button_AX))
            {
                PieceManager piece = null;

                switch(inFocusPiece.Type)
                {
                    case PieceType.Queen:
                        piece = (inFocusPiece.Set == Set.Light) ? lightQueen : darkQueen;
                        break;
                    
                    case PieceType.Knight:
                        piece = (inFocusPiece.Set == Set.Light) ? lightKnight : darkKnight;
                        break;

                    case PieceType.Rook:
                        piece = (inFocusPiece.Set == Set.Light) ? lightRook : darkRook;
                        break;

                    case PieceType.Bishop:
                        piece = (inFocusPiece.Set == Set.Light) ? lightBishop : darkBishop;
                        break;
                }

                EventReceived?.Invoke(piece);
            }
        }
    }
}
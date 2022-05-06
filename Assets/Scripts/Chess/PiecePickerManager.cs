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

        public void ConfigureAndShow(Set set)
        {
            foreach (PieceManager piece in pieces)
            {
                piece.Set = set;
            }

            Show();
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
                PieceManager piece = ResolvePieceBySet(inFocusPiece.Set, inFocusPiece.Type);
                EventReceived?.Invoke(piece);
            }
        }

        public PieceManager ResolvePieceBySet(Set set, PieceType type)
        {
            PieceManager piece = null;

            switch(type)
            {
                case PieceType.Queen:
                    piece = (set == Set.Light) ? lightQueen : darkQueen;
                    break;
                
                case PieceType.Knight:
                    piece = (set == Set.Light) ? lightKnight : darkKnight;
                    break;

                case PieceType.Rook:
                    piece = (set == Set.Light) ? lightRook : darkRook;
                    break;

                case PieceType.Bishop:
                    piece = (set == Set.Light) ? lightBishop : darkBishop;
                    break;
            }

            return piece;
        }

        public PieceType PickRandomType()
        {
            int pieceIdx = Random.Range(0, pieces.Count);
            return pieces[pieceIdx].Type;
        }
    }
}
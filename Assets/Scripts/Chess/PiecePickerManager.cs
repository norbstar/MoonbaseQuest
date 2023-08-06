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
            HandController.InputChangeEventReceived += OnActuation;

            foreach (PieceManager piece in pieces)
            {
                piece.EventReceived += OnEvent;
            }
        }

        void OnDisable()
        {
            HandController.InputChangeEventReceived -= OnActuation;

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

        private void OnEvent(PieceManager manager, FocusType focusType)
        {
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    manager.ShowOutline();
                    inFocusPiece = manager;
                    break;

                case FocusType.OnFocusLost:
                    manager.HideOutline();
                    inFocusPiece = null;
                    break;
            }
        }

        public void OnActuation(HandController controller, Enum.ControllerEnums.Input actuation, InputDeviceCharacteristics characteristics)
        {
            if (inFocusPiece == null) return;

            if (actuation.HasFlag(Enum.ControllerEnums.Input.Button_AX))
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
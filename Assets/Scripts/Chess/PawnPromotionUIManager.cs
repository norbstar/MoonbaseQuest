using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.XR;

using static Enum.ControllerEnums;

using Chess.Pieces;

namespace Chess
{
    public class PawnPromotionUIManager : MonoBehaviour
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Serializable]
        public class Element
        {
            public FocusableManager manager;
            public PieceType type;
        }

        [Header("Components")]
        [SerializeField] List<Element> elements;

        [Header("Prefabs")]
        [SerializeField] PieceManager lightQueen;
        [SerializeField] PieceManager darkQueen;
        [SerializeField] PieceManager lightKnight;
        [SerializeField] PieceManager darkKnight;
        [SerializeField] PieceManager lightRook;
        [SerializeField] PieceManager darkRook;
        [SerializeField] PieceManager lightBishop;
        [SerializeField] PieceManager darkBishop;

        public delegate void Event(Set set, PieceManager piece);
        public static event Event EventReceived;

        [Header("Config")]
        [SerializeField] AudioClip inFocusAudioClip;
        [SerializeField] AudioClip onSelectAudioClip;

        // private ChessBoardManager chessBoardManager;
        private Element inFocusElement;
        private Set set;

        void OnEnable()
        {
            HandController.ActuationEventReceived += OnActuation;

            foreach (Element element in elements)
            {
                element.manager.EventReceived += OnEvent;
            }
        }

        void OnDisable()
        {
            HandController.ActuationEventReceived -= OnActuation;

            foreach (Element element in elements)
            {
                element.manager.EventReceived -= OnEvent;
            }
        }

        public void Show(Set set) => ConfigureAndActivate(set);

        public void Activate() => gameObject.SetActive(true);

        public void ConfigureAndActivate(Set set)
        {
            this.set = set;
            Activate();
        }

        public void Hide() => Deactivate();

        public void Deactivate() => gameObject.SetActive(false);

        private void OnEvent(FocusableManager manager, FocusType focusType)
        {
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    AudioSource.PlayClipAtPoint(inFocusAudioClip, transform.position, 1.0f);
                    inFocusElement = ResolveElement(manager);
                    break;

                case FocusType.OnFocusLost:
                    inFocusElement = null;
                    break;
            }
        }

        public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
        {
            if (inFocusElement == null) return;

            if (actuation.HasFlag(Actuation.Trigger))
            {
                AudioSource.PlayClipAtPoint(onSelectAudioClip, transform.position, 1.0f);
                PieceManager piece = ResolvePieceBySet(set, inFocusElement.type);
                EventReceived?.Invoke(set, piece);
            }
        }

        private Element ResolveElement(FocusableManager manager)
        {
            foreach (Element element in elements)
            {
                if (element.manager.GetInstanceID() == manager.GetInstanceID())
                {
                    return element;
                }
            }

            return null;
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
            int pieceIdx = UnityEngine.Random.Range(0, elements.Count);
            return elements[pieceIdx].type;
        }
    }
}
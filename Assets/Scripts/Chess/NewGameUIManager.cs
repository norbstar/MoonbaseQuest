using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;

using TMPro;

using static Enum.ControllerEnums;

namespace Chess
{
    public class NewGameUIManager : MonoBehaviour
    {
        [Serializable]
        public class Element
        {
            public FocusableManager manager;
            public NewGameManager.Mode mode;
        }

        [Header("Components")]
        [SerializeField] List<Element> elements;
        [SerializeField] TextMeshProUGUI modeTextUI;

        [Header("Config")]
        [SerializeField] AudioClip inFocusAudioClip;
        [SerializeField] AudioClip onSelectAudioClip;

        public delegate void Event(NewGameManager.Mode mode);
        public static event Event EventReceived;

        private Element inFocusElement;

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

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

         private void OnEvent(FocusableManager manager, FocusType focusType)
        {
            switch (focusType)
            {
                case FocusType.OnFocusGained:
                    AudioSource.PlayClipAtPoint(inFocusAudioClip, transform.position, 1.0f);

                    inFocusElement = ResolveElement(manager);

                    switch (inFocusElement.mode)
                    {
                        case NewGameManager.Mode.PVP:
                            modeTextUI.text = "Player v Player";
                            break;

                        case NewGameManager.Mode.PVB:
                            modeTextUI.text = "Player v Bot";
                            break;

                        case NewGameManager.Mode.BVB:
                            modeTextUI.text = "Bot v Bot";
                            break;
                    }
                    break;

                case FocusType.OnFocusLost:
                    inFocusElement = null;
                    modeTextUI.text = String.Empty;
                    break;
            }
        }
        
        public void OnActuation(Actuation actuation, InputDeviceCharacteristics characteristics)
        {
            if (inFocusElement == null) return;

            if (actuation.HasFlag(Actuation.Trigger))
            {
                AudioSource.PlayClipAtPoint(onSelectAudioClip, transform.position, 1.0f);
                EventReceived?.Invoke(inFocusElement.mode);
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
    }
}
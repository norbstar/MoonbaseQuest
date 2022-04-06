using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SimonSays
{
    public class SimonSaysManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] SimonSaysStartSpringButtonManager startButton;
        [SerializeField] List<SimonSaysSpringButtonManager> buttons;

        [Header("Audio")]
        [SerializeField] AudioClip buttonPressClip;
        [SerializeField] AudioClip endDemoClip;
        [SerializeField] AudioClip passClip;
        [SerializeField] AudioClip failClip;

        private class Sequence
        {
            // Represents the timeframe in which the sequence needs to be completed
            public float timeframe;

            // The sequence itself detailed as a simple list of Ids
            public IList<Id> sequence;

            public Sequence() => sequence = new List<Id>();
        }

        private float startingTimeframePerButton = 1f;
        private int startingFrequency = 1;
        
        private float timeframePerButton;
        private int frequency;
        private Coroutine testSequenceCoroutine;
        private Sequence sequence;
        private bool testInProgress, testComplete;

        // Start is called before the first frame update
        void Start()
        {
            ResetGame();
        }

        void OnEnable()
        {
            SimonSaysStartSpringButtonManager.EventReceived += OnStartButtonEvent;
            SimonSaysSpringButtonManager.EventReceived += OnButtonEvent;
        }

        void OnDisable()
        {
            SimonSaysStartSpringButtonManager.EventReceived -= OnStartButtonEvent;
            SimonSaysSpringButtonManager.EventReceived -= OnButtonEvent;
        }

        public void ResetGame()
        {
            timeframePerButton = startingTimeframePerButton;
            frequency = startingFrequency;
            testInProgress = testComplete = false;
            startButton.gameObject.SetActive(true);
        }

        private Sequence CreateSequence()
        {
            IList<Id> sequence = new List<Id>();

            for (int itr = 0; itr < frequency; itr++)
            {
                int idx = Random.Range(0, buttons.Count);
                var buttonId = buttons[idx].ButtonId;
                sequence.Add(buttonId);
            }

            return new Sequence
            {
                timeframe = sequence.Count * timeframePerButton,
                sequence = sequence
            };
        }

        private IEnumerator SubmitSequenceCoroutine(Sequence sequence)
        {
            yield return StartCoroutine(DemoSequenceCoroutine(sequence));
            testSequenceCoroutine = StartCoroutine(TestSequenceCoroutine(sequence));
        }

        private IEnumerator DemoSequenceCoroutine(Sequence sequence)
        {
            foreach (Id id in sequence.sequence)
            {
                var button = ResolveButtonById(id);

                if (button != null)
                {
                    AudioSource.PlayClipAtPoint(buttonPressClip, transform.position, 1.0f);
                    yield return StartCoroutine(button.SimulateButtonPressCoroutine(timeframePerButton));
                }
            }

            AudioSource.PlayClipAtPoint(endDemoClip, transform.position, 1.0f);
        }

        private IEnumerator TestSequenceCoroutine(Sequence sequence)
        {
            testInProgress = true;
            yield return new WaitForSeconds(sequence.timeframe);

            if (!testComplete) FailTest();
        }

        private void PassTest()
        {
            testComplete = true;
            testInProgress = false;

            AudioSource.PlayClipAtPoint(passClip, transform.position, 1.0f);

            // TODO
        }

        private void FailTest()
        {
            StopCoroutine(testSequenceCoroutine);
            testInProgress = false;

            AudioSource.PlayClipAtPoint(failClip, transform.position, 1.0f);

            // TODO

            ResetGame();
        }

        private SimonSaysSpringButtonManager ResolveButtonById(Id id)
        {
            return buttons.SingleOrDefault(b => b.ButtonId == id);
        }

        private void OnButtonEvent(Id id, EventType eventType)
        {
            if (testInProgress && (eventType == EventType.OnPressed))
            {
                AudioSource.PlayClipAtPoint(buttonPressClip, transform.position, 1.0f);

                if (!HasElements(sequence.sequence))
                {
                    PassTest();
                    return;
                }

                if (id == sequence.sequence[0])
                {
                    sequence.sequence.RemoveAt(0);
                }
                else
                {
                    FailTest();
                }
            }
        }

        private void OnStartButtonEvent(EventType eventType)
        {
            startButton.gameObject.SetActive(false);
            sequence = CreateSequence();
            StartCoroutine(SubmitSequenceCoroutine(sequence));
        }

        private bool HasElements(IList<Id> elements)
        {
            return elements.Count > 0;
        }
    }
}
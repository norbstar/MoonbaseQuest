using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SimonSays.Simplified
{
    public class SimplifiedSimonSaysManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] SimonSaysStartButtonManager startButton;
        [SerializeField] GameObject playArea;
        [SerializeField] List<SimonSaysButtonManager> buttons;
        [SerializeField] SimonSaysUIManager uiManager;

        [Header("Audio")]
        [SerializeField] AudioClip readyClip;
        [SerializeField] AudioClip setClip;
        [SerializeField] AudioClip goClip;
        [SerializeField] AudioClip buttonPressClip;
        [SerializeField] AudioClip startDemoClip;
        [SerializeField] AudioClip endDemoClip;
        [SerializeField] AudioClip passClip;
        [SerializeField] AudioClip failClip;
        [SerializeField] AudioClip fxAClip;
        [SerializeField] AudioClip fxBClip;
        [SerializeField] AudioClip gameOverClip;

        [Header("Config")]
        [SerializeField] int startingFrequency = 1;
        [SerializeField] int maxIterationsAtFrequency = 3;
        [SerializeField] float waitTimeBetweenSequence = 0.5f;
        [SerializeField] float minTimeframePerButton = 0.5f;
        [SerializeField] float maxTimeframePerButton = 1f;
        [SerializeField] float timeframePerButtonModifierPercentage = 0.05f;

        private class Sequence
        {
            // Represents the timeframe in which the sequence needs to be completed to pass the round
            public float timeframe;

            // The sequence itself detailed as a simple list of Ids representing the buttons
            public IList<Id> sequence;

            public Sequence() => sequence = new List<Id>();
        }

        private enum FailReason
        {
            WrongButton,
            Timeout
        }

        private float timeframePerButton;
        private int frequency;
        private Coroutine testSequenceCoroutine;
        private Sequence sequence;
        private int? iterationsAtFrequency;
        private bool testInProgress, testComplete;

        // Start is called before the first frame update
        void Start()
        {
            ResetGame();
        }

        void OnEnable()
        {
            SimonSaysStartButtonManager.EventReceived += OnStartButtonEvent;
            SimonSaysButtonManager.EventReceived += OnButtonEvent;
        }

        void OnDisable()
        {
            SimonSaysStartButtonManager.EventReceived -= OnStartButtonEvent;
            SimonSaysButtonManager.EventReceived -= OnButtonEvent;
        }

        public void ResetGame()
        {
            timeframePerButton = maxTimeframePerButton;
            frequency = startingFrequency;
            iterationsAtFrequency = null;
            testInProgress = testComplete = false;
            startButton.gameObject.SetActive(true);
            uiManager.MainTextUI = "Simon Says";
            uiManager.ScoreTextUI = "0";
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
            yield return new WaitForSeconds(waitTimeBetweenSequence);
            yield return StartCoroutine(DemoSequenceCoroutine(sequence));
            testSequenceCoroutine = StartCoroutine(TestSequenceCoroutine(sequence));
        }

        private IEnumerator DemoSequenceCoroutine(Sequence sequence)
        {
            AudioSource.PlayClipAtPoint(startDemoClip, transform.position, 1.0f);

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

            FailTest(FailReason.Timeout);
        }

        private void PassTest()
        {
            StopCoroutine(testSequenceCoroutine);

            testComplete = true;
            testInProgress = false;

            AudioSource.PlayClipAtPoint(passClip, transform.position, 1.0f);

            --iterationsAtFrequency;

            if (iterationsAtFrequency == 0)
            {
                iterationsAtFrequency = null;
                ++frequency;

                if (timeframePerButton > minTimeframePerButton)
                {
                    float range = timeframePerButton - minTimeframePerButton;
                    float newTimeframePerButton = timeframePerButton - (range * timeframePerButtonModifierPercentage);
                    timeframePerButton = (newTimeframePerButton <= minTimeframePerButton) ? minTimeframePerButton : newTimeframePerButton; 
                }
            }

            StartTest();
        }

        private void FailTest(FailReason reason)
        {
            StopCoroutine(testSequenceCoroutine);

            testComplete = true;
            testInProgress = false;

            int score = PenaliseScore();

            if (score > 0)
            {
                StartTest();
                return;
            }

            AudioSource.PlayClipAtPoint(failClip, transform.position, 1.0f);
            uiManager.MainTextUI = "Game Over";
            
            StartCoroutine(ResetGameCoroutine());
        }

        private IEnumerator ResetGameCoroutine()
        {
            yield return new WaitForSeconds(1f);
            ResetGame();
        }

        private SimonSaysButtonManager ResolveButtonById(Id id)
        {
            return buttons.SingleOrDefault(b => b.ButtonId == id);
        }

        private void OnButtonEvent(Id id, EventType eventType)
        {
            if (testInProgress && (eventType == EventType.OnPressed))
            {
                AudioSource.PlayClipAtPoint(buttonPressClip, transform.position, 1.0f);

                if (!HasElements(sequence.sequence)) return;

                if (id == sequence.sequence[0])
                {
                    sequence.sequence.RemoveAt(0);

                    AddToScore();

                    if (!HasElements(sequence.sequence))
                    {
                        PassTest();
                    }
                }
                else
                {
                    FailTest(FailReason.WrongButton);
                }
            }
        }

        private int AddToScore()
        {
            int score;

            if (int.TryParse(uiManager.ScoreTextUI, out score))
            {
                ++score;

                uiManager.ScoreTextUI = score.ToString();

                bool fxA = (score % 3 == 0);
                bool fxB = (score % 5 == 0);

                if (fxA)
                {
                    AudioSource.PlayClipAtPoint(fxAClip, transform.position, 1.0f);
                }

                if (fxB)
                {
                    AudioSource.PlayClipAtPoint(fxBClip, transform.position, 1.0f);
                }
            }
            
            return score;
        }

        private int PenaliseScore()
        {
            AudioSource.PlayClipAtPoint(gameOverClip, transform.position, 1.0f);

            int score;

            if (int.TryParse(uiManager.ScoreTextUI, out score))
            {
                --score;
                uiManager.ScoreTextUI = score.ToString();
            }

            return score;
        }

        private void OnStartButtonEvent(EventType eventType)
        {
            startButton.gameObject.SetActive(false);
            StartGame();
        }

        private void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            uiManager.MainTextUI = "Ready";
            AudioSource.PlayClipAtPoint(readyClip, transform.position, 1.0f);
            yield return new WaitForSeconds(1f);

            uiManager.MainTextUI = "Set";
            AudioSource.PlayClipAtPoint(setClip, transform.position, 1.0f);
            yield return new WaitForSeconds(1f);

            uiManager.MainTextUI = "Go";
            AudioSource.PlayClipAtPoint(goClip, transform.position, 1.0f);
            yield return new WaitForSeconds(1f);

            uiManager.MainTextUI = "Simon Says";

            StartTest();
        }

        private void StartTest()
        {
            if (!iterationsAtFrequency.HasValue)
            {
                iterationsAtFrequency = Random.Range(0, maxIterationsAtFrequency) + 1;
            }

            sequence = CreateSequence();
            StartCoroutine(SubmitSequenceCoroutine(sequence));
        }

        private bool HasElements(IList<Id> elements)
        {
            return elements.Count > 0;
        }
    }
}
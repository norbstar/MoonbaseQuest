using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class TestCaseRunner : MonoBehaviour
{
    public delegate void Event(State state, object data = null);
    public static event Event EventReceived;
    
    public enum State
    {
        Start,
        Pass,
        Fail,
        End
    }

    public enum Result
    {
        Pass,
        Fail
    }

    [Serializable]
    public class DataPoint
    {
        public string expected;
        public string posted;
        public bool pass;
    }

    [Header("Components")]
    [SerializeField] GameObject box;
    [SerializeField] List<GameObject> faces;
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI stateTextUI;
    [SerializeField] TextMeshProUGUI countTextUI;

    [Header("Progression")]
    [SerializeField] Sprite inProgressSprite;

    [Header("Pass Scenario")]
    [SerializeField] Sprite passSprite;
    [SerializeField] AudioClip passAudio;

    [Header("Fail Scenario")]
    [SerializeField] Sprite failSprite;
    [SerializeField] AudioClip failAudio;

    [Header("Results")]
    [SerializeField] private List<DataPoint> resultSequence;

    private static TestCaseRunner instance;

    private new Camera camera;
    private List<string> sequence;
    private FX.RotateFX rotateFX;
    private List<string> postedSequence;
    private bool complete;

    public static TestCaseRunner GetInstance()
    {
        if (instance == null)
        {
            var obj = GameObject.Find("Test Case Runner");
            instance = obj?.GetComponent<TestCaseRunner>() as TestCaseRunner;
        }

        return instance;
    }

    void Awake()
    {
        ResolveDependencies();
        postedSequence = new List<string>();
    }

    private void ResolveDependencies()
    {
        camera = Camera.main;
        rotateFX = box.GetComponent<FX.RotateFX>() as FX.RotateFX;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePosition = camera.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        canvas.transform.rotation = rotation;
    }

    public void SetExpectedSequence(List<string> sequence)
    {
        this.sequence = sequence;
    }

    public void Post(string data)
    {
        if (complete) return;

        if (postedSequence.Count == 0)
        {
            foreach (GameObject face in faces)
            {
                var spriteRenderer = face.GetComponent<SpriteRenderer>() as SpriteRenderer;
                spriteRenderer.sprite = inProgressSprite;
            }

            rotateFX.Start();
            stateTextUI.text = $"Start";
            EventReceived(State.Start);
        }

        postedSequence.Add(data);

        var expected = sequence[resultSequence.Count];

        resultSequence.Add(new DataPoint
        {
            expected = expected,
            posted = data,
            pass = expected.Equals(data)
        });

        countTextUI.text = $"{postedSequence.Count}|{sequence.Count}";

        var expectedSequence = new List<string>(sequence.Take(postedSequence.Count));
        // var firstNotSecond = postedSequence.Except(expectedSequence).ToList();
        // var secondNotFirst = expectedSequence.Except(postedSequence).ToList();

        // if (firstNotSecond.Any() || secondNotFirst.Any())
        if (!resultSequence.Last<DataPoint>().pass)
        {
            rotateFX.Stop();
            stateTextUI.text = $"Stop";
            PostResult(Result.Fail);
            var dataPoints = GenerateDataPoints(postedSequence, expectedSequence);
            EventReceived(State.Fail, dataPoints);
            EventReceived(State.End);
            complete = true;
        }
        else if (postedSequence.Count == sequence.Count)
        {
            rotateFX.Stop();
            stateTextUI.text = $"Stop";
            PostResult(Result.Pass);
            var dataPoints = GenerateDataPoints(postedSequence, expectedSequence);
            EventReceived(State.Pass, dataPoints);
            EventReceived(State.End);
            complete = true;
        }
    }

    private List<DataPoint> GenerateDataPoints(List<string> postedSequence, List<string> expectedSequence)
    {
        var dataPoints = new List<DataPoint>();

        for (int idx = 0; idx < postedSequence.Count; idx++)
        {
            dataPoints.Add(new DataPoint
            {
                expected = expectedSequence[idx],
                posted = postedSequence[idx],
                pass = postedSequence[idx].Equals(expectedSequence[idx])
            });
        }

        return dataPoints;
    }
    

    private void PostResult(Result result)
    {
        Sprite sprite = (result == Result.Pass) ? passSprite : failSprite;
        AudioClip audioClip = (result == Result.Pass) ? passAudio : failAudio;

        foreach (GameObject face in faces)
        {
            var spriteRenderer = face.GetComponent<SpriteRenderer>() as SpriteRenderer;
            spriteRenderer.sprite = sprite;
        }

        AudioSource.PlayClipAtPoint(audioClip, transform.position, 1.0f);
    }
}
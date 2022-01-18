using System.Linq;
using System.Collections.Generic;

using UnityEngine;

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

    public class DataPoint
    {
        public string expected;
        public string posted;
        public bool result;
    }

    [SerializeField] List<GameObject> faces;
    [SerializeField] Sprite inProgressSprite;

    [Header("Pass Scenario")]
    [SerializeField] Sprite passSprite;
    [SerializeField] AudioClip passAudio;

    [Header("Fail Scenario")]
    [SerializeField] Sprite failSprite;
    [SerializeField] AudioClip failAudio;

    [Header("Config")]
    [SerializeField] List<string> sequence;

    private List<string> postedSequence;
    private bool complete;

    void Awake()
    {
        postedSequence = new List<string>();
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

            EventReceived(State.Start);
        }

        postedSequence.Add(data);

        var expectedSequence = new List<string>(sequence.Take(postedSequence.Count));
        var firstNotSecond = postedSequence.Except(expectedSequence).ToList();
        var secondNotFirst = expectedSequence.Except(postedSequence).ToList();

        if (firstNotSecond.Any() || secondNotFirst.Any())
        {
            PostResult(Result.Fail);
            var dataPoints = GenerateDataPoints(postedSequence, expectedSequence);
            EventReceived(State.Fail, dataPoints);
            EventReceived(State.End);
            complete = true;
        }
        else if (postedSequence.Count == sequence.Count)
        {
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
                result = postedSequence[idx].Equals(expectedSequence[idx])
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
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
        End
    }

    public enum Result
    {
        Pass,
        Fail
    }

    [SerializeField] List<GameObject> faces;

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
            EventReceived(State.Start);
        }

        postedSequence.Add(data);

        var croppedSequence = new List<string>(sequence.Take(postedSequence.Count));
        var firstNotSecond = postedSequence.Except(croppedSequence).ToList();
        var secondNotFirst = croppedSequence.Except(postedSequence).ToList();

        if (firstNotSecond.Any() || secondNotFirst.Any())
        {
            PostResult(Result.Fail);
            EventReceived(State.End, Result.Fail);
            complete = true;
        }
        else if (postedSequence.Count == sequence.Count)
        {
            PostResult(Result.Pass);
            EventReceived(State.End, Result.Pass);
            complete = true;
        }
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
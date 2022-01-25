using UnityEngine;

using TMPro;

public class ScoreCanvasManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI scoreUI;

    private int score;

    public void AddToScore(int points)
    {
        SetScore(score + points);
    }

    public void ResetScore()
    {
        SetScore(0);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreUI.text = score.ToString();
    }
}
using UnityEngine;

using TMPro;

public class ScoreCanvasManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TextMeshProUGUI scoreUI;

    private int score;

    public int Score {
        get
        {
            return score;
        }
        
        set
        {
            this.score = value;
            scoreUI.text = this.score.ToString();
        }
    }

    public void AddToScore(int points)
    {
        Score = score + points;
    }

    public void ResetScore()
    {
        Score = 0;
    }
}
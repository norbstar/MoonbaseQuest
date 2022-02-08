using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour/*CachedObject<GameManager>*/
{
    public UnityEvent onStartActivated;
    public UnityEvent onGameOver;
    public UnityEvent onGameReset;

    public enum State
    {
        NotInPlay,
        InPlay,
        GameOver
    }

    public enum EventType
    {
        Score
    }

    [SerializeField] private int score = 0;

    public delegate void Event(EventType type, object obj);
    public event Event EventReceived;

    public int Score { get { return score; } set { score = value; } }

    public State GameState
    {
        get
        {
            return gameState;
        }
        
        set
        {
            gameState = value;

            if (gameState == State.GameOver)
            {
                onGameOver.Invoke();
            }
        }
    }

    private State gameState;

    public static GameManager GetInstance()
    {
        var obj = GameObject.Find("Game Manager");
        return obj.GetComponent<GameManager>() as GameManager;
    }

    public void StartLevel()
    {
        gameState = State.InPlay;
        onStartActivated.Invoke();
    }

    public void ModifyScoreBy(int score)
    {
        this.score += score;
        EventReceived(EventType.Score, this.score);
    }

    private void ResetScore() => score = 0;

    private void ResetGameState() => gameState = State.NotInPlay;

    public void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        ResetGameState();
        ResetScore();
        onGameReset.Invoke();
    }
}
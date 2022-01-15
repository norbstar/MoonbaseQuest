using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour/*CachedObject<GameManager>*/
{   
    public enum State
    {
        NotInPlay,
        InPlay,
    }

    private State gameState;

    public enum EventType
    {
        Score
    }

    public delegate void Event(EventType type, object obj);

    public event Event EventReceived;

    public State GameState { get { return gameState; } set { gameState = value; } }

    private int score = 0;

    public static GameManager GetInstance()
    {
        var obj = GameObject.Find("Game Manager");
        return obj.GetComponent<GameManager>() as GameManager;
    }

    public void StartLevel()
    {
        gameState = State.InPlay;
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
    }
}
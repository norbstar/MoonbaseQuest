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
        // Debug.Log($"GameManager Instance: Found : {obj != null}");
        return obj.GetComponent<GameManager>() as GameManager;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     Debug.Log($"GameManager State : {gameState}");
    // }

    public void StartLevel()
    {
        // Debug.Log($"GameManager.StartLevel");
        gameState = State.InPlay;
    }

    public void ModifyScoreBy(int score)
    {
        // Debug.Log($"GameManager.ModifyScoreBy:{this.score} += {score.ToString()}");

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
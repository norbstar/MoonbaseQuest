using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

public class TerminalCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] UnityEvent onReset;

    [Header("Timer")]
    [SerializeField] Image timerUI;
    [SerializeField] int durationSec;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreUI;

    private IDictionary<string, string> elements = new Dictionary<string, string>();
    private float startTime, endTime;
    private GameManager gameManager;
    
    void Awake()
    {
        ResolveDependencies();
        startTime = Time.time;
        endTime = startTime + durationSec;
    }

    private void ResolveDependencies()
    {
        gameManager = GameManager.GetInstance();
    }

    void OnEnable()
    {
        gameManager.EventReceived += OnGameManagerEvent;
    }

    void OnDisable()
    {
        gameManager.EventReceived -= OnGameManagerEvent;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.GameState == GameManager.State.InPlay)
        {
            timerUI.fillAmount = timerUI.fillAmount - (Time.deltaTime / durationSec);

            if (timerUI.fillAmount <= 0f)
            {
                gameManager.GameState = GameManager.State.GameOver;
                Reset();
            }
        }
    }

    public void Reset() => onReset.Invoke();

    public void OnGameManagerEvent(GameManager.EventType type, object obj)
    {
        switch (type)
        {
            case GameManager.EventType.Score:
                int score = (int) obj;
                // Debug.Log($"{gameObject.name}.Score:[{score.ToString()}]");
                scoreUI.text = score.ToString();
                break;
        }
    }
}
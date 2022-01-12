using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AsteroidController : MonoBehaviour, IInteractableEvent, IDamage
{
    [SerializeField] GameObject destructionPrefab;

    [Header("The maximum speed with which the asteroid moves")]
    [SerializeField] float maxRotationSpeed = 50f;

    [Header("The maximum scale to render the asteroid")]
    [SerializeField] float maxScale = 1f;

    [Header("The score for destroying an asteroid")]
    [SerializeField] int score = 100;
    
    private GameManager gameManager;
    private GameObject child;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        gameManager = GameManager.GetInstance();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        var scale = Random.Range(maxScale / 4, maxScale);
        transform.localScale = new Vector3(scale, scale, scale);

        child = transform.GetChild(0).gameObject;
        child.transform.Rotate(Random.Range(maxRotationSpeed / 4, maxRotationSpeed), Random.Range(maxRotationSpeed / 4, maxRotationSpeed), Random.Range(maxRotationSpeed / 4, maxRotationSpeed), Space.Self);
    }

    // Update is called once per frame
    void Update()
    {
        child.transform.Rotate(Vector3.up * Time.deltaTime * Random.Range(maxRotationSpeed / 4, maxRotationSpeed), Space.Self);
    }

    public void OnActivate(XRGrabInteractable interactable)
    {
        // Console.Log($"{gameObject.name}.OnActivate:{interactable.name}");

        if (gameManager.GameState == GameManager.State.InPlay)
        {
            ApplyDamage();
        }
    }

    public void ApplyDamage(float damage = 0)
    {
        Destroy(gameObject);

        var instance = GameObject.Instantiate(destructionPrefab, transform.position, transform.rotation) as GameObject;       
        var scale = transform.localScale.x;
        instance.transform.localScale = new Vector3(scale, scale, scale);
        instance.GetComponent<AudioSource>().volume = scale;

        // Debug.Log($"{gameObject.name}.ModifyScoreBy:{score.ToString()}");

        var obj = GameObject.Find("Game Manager");
        obj.GetComponent<GameManager>().ModifyScoreBy(score);
    }
}
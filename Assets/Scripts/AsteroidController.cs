using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AsteroidController : MonoBehaviour, IInteractableEvent, IDamage
{
    [SerializeField] GameObject scorePrefab;
    [SerializeField] float scorePrefabScaleFactor = 4f;
    [SerializeField] GameObject destructionPrefab;

    [Header("The maximum speed with which the asteroid moves")]
    [SerializeField] float maxRotationSpeed = 50f;

    [Header("The maximum scale to render the asteroid")]
    [SerializeField] float maxScale = 1f;

    [Header("The score for destroying an asteroid")]
    [SerializeField] int score = 100;
    
    private GameManager gameManager;
    private GameObject child;
    private new Camera camera;
    private Transform hitOrigin;
    private Vector3 hitPoint;

    void Awake()
    {
        ResolveDependencies();

        camera = Camera.main;
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

    public void OnActivate(XRGrabInteractable interactable, Transform origin, Vector3 hitPoint, Vector3 force)
    {
        // Debug.Log($"{gameObject.name}.OnActivate:{interactable.name} {hitPoint}");

        hitOrigin = origin;
        this.hitPoint = hitPoint;

        if (gameManager.GameState == GameManager.State.InPlay)
        {
            ApplyDamage();
        }
    }

    public void ApplyDamage(float damage = 0)
    {
        Destroy(gameObject);
        RenderDestruction();

        var distance = Vector3.Distance(camera.transform.position, transform.position);
        int distanceAdjustedScore = score * Mathf.RoundToInt(distance);

        RenderScore(distanceAdjustedScore);
        gameManager.ModifyScoreBy(distanceAdjustedScore);
    }

    private void RenderDestruction()
    {
        var instance = GameObject.Instantiate(destructionPrefab, transform.position, transform.rotation) as GameObject;
        var scale = transform.localScale.x;
        instance.transform.localScale = new Vector3(scale, scale, scale);

        if (instance.TryGetComponent<AudioSource>(out var audioSource))
        {
            audioSource.volume = scale;
        }
            
        Destroy(instance, 5f);
    }

    private void RenderScore(int score)
    {
        var instance = GameObject.Instantiate(scorePrefab, transform.position, Quaternion.identity) as GameObject;
        float scaleFactor = scorePrefabScaleFactor;
        instance.transform.localScale = new Vector3(instance.transform.localScale.x * scaleFactor, instance.transform.localScale.y * scaleFactor, instance.transform.localScale.z * scaleFactor);
        FaceToCamera(instance);

        if (instance.TryGetComponent<ScoreCanvasManager>(out var manager))
        {
            manager.Score = score;
        }

        Destroy(instance, 1f);
    }

    private void FaceToCamera(GameObject gameObject)
    {
        Vector3 relativePosition = gameObject.transform.position - camera.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        gameObject.transform.rotation = rotation;
    }
}
using System.Collections;

using UnityEngine;

public class VelocityController : MonoBehaviour, IActuate, INotify
{
    public delegate void OnAsteroidDamaged(GameObject gameObject, GameObject trigger, HealthAttributes healthAttributes);
    public delegate void OnAsteroidDestroyed(GameObject gameObject, GameObject trigger);
    public delegate void OnAsteroidJourneyComplete(GameObject gameObject);

    public class Delegates
    {
        public OnAsteroidDamaged OnAsteroidDamagedDelegate { get; set; }
        public OnAsteroidDestroyed OnAsteroidDestroyedDelegate { get; set; }
        public OnAsteroidJourneyComplete OnAsteroidJourneyCompleteDelegate { get; set; }
    }

    public class Configuration : IConfiguration
    {
        public Vector3 Vector { get; set; }
        public string Zone { get; set; }
        public float Speed { get; set; }
        public float Rotation { get; set; }
    }

    [Header("Partical Effects")]
    [SerializeField] GameObject particalEffectPrefab;
    [SerializeField] AudioClip particalEffectAudio;

    [Header("Components")]
    [SerializeField] GameObject healthBarCanvas;

    private Delegates delegates;
    // private HealthBarSliderUIManager healthBarSliderUIManager;
    private Rigidbody rigidBody;
    private HealthAttributes healthAttributes;
    private float startTime;
    private Vector3 vector;
    private string zone;
    private float speed;
    private float rotation;

    public void Awake()
    {
        ResolveDependencies();

        // healthBarSliderUIManager?.SetMaxHealth(healthAttributes.GetHealthMetric());
    }

    public void Actuate(IConfiguration configuration)
    {
        if (configuration != null)
        {
            if (typeof(Configuration).IsInstanceOfType(configuration))
            {
                vector = ((Configuration) configuration).Vector;
                zone = ((Configuration) configuration).Zone;
                speed = ((Configuration) configuration).Speed;
                rotation = ((Configuration) configuration).Rotation;
            }
        }

        Debug.Log($"{gameObject.GetInstanceID()}.Vector:{vector}");
        Debug.Log($"{gameObject.GetInstanceID()}.Zone:{zone}");
        Debug.Log($"{gameObject.GetInstanceID()}.Speed:{speed}");
        Debug.Log($"{gameObject.GetInstanceID()}.Rotation:{rotation}");

        StartCoroutine(ActuateCoroutine(vector, speed, rotation));
    }

    private IEnumerator ActuateCoroutine(Vector3 vector, float speed, float rotation)
    {
        // rigidBody.AddForce(new Vector3(1f, 0f, 1f));//vector * speed;
        rigidBody.velocity = vector * speed;
        Debug.Log($"{gameObject.GetInstanceID()}.Velocity:{rigidBody.velocity}");
        // rigidBody.angularVelocity = new Vector3(1f, 1f, 1f);//rotation * speed;

        yield return null;
    }

    private void ResolveDependencies()
    {
        // healthBarSliderUIManager = healthBarCanvas.GetComponentInChildren<HealthBarSliderUIManager>() as HealthBarSliderUIManager;
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
        rigidBody = GetComponent<Rigidbody>();
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    private Vector3 CalculatePosition(Vector3 originPosition, Vector3 targetPosition, float fractionComplete)
    {
        return new Vector3(
            Mathf.Lerp(originPosition.x, targetPosition.x, fractionComplete),
            Mathf.Lerp(originPosition.y, targetPosition.y, fractionComplete));
    }

    private Quaternion CalculateRotation(float rotation)
    {
        return Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + (rotation * Time.deltaTime));
    }

    public void OnTriggerEnter(Collider collider)
    {
        GameObject trigger = collider.gameObject;

        if (trigger.CompareTag("Velocity Boundary") && trigger.name.Equals(zone))
        {
            OnJourneyComplete();
        }
        else if (!trigger.CompareTag("Asteroid"))
        {
            var damageAttributes = trigger.GetComponent<DamageAttributes>() as DamageAttributes;

            if (damageAttributes != null)
            {
                float damageMetric = damageAttributes.GetDamageMetric();
                healthAttributes.SubstractHealth(damageMetric);
                // healthBarSliderUIManager?.SetHealth(healthAttributes.GetHealthMetric());

                if (healthAttributes.GetHealthMetric() > 0.0f)
                {
                    StartCoroutine(ManifestDamage());
                    delegates?.OnAsteroidDamagedDelegate?.Invoke(gameObject, trigger, healthAttributes);
                }
                else
                {
                    // var particalEffect = Instantiate(particalEffectPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                    // particalEffect.transform.localScale = transform.localScale;

                    // var particalEffectController = particalEffect.GetComponent<ParticalEffectController>() as ParticalEffectController;
                    // particalEffectController.Actuate();

                    // Destroy(particalEffect, 0.15f);

                    AudioSource.PlayClipAtPoint(particalEffectAudio, Camera.main.transform.position, 2.0f);

                    Destroy(gameObject);

                    delegates?.OnAsteroidDestroyedDelegate?.Invoke(gameObject, trigger);
                }
            }
        }
    }

    private IEnumerator ManifestDamage()
    {
        for (int itr = 0; itr < 3; ++itr)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);

            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void OnJourneyComplete()
    {
        Destroy(gameObject);
        delegates?.OnAsteroidJourneyCompleteDelegate?.Invoke(gameObject);
    }
}
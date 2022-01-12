using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class VelocityLayerManager : MonoBehaviour
{
    [Serializable]
    public class InterSpawnDelaySettings
    {
        public float min = 0.1f;
        public float max = 30.0f;
    }

    [Serializable]
    public class SpeedSettings
    {
        public float min = 0.1f;
        public float max = 2.0f;
    }

    [Serializable]
    public class RotationSettings
    {
        public float min = 5.0f;
        public float max = 90.0f;
    }

    [Serializable]
    public class ScaleSettings
    {
        public float min = 0.5f;
        public float max = 2.5f;
    }

    [Serializable]
    public class SpawnCountSettings
    {
        public int min = 1;
        public int max = 50;
    }

    [Serializable]
    public class MinMaxIntSettings
    {
        public int min;
        public int max;
    }

    [Serializable]
    public class Settings
    {
        public InterSpawnDelaySettings interSpawnDelay;
        public SpeedSettings speed;
        public RotationSettings rotation;
        public ScaleSettings scale;
        public SpawnCountSettings spawnCount;
    }

    [Serializable]
    public class Zone
    {
        public GameObject zone;
        public bool canSpawn = false;
    }

    [SerializeField] Zone[] zones;
    [SerializeField] Settings settings;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] bool clampSpeedToScale = false;

    private class Journey
    {
        public Vector3 Origin { get; set; }
        public Vector3 Vector { get; set; }
        public string Zone { get; set; }
    }

    private int spawnCount, activeSpawnCount;

    void Awake()
    {
        ResolveDependencies();
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(SpawnPrefabs());
    }

    public Zone[] GetZones()
    {
        return zones;
    }

    public Settings GetSettings()
    {
        return settings;
    }

    public GameObject[] GetPrefabs()
    {
        return prefabs;
    }

    public bool GetClampSpeedToScale()
    {
        return clampSpeedToScale;
    }

    private void ResolveDependencies() { }

    private IEnumerator SpawnPrefabs()
    {
        spawnCount = UnityEngine.Random.Range(settings.spawnCount.min, settings.spawnCount.max + 1);
        activeSpawnCount = 0;

        while (true)
        {
            if (activeSpawnCount < spawnCount)
            {
                GameObject prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length - 1)];
                float speed = UnityEngine.Random.Range(settings.speed.min, settings.speed.max);
                float rotation = UnityEngine.Random.Range(settings.rotation.min, settings.rotation.max);
                float scale = UnityEngine.Random.Range(settings.scale.min, settings.scale.max);

                if (clampSpeedToScale)
                {
                    speed /= scale;
                }

                yield return StartCoroutine(SpawnPrefab(prefab, speed, rotation, scale));
                ++activeSpawnCount;

                float delay = UnityEngine.Random.Range(settings.interSpawnDelay.min, settings.interSpawnDelay.max);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator SpawnPrefab(GameObject prefab, float speed, float rotation, float scale)
    {
        Journey journey = CreateJourney();
        Vector3 origin = journey.Origin;
        Vector3 vector = journey.Vector;
        string zone = journey.Zone;

        Debug.Log($"Journey.Origin:{origin}");
        Debug.Log($"Journey.Vector:{vector}");
        Debug.Log($"Journey.Zone:{zone}");

        var instance = Instantiate(prefab, origin, Quaternion.identity) as GameObject;
        instance.transform.SetParent(transform);
        instance.transform.localScale = new Vector3(scale, scale, scale);

        var asteroidController = instance.GetComponent<VelocityController>() as VelocityController;

        if (asteroidController != null)
        {
            asteroidController.RegisterDelegates(new VelocityController.Delegates
            {
                OnAsteroidDamagedDelegate = OnAsteroidDamaged,
                OnAsteroidDestroyedDelegate = OnAsteroidDestroyed,
                OnAsteroidJourneyCompleteDelegate = OnAsteroidJourneyComplete
            });

            asteroidController.Actuate(new VelocityController.Configuration
            {
                Vector = vector,
                Zone = zone,
                Speed = speed,
                Rotation = rotation
            });
        }

        yield return null;
    }

    private Journey CreateJourney()
    {
        BoxCollider collider;

        IList<Zone> availableZones = zones.ToList();

        var availableOriginZones = zones.Where(z => z.canSpawn).ToList<Zone>();
        int originIndex = PickZone(availableOriginZones);
        var originZone = availableOriginZones[originIndex].zone;
        Debug.Log($"Journey.OriginIndex:{originIndex} : {originZone.name}");
        collider = originZone.GetComponent<BoxCollider>() as BoxCollider;
        Vector3 originZoneSize = collider.size;
        Vector3 origin = PickPositionInScope(originZone.transform.position, originZoneSize);

        var availableTargetZones = zones.Where(z => !z.canSpawn).ToList<Zone>();
        int targetIndex = PickZone(availableTargetZones);
        var targetZone = availableTargetZones[targetIndex].zone;
        Debug.Log($"Journey.TargetIndex:{targetIndex} : {targetZone.name}");
        collider = targetZone.GetComponent<BoxCollider>() as BoxCollider;
        Vector3 targetZoneSize = collider.size;
        Vector3 target = PickPositionInScope(targetZone.transform.position, targetZoneSize);
        
        return new Journey
        {
            Origin = origin,
            Zone = availableTargetZones[targetIndex].zone.name,
            Vector = (target - origin).normalized
        };
    }

    private bool enableCollisions = true;

    public void EnableCollisions(bool active)
    {
        enableCollisions = !enableCollisions;

        var iStates = gameObject.GetComponentsInChildren<IState>() as IState[];

        if (iStates != null)
        {
            foreach (IState iState in iStates)
            {
                iState.SetActive(active);
            }
        }
    }

    private void HandleAsteroidEndOfLife()
    {
        --activeSpawnCount;
        spawnCount = UnityEngine.Random.Range(settings.spawnCount.min, settings.spawnCount.max + 1);
    }

    public void OnAsteroidDamaged(GameObject gameObject, GameObject trigger, HealthAttributes healthAttributes) { }

    public void OnAsteroidDestroyed(GameObject gameObject, GameObject trigger)
    {
        HandleAsteroidEndOfLife();
    }

    public void OnAsteroidJourneyComplete(GameObject gameObject)
    {
        HandleAsteroidEndOfLife();
    }

    private int PickZone(IList<Zone> zones)
    {
        return UnityEngine.Random.Range(0, zones.Count);
    }

    private Vector3 PickPositionInScope(Vector3 position, Vector3 scope)
    {
        return new Vector3
        {
            x = UnityEngine.Random.Range(position.x - (scope.x / 2), position.x + (scope.x / 2)),
            y = UnityEngine.Random.Range(position.y - (scope.y / 2), position.y + (scope.y / 2)),
            z = UnityEngine.Random.Range(position.z - (scope.z / 2), position.z + (scope.z / 2))
        };
    }
}
using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class SpherePropagatorSyncedToScale : BaseSpherePropagator
    {
        [Header("Config")]
        [SerializeField] GameObject sphere;
        public float Scale { get { return scale; } }

        private float scale = 5f;
        private int defaultLayerMask;
        private List<OpacityModifierByProximity> modifiers;

        void Awake()
        {
            defaultLayerMask = LayerMask.GetMask("Default");
            sphere.transform.localScale = Vector3.one * scale;

            modifiers = new List<OpacityModifierByProximity>();
        }

        void FixedUpdate()
        {
            scale = sphere.transform.localScale.z;
            radius = scale * 0.5f;
            Collider[] hits = Physics.OverlapSphere(transform.TransformPoint(Vector3.zero), radius);
            // Debug.Log($"Has Hits : {hits.Length > 0}");

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    GameObject trigger = hit.gameObject;
                    Vector3 point = hit.ClosestPoint(trigger.transform.position);
                    float distance = Vector3.Distance(transform.position, point);
                    // Debug.Log($"GameObject : {trigger.name} Distance : {distance}");

                    if (trigger.TryGetComponent<OpacityModifierByProximity>(out OpacityModifierByProximity modifier))
                    {
                        if (!modifiers.Contains(modifier))
                        {
                            modifiers.Add(modifier);
                            modifier.RegisterWithSource(this);
                        }
                    }
                }
            }
        }

        void FixedUpdateAlt1()
        {
            bool hasHits = Physics.SphereCast(transform.TransformPoint(Vector3.zero), radius, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, defaultLayerMask);
            Debug.Log($"Has Hits : {hasHits}");

            if (hasHits)
            {
                var objectHit = hitInfo.transform.gameObject;
                var point = hitInfo.point;
                var distance = Vector3.Distance(transform.position, point);
                Debug.Log($"GameObject : {objectHit.name} Distance : {distance}");
            }
        }

        void FixedUpdateAlt2()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.TransformPoint(Vector3.zero), radius, transform.forward, Mathf.Infinity, defaultLayerMask);
            Debug.Log($"Has Hits : {hits.Length > 0}");

            if (hits.Length > 0)
            {
                foreach (RaycastHit hitInfo in hits)
                {
                    var objectHit = hitInfo.transform.gameObject;
                    var point = hitInfo.point;
                    var distance = Vector3.Distance(transform.position, point);
                    Debug.Log($"GameObject : {objectHit.name} Distance : {distance}");
                }
            }
        }
    }
}
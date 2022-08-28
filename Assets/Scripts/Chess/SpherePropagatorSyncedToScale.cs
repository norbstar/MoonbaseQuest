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

            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    GameObject trigger = hit.gameObject;
                    Vector3 point = hit.ClosestPoint(trigger.transform.position);
                    float distance = Vector3.Distance(transform.position, point);

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
    }
}
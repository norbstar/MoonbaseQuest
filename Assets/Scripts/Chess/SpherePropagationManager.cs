using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    public class SpherePropagationManager : BaseSpherePropagator
    {
        [Header("Config")]
        [SerializeField] GameObject sphere;
        public float Scale { get { return scale; } }

        private float scale = 1f;
        private int defaultLayerMask;
        private List<IPropagationEvent> receivers;

        void Awake()
        {
            defaultLayerMask = LayerMask.GetMask("Default");
            sphere.transform.localScale = Vector3.one * scale;

            receivers = new List<IPropagationEvent>();
        }

        // Update is called once per frame
        void Update()
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

                    if (trigger.TryGetComponent<IPropagationEvent>(out IPropagationEvent receiver))
                    {
                        if (!receivers.Contains(receiver))
                        {
                            receivers.Add(receiver);
                        }
                    }
                }
            }

            foreach (IPropagationEvent receiver in receivers)
            {
                receiver.OnUpdate(transform.position, radius);
            }
        }
    }
}
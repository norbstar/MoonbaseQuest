using System.Collections.Generic;

using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(Renderer))]
    public class OpacityModifierByProximity : MonoBehaviour
    {
        // [Header("Components")]
        // [SerializeField] List<Material> materials;

        [Header("Config")]
        [SerializeField] float range = 1f;

        private new Renderer renderer;
        private BaseSpherePropagator propagator;

        void Awake()
        {
            ResolveDependencies();
            var name = renderer.material.name.Replace(" (Instance)","");
            Debug.Log($"{renderer.material.name} -> {name}");
            renderer.material = Instantiate(Resources.Load($"Materials/{name}") as Material);
            // renderer.material = Instantiate(Resources.Load("Materials/Transparent Blue") as Material);
        }

        private void ResolveDependencies() => renderer = GetComponent<Renderer>() as Renderer;
        
        public void RegisterWithSource(BaseSpherePropagator propagator) => this.propagator = propagator;

        public void UnregisterWithSource() => this.propagator = null;

        // Update is called at fixed intervals of 1/60th of a second
        void FixedUpdate()
        {
            if (propagator == null) return;
            
            if (TryGetNormalizedAlpha(propagator.Radius, out var normalizedAlpha))
            {
                renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, normalizedAlpha);
            }
        }

        private bool TryGetNormalizedAlpha(float radius, out float result)
        {
            if (propagator == null)
            {
                result = default(float);
                return false;
            };

            float distance = Vector3.Distance(transform.position, propagator.transform.position) - propagator.Radius;
            
            if (distance >= 0f)
            {
                result = 0f;
                return true;    
            }

            distance = Mathf.Abs(distance);
            // Debug.Log($"Game Object : {gameObject.name} Distance : {distance}");
            float value = Mathf.Clamp(distance, 0f, range);
            // Debug.Log($"Game Object : {gameObject.name} Value : {value}");
            float normalizedValue = (value - 0f) / (range - 0f);
            // Debug.Log($"Game Object : {gameObject.name} Normalized Value : {normalizedValue}");

            result = normalizedValue;
            return true;
        }
    }
}
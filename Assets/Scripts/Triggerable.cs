using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Triggerable : MonoBehaviour
{
    public delegate void OnEnter(Collider collider);
    public delegate void OnExit(Collider collider);

    public class Delegates
    {
        public OnEnter OnEnter { get; set; }
        public OnExit OnExit { get; set; }
    }

    public Delegates Subscriptions { set { delegates = value; } }

    private Delegates delegates;

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log($"{gameObject.name}.Triggerable:OnTriggerEnter");
        delegates?.OnEnter?.Invoke(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        Debug.Log($"{gameObject.name}.Triggerable:OnTriggerEnter");
        delegates?.OnExit?.Invoke(collider);
    }
}
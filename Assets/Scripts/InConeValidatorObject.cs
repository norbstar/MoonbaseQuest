using UnityEngine;

public class InConeValidatorObject : MonoBehaviour
{
    private InConeValidator validator;

    void OnDrawGizmos()
    {
        if (validator == null)
        {
            validator = GameObject.FindObjectOfType<InConeValidator>() as InConeValidator;
        }

        Gizmos.color = validator.IsInCone(transform.position) ? Color.green : Color.red;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.1f);
    }
}
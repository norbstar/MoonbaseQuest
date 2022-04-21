using UnityEngine;

public interface IFocus
{
    void GainedFocus(GameObject gameObject, Vector3 focalPoint);

    void LostFocus(GameObject gameObject);
}
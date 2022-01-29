using UnityEngine;

public interface IFocus
{
    void GainedFocus(GameObject gameObject);

    void LostFocus(GameObject gameObject);
}
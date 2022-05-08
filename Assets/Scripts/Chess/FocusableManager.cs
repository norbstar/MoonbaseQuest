using UnityEngine;

namespace Chess
{
    public class FocusableManager : MonoBehaviour, IFocus
    {
        public delegate void Event(FocusableManager manager, FocusType focusType);
        public event Event EventReceived;

        public virtual void GainedFocus(GameObject gameObject, Vector3 point) => EventReceived?.Invoke(this, FocusType.OnFocusGained);

        public virtual void LostFocus(GameObject gameObject) => EventReceived?.Invoke(this, FocusType.OnFocusLost);
    }
}
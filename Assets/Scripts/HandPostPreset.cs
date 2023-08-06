using UnityEngine;

[RequireComponent(typeof(HandAnimationController))]
public class HandPostPreset : MonoBehaviour
{
    public enum Pose
    {
        Default,
        Pinch,
        Fist
    }

    [Header("Config")]
    [SerializeField] Pose pose;

    private HandAnimationController controller;

    void Awake() => controller = GetComponent<HandAnimationController>();

    // Start is called before the first frame update
    void Start()
    {
        switch (pose)
        {
            case Pose.Pinch:
                controller.SetFloat("Trigger", 1f);
                break;

            case Pose.Fist:
                controller.SetFloat("Grip", 1f);
                break;
        }
    }
}
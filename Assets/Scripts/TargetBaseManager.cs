using UnityEngine;

[RequireComponent(typeof(FX.RotateFX))]
public class TargetBaseManager : MonoBehaviour
{
    public float Speed { get { return speed; } }
    
    private FX.RotateFX rotateFX;
    private float speed;

    void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        rotateFX = GetComponent<FX.RotateFX>() as FX.RotateFX;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
        rotateFX.SpeedY = speed;
    }
}
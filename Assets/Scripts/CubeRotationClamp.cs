using UnityEngine;

public class CubeRotationClamp : MonoBehaviour
{
    // [SerializeField] float maxOffset = 10f;

    // void Start()
    // {
    //     TestSpec(44f, true);
    //     TestSpec(45f, true);
    //     TestSpec(50f, false);
    //     TestSpec(316f, true);
    //     TestSpec(315f, true);
    //     TestSpec(310f, false);
    // }

    // private void TestSpec(float y, bool shouldPass)
    // {
    //     var value = Quaternion.Euler(0f, y, 0f);
    //     var expectation = shouldPass ? "PASSED" : "FAILED";

    //     if (OutsideSpec(value, out Quaternion result))
    //     {
    //         Debug.Log($"{value.eulerAngles} FAILED | Expectation : {expectation}");
    //     }
    //     else
    //     {
    //         Debug.Log($"{value.eulerAngles} PASSED | Expectation : {expectation}");
    //     }
    // }

    private bool OutsideSpec(Quaternion value, out Quaternion adjustment)
    {
        if (Mathf.Floor(value.eulerAngles.y) > 45f && Mathf.Floor(value.eulerAngles.y) < 315f)
        {
            Debug.Log($"{value.eulerAngles.y} FAIL TEST");

            var minDiff = Mathf.Abs(value.eulerAngles.y - 45f);
            var maxDiff = Mathf.Abs(value.eulerAngles.y - 315f);
            adjustment = Quaternion.Euler(value.eulerAngles.x, (minDiff > maxDiff) ? 45f : 315f, value.eulerAngles.z);
            return true;
        }

        Debug.Log($"{value.eulerAngles.y} PASS TEST");

        adjustment = default(Quaternion);
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log($"Angle (1a) : {transform.localRotation.eulerAngles.y}");
        // Angle (1a) : 6.3502
        // Angle (1a) : 356.3275
        // Debug.Log($"Angle (1b) : {transform.localRotation.y}");
        // Angle (1b) : 0.05538753
        // Angle (1b) : -0.03204341
        // Debug.Log($"Angle (2a) : {transform.rotation.eulerAngles.y}");
        // Angle (2a) : 6.3502
        // Angle (2a) : 356.3275
        // Debug.Log($"Angle (2b) : {transform.rotation.y}");
        // Angle (2b) : 0.05538753
        // Angle (2b) : -0.03204341
        // Debug.Log($"Angle (3) : {transform.eulerAngles.y}");
        // Angle (3) : 6.3502
        // Angle (3) : 356.3275

        // var y = Mathf.Clamp(transform.localRotation.eulerAngles.y, 45f, 315f);
        // transform.rotation = Quaternion.Euler(transform.eulerAngles.x, y, transform.eulerAngles.z);

        // if (transform.eulerAngles.y > maxAngle)
        // { 
        //     Debug.Log($"1");
        //     var y = Mathf.Clamp(transform.localRotation.rotation.y, 0f, maxAngle);
        //     transform.rotation = Quaternion.Euler(transform.rotation.x, y, transform.rotation.z);
        // }
        // else if (transform.eulerAngles.y < (360f - maxAngle))
        // {
        //     Debug.Log($"2");
        //     var y = Mathf.Clamp(transform.localRotation.rotation.y, 0f, 360f - maxAngle);
        //     transform.rotation = Quaternion.Euler(transform.rotation.x, y, transform.rotation.z);
        // }

        if (OutsideSpec(transform.rotation, out Quaternion adjustment))
        {
            transform.rotation = adjustment;
        }
    }
}
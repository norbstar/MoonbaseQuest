using UnityEngine;

using TMPro;

public class LookAtCameraTransform : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] GameObject origin;
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] float nearDistance;
    [SerializeField] float farDistance;
    [SerializeField] bool scaleWithDistance = false;
    [SerializeField] float offset = 0.1f;
    
    private Vector3 originalScale;
    private bool isShown;

    protected virtual void Awake()
    {
        originalScale = transform.localScale;
        textUI.text = transform.parent.name;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(origin.transform.position, camera.transform.position);

        bool inRange = (distance >= nearDistance && distance <= farDistance);
        ShowChildren(inRange);
        
        // for (int idx = 0; idx < transform.childCount; idx++)
        // {
        //     var child = transform.GetChild(idx);
        //     Debug.Log($"Child {child.name} {child.transform.localScale}");
        // }

        ScaleWithDistance(distance);

        if (inRange)
        {
            Vector3 midPoint = Vector3.Lerp(origin.transform.position, camera.transform.position, offset);
            Vector3 relativePosition = camera.transform.position - origin.transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
            transform.rotation = rotation;
            transform.position = midPoint;
        }
    }

    private void ShowChildren(bool show)
    {
        if (isShown == show) return;

        for (int idx = 0; idx < transform.childCount; idx++)
        {
            var child = transform.GetChild(idx);
            child.gameObject.SetActive(show);
        }

        isShown = show;
    }

    private void ScaleWithDistance(float distance)
    {
        if (!scaleWithDistance) return;

        float nearDistanceScaleFactor = 1f;
        float farDistanceScaleFactor = 0.2f;

        // for (int idx = 0; idx < transform.childCount; idx++)
        // {
        //     var child = transform.GetChild(idx);
        //     Debug.Log($"Child {child.name} Original Scale:{originalScale}");

        //     var scaleFactor = ( (distance - nearDistance) / (farDistance - nearDistance) ) * (farDistanceScaleFactor - nearDistanceScaleFactor) + nearDistanceScaleFactor;
        //     child.transform.localScale = originalScale * scaleFactor;

        //     Debug.Log($"Child Child {child.name} New Scale:{scaleFactor} {child.transform.localScale}");
        // }

        var scaleFactor = ( (distance - nearDistance) / (farDistance - nearDistance) ) * (farDistanceScaleFactor - nearDistanceScaleFactor) + nearDistanceScaleFactor;
        transform.localScale = originalScale * scaleFactor;
        Debug.Log($"New Scale:{scaleFactor} {transform.localScale}");
    }
}
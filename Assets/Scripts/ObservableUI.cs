using UnityEngine;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(ProximityBaseController))]
public class ObservableUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] Image image;
    [SerializeField] Sprite observableSprite;
    [SerializeField] Sprite focusSprite;

    [SerializeField] float nearDistance = 0.5f;
    [SerializeField] float farDistance = 2.5f;
    [SerializeField] float transitionPoint = 0.75f;

    private ProximityBaseController baseController;
    private Vector3 originalScale, baseScale;
    private float transitionDistance;

    void Awake()
    {
        ResolveDependencies();

        if (textUI != null)
        {
            textUI.text = transform.parent.name;
        }

        originalScale = transform.localScale;
        var referenceScale = baseController.Origin.transform.localScale;

        baseScale = new Vector3
        {
            x = 1f / (referenceScale.x / 0.1f),
            y = 1f / (referenceScale.y / 0.1f),
            z = 1f / (referenceScale.z / 0.1f)
        };

        transform.localScale = baseScale;
        transitionDistance = (nearDistance + ((farDistance - nearDistance) * (1f - transitionPoint)));
    }

    private void ResolveDependencies()
    {
        baseController = GetComponent<ProximityBaseController>() as ProximityBaseController;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(baseController.Origin.transform.position, baseController.Camera.transform.position);
        bool inRange = (distance >= nearDistance && distance <= farDistance);
        baseController.InRange = inRange;

        if (inRange)
        {
            if (distance <= transitionDistance)
            {
                SetFocusMode(distance);
            }
            else
            {
                SetObservableMode();
            }
        }
    }

    private void SetObservableMode()
    {
        image.sprite = observableSprite;
        textUI.gameObject.SetActive(false);
        transform.localScale = baseScale;
    }

    private void SetFocusMode(float distance)
    {
        image.sprite = focusSprite;
        textUI.gameObject.SetActive(true);

        ScaleWithinFocusableRange(distance);
    }

    private void ScaleWithinFocusableRange(float distance)
    {
        float nearDistanceScaleFactor = 4f;
        float farDistanceScaleFactor = 1f;

        var scaleFactor = ( (distance - nearDistance) / (transitionDistance - nearDistance) ) * (farDistanceScaleFactor - nearDistanceScaleFactor) + nearDistanceScaleFactor;
        transform.localScale = baseScale * scaleFactor;
    }
}
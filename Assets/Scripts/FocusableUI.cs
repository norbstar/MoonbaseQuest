using UnityEngine;
using UnityEngine.UI;

using TMPro;

[RequireComponent(typeof(ProximityBaseController))]
public class FocusableUI : ObservableUI
{
    [SerializeField] GameObject focus;
    [SerializeField] Image focusImage;
    [SerializeField] TextMeshProUGUI textUI;
    [SerializeField] float transitionDistance = 1.5f;

    private Vector3 originalFocusScale;

    public override void Awake()
    {
        base.Awake();

        originalFocusScale = focus.transform.localScale;

        if (textUI != null)
        {
            textUI.text = transform.parent.name;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

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
        focus.gameObject.SetActive(false);
        focus.transform.localScale = originalFocusScale;
    }

    private void SetFocusMode(float distance)
    {
        focus.gameObject.SetActive(true);
        ScaleWithinFocusableRange(distance);
    }

    private void ScaleWithinFocusableRange(float distance)
    {
        float farDistanceScaleFactor = 1.85f;
        float nearDistanceScaleFactor = farDistanceScaleFactor * 5.5f;

        var scaleFactor = ( (distance - NearDistance) / (transitionDistance - NearDistance) ) * (farDistanceScaleFactor - nearDistanceScaleFactor) + nearDistanceScaleFactor;
        focus.transform.localScale = originalFocusScale * scaleFactor;
    }
}
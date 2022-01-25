using System;
using System.Collections;

using UnityEngine;

public class Movable : MonoBehaviour
{
    [SerializeField] float speed;

    public void MoveTo(WaypointManager.Waypoint waypoint, Action onComplete = null)
    {
        var distance = Vector2.Distance(transform.position, waypoint.position);
        var duration = distance / speed;
        var startAt = transform.position;
        var endAt = waypoint.position;

        StartCoroutine(MoveToCoroutine(startAt, endAt, duration, onComplete));
    }

    private IEnumerator MoveToCoroutine(Vector3 startAt, Vector3 endAt, float duration, Action onComplete)
    {
        bool complete = false;
        float elapsedDuration = 0.0f;

        while (!complete)
        {
            elapsedDuration += Time.deltaTime;
            float fractionComplete = elapsedDuration / duration;
            complete = (fractionComplete >= 1.0f);

            if (!complete)
            {
                transform.position = Vector3.Lerp(startAt, endAt, (float) fractionComplete);
            }
            else
            {
                onComplete?.Invoke();
            }
            
            yield return null;
        }
    }
}
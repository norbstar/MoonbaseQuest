using System;
using System.Collections;

using UnityEngine;

namespace Chess
{
    public class Sandbox : MonoBehaviour
    {
        [SerializeField] Vector3 target;
        [SerializeField] float movementSpeed = 1f;
        [SerializeField] float apexHeight = 0.25f;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return StartCoroutine(MoveCoroutine(target, movementSpeed));
        }

        private float RoundFloat(float value)
        {
            return (float) Math.Round((value * 100f) / 100f, 2);
        }

        private Vector3 RoundVector3(Vector3 value)
        {
            return new Vector3
            {
                x = (float) Math.Round((value.x * 100f) / 100f, 2),
                y = (float) Math.Round((value.y * 100f) / 100f, 2),
                z = (float) Math.Round((value.z * 100f) / 100f, 2)
            };
        }

        private IEnumerator MoveCoroutine(Vector3 target, float movementSpeed)
        {
            Vector3 targetPosition = RoundVector3(target);
            Vector3 startPosition = RoundVector3(transform.localPosition);
            
            Debug.Log($"MoveCoroutine Piece : {name} From : [{startPosition.x}, {startPosition.y}, {startPosition.z}] To : [{targetPosition.x}, {targetPosition.y}, {targetPosition.z}]");
            
            float distance = Vector3.Distance(startPosition, targetPosition);
            float duration = distance * movementSpeed;
            float timestamp = 0;

            if (duration < 1f) duration = 1f;

            while ((RoundVector3(transform.localPosition) != targetPosition))
            {
                timestamp += Time.deltaTime;

                float timeframe = Mathf.Clamp01(timestamp / duration);

                transform.localPosition = Parabola.MathParabola.Parabola(startPosition, targetPosition, apexHeight, timeframe);
                Debug.Log($"MoveCoroutine Position : [{transform.localPosition.x}, {transform.localPosition.y}, {transform.localPosition.z}]");

                if (timeframe >= 1f)
                {
                    transform.localPosition = targetPosition;
                }

                yield return null;
            }

            Debug.Log($"MoveCoroutine End");
        }
    }
}
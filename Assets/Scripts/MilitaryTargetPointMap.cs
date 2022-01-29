using System;
using System.Collections.Generic;

using UnityEngine;

public class MilitaryTargetPointMap : BaseManager
{
    [Serializable]
    public class PointMap
    {
        public List<Transform> transforms;
        public int value;
    }

    [SerializeField] List<PointMap> pointMaps;
    // [SerializeField] float cullingDistance = 0.1f;

    public List<PointMap> PointMaps { get { return pointMaps; } }

    public bool TryGetValueFromPoint(Transform pointTransform, out int value)
    {
        int? preferredValue = null;
        float? preferredDistance = null;

        foreach (PointMap pointMap in pointMaps)
        {
            var thisValue = pointMap.value;

            foreach (Transform thisTransform in pointMap.transforms)
            {
                var distance = Vector3.Distance(pointTransform.position, thisTransform.position);
                Log($"{gameObject.name}.TryGetValueFromPoint:{pointTransform.position} {thisTransform.position} Distance : {distance} Value : {thisValue}");

                if ((!preferredDistance.HasValue) || (distance < preferredDistance))
                {
                    preferredDistance = distance;
                    preferredValue = thisValue;
                }
            }
        }

        Log($"{gameObject.name}.TryGetValueFromPoint:Distance : {preferredDistance} Value : {preferredValue}");
        
        if (preferredValue.HasValue)
        {
            value = preferredValue.Value;
            return true;
        }
        
        value = default(int);
        return false;
    }
}
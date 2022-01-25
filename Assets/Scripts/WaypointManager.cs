using System;
using System.Collections.Generic;

using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] private Movable movable;

    [Serializable]
    public class Waypoint
    {
        [Header("Transform")]
        public Vector3 position;
    }

    [SerializeField] List<Waypoint> waypoints;

    private int nextWayPointIdx;

    // Start is called before the first frame update
    void Start()
    {
        MoveToNextWaypoint();
    }

    private void MoveToNextWaypoint()
    {
        var nextWayPoint = waypoints[nextWayPointIdx];
        movable.MoveTo(nextWayPoint, MoveToNextWaypoint);
        nextWayPointIdx = (nextWayPointIdx + 1 < waypoints.Count) ? nextWayPointIdx + 1 : 0;
    }
}
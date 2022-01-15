using System;

using UnityEngine;

public class DockManager : Gizmo
{
    [Serializable]
    public class OccupancyData
    {
        public bool occupied;
        public GameObject gameObject;
    }
    
    [Header("Status")]
    [SerializeField] private OccupancyData occupied;

    public OccupancyData Occupied { get { return (occupied != null) ? occupied : new OccupancyData(); } set { occupied = value; } }
}
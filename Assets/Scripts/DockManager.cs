using System;

using UnityEngine;

public class DockManager : Gizmo
{
    [Serializable]
    public class OccupancyData
    {
        public bool occupied;
        public GameObject gameObject;
        public bool useGravity;
    }
    
    [Header("Status")]
    [SerializeField] private OccupancyData occupied;

    public OccupancyData Data { get { return (occupied != null) ? occupied : new OccupancyData(); } set { occupied = value; } }

    public void Free() => occupied = new DockManager.OccupancyData();
}
using UnityEngine;

public class DockManager : Gizmo
{
    [Header("Status")]
    [SerializeField] private bool occupied;

    public bool Occupied { get { return occupied; } set { occupied = value; } }
}
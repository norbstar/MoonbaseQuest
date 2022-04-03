using System.Reflection;

using UnityEngine;

public class FlightStickManager : BaseManager
{
    private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

    [Header("Config")]
    [SerializeField] StickInteractableManager stick;
}
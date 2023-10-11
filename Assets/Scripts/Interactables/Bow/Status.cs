using System;

namespace Interactables.Bow
{
    [Flags]
    public enum Status
    {
        None = 0,
        CanActuate = 1,
        Actuating = 2,
        Firing = 4
    }
}
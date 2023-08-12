using System;

namespace Interactables.Bow
{
    [Flags]
    public enum Status
    {
        None = 0,
        IsHeld = 1,
        CanActuate = 2,
        Actuating = 4,
        Firing = 8
    }
}
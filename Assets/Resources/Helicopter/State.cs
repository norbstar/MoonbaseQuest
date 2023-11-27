using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public enum State
    {
        Idle,
        EngagePower,
        EngagingPower,
        Active,
        StabilisingElevation,
        CutPower,
        CuttingPower
    }
}
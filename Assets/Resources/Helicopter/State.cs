using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Helicopter
{
    public enum State
    {
        Idle,
        EngagingPower,
        Active,
        StabilisingElevation,
        StabilisingDescent,
        CuttingPower
    }
}
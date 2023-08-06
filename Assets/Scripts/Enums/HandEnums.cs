using System;

using UnityEngine;

namespace Enum
{
    public class HandEnums : MonoBehaviour
    {
        [Flags]
        public enum Action
        {
            None = 0,
            Holding = 1,
            Pinching = 2,
            Pointing = 4,
            Clawing = 8,
            Hovering = 16
        }
    }
}
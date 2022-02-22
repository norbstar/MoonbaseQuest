using System;

using UnityEngine;

namespace Enum
{
    public class HandEnums : MonoBehaviour
    {
        [Flags]
        public enum State
        {
            None = 0,
            Grip = 1,
            Pinch = 2,
            Point = 4,
            Claw = 8,
            Hover = 16
        }
    }
}